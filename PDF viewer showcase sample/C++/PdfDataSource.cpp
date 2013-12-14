// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "pch.h"

using namespace Concurrency;
using namespace Windows::Data::Pdf;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml;

namespace PdfShowcase
{
	PdfDataSource::~PdfDataSource()
	{
		DeleteCriticalSection(&criticalSectionPageLoad);

	}

	/// <summary>
	/// Constructor for PdfDataSource
	/// </summary>
	/// <param name="pdfDocumentLocal">PDF Document pointer returned by PDF API's</param>
	/// <param name="pageSize">Size of rendered pages</param>
	/// <param name="pagesToLoad">Number of pages to load on each request</param>
	/// <param name="surfaceType">Surface Type SIS/VSIS</param>
	PdfDataSource::PdfDataSource(_In_ PdfDocument^ pdfDocumentLocal, _In_ Windows::Foundation::Size pageSize, _In_ unsigned int pagesToLoad, _In_ SurfaceType surfaceType)
		: pdfDocument(pdfDocumentLocal), pagesToLoad(pagesToLoad), surfaceType(surfaceType)
	{
		// Creating new renderer which inturn creates all required reesources needed to render a page
		renderer = ref new Renderer(
			Window::Current->Bounds,
			DisplayInformation::GetForCurrentView()->LogicalDpi
			);

		// Initializing other private variables
		pagesCurrentlyLoaded = 0;
		zoomFactor = 1;
		pageCount = pdfDocument->PageCount;
		dispatcher = Windows::ApplicationModel::Core::CoreApplication::MainView->CoreWindow->Dispatcher;

		// Initialize the critical section one time only.
		InitializeCriticalSectionEx(&criticalSectionPageLoad, 0x00000400, CRITICAL_SECTION_NO_DEBUG_INFO);

		// Initialize semaphore used for synchronizing update of data source
		vectorUpdateSemaphore = CreateSemaphoreEx(NULL, 1, 1, L"vectorUpdateSemaphore", 0, SEMAPHORE_ALL_ACCESS);

		// Set item size based on the passed page size
		this->SetItemSize(pageSize);

	}

	/// <summary>
	/// Overrides method of base class
	/// </summary>
	/// <param name="c">Token which can be used for canceling tasks if needed</param>
	/// <param name="count">Number of items to be loaded</param>
	/// <returns>Array of loaded items</returns>
	task<Windows::UI::Xaml::Data::LoadMoreItemsResult> PdfDataSource::LoadMoreItemsOverrideAsync(_In_ cancellation_token c, _In_ unsigned int count)
	{
		return Concurrency::task<Windows::UI::Xaml::Data::LoadMoreItemsResult>(
			[this, count]()->Windows::UI::Xaml::Data::LoadMoreItemsResult {

				// Check for cancelation. 
				if (is_task_cancellation_requested())
				{
					// Cancel the current task.
					cancel_current_task();
				}
				else
				{
					Windows::UI::Xaml::Data::LoadMoreItemsResult result;

					// Entering critical section
					EnterCriticalSection(&criticalSectionPageLoad);

					unsigned int toGenerate = min(pagesToLoad, pageCount - pagesCurrentlyLoaded);
					unsigned int requestIndex = this->pagesCurrentlyLoaded;

					this->pagesCurrentlyLoaded += toGenerate;

					if (this->Size < this->pagesCurrentlyLoaded)
					{
						unsigned int count = max(0, this->pagesCurrentlyLoaded - this->Size);

						// Creating placeholders for items to be loaded
						if (count > 0)
						{
							run_async_non_interactive(Windows::UI::Core::CoreDispatcherPriority::High, [this, count]()
							{
								CreatePlaceholderItems(count);
							});
						}
					}
					
					// Leaving critical section
					LeaveCriticalSection(&criticalSectionPageLoad);

					// Note that Platform::Collections::Vector<Platform::Object^> can be returned as Windows::Foundation::Collections::IVector<Platform::Object^>
					std::vector<task<void>> taskGroup;

					for (unsigned int i = requestIndex, len = requestIndex + toGenerate; i < len; i++)
					{
						if (is_task_cancellation_requested())
						{
							// Cancel the current task.
							cancel_current_task();
						}
						else
						{
							// Creating resources to render page
							taskGroup.push_back(Concurrency::task<void>([this, i](void)
							{
								// Dispatching task on UI thread to create resources required for rendering
								run_async_non_interactive(Windows::UI::Core::CoreDispatcherPriority::Normal, [this, i]()
								{
									// Loading and rendering pages on the surfaces created for each page
									if (Size > i)
									{
										// Locking before updating datasource vector
										WaitForSingleObjectEx(vectorUpdateSemaphore, INFINITE, false);
										this->SetAt(i, this->LoadPages(i, pdfDocument));
										ReleaseSemaphore(vectorUpdateSemaphore, 1, NULL);
									}
								});
							}));
						}
					}

					// Joining all tasks created in above step and waiting for it to complete
					when_all(begin(taskGroup), end(taskGroup)).wait();

					result.Count = toGenerate;
					return result;
				}
		});
	}

	/// <summary>
	/// Override for HasMoreItems
	/// </summary>
	/// <returns>Return true if there are more items to be loaded</returns>
	bool PdfDataSource::HasMoreItemsOverride()
	{
		return pagesCurrentlyLoaded < pageCount;
	}

	/// <summary>
	/// Following function determines the size of rendered items
	/// </summary>
	/// <param name="pageSizeLocal">Page size passed by user</param>
	void PdfDataSource::SetItemSize(_In_ Windows::Foundation::Size pageSizeLocal)
	{
		// Getting height and width of the rendered surface based on parameter value size
		// If Size is 0 than pages of original width and height are rendered
		PdfPage^ pdfPage = pdfDocument->GetPage(0);

		if (pdfPage != nullptr)
		{
			Windows::Foundation::Size actualPageSize = pdfPage->Size;
			if ((pageSizeLocal.Width == 0.0f) && (pageSizeLocal.Height == 0.0f))
			{
				pageSize.Width = actualPageSize.Width;
				pageSize.Height = actualPageSize.Height;
			}
			else
			{
				// If Size is non zero determine width & height of pages maintaining aspect ratio of each page
				float scale = min(static_cast<float>(pageSizeLocal.Width) / pdfPage->Size.Width, static_cast<float>(pageSizeLocal.Height) / pdfPage->Size.Height);
				pageSize.Width = actualPageSize.Width * scale;
				pageSize.Height = actualPageSize.Height * scale;
			}
		}
	}

	void PdfDataSource::CreatePlaceholderItems(_In_ unsigned int count)
	{
		for (unsigned int index = 0; index < count; index++)
		{
			if (this->Size < pageCount)
			{
				Platform::Object^ item = ref new PdfData(pageSize.Width, pageSize.Height);
				WaitForSingleObjectEx(vectorUpdateSemaphore, INFINITE, false);
				Append(item);
				ReleaseSemaphore(vectorUpdateSemaphore, 1, NULL);
			}
		}
	}

	/// <summary>
	/// Following function loads pages and updates the datasource with each item
	/// </summary>
	/// <param name="index">Index of page to be loaded</param>
	/// <param name="PDFDocument">PDF Document handle which is used for extracting page at given index</param>
	Platform::Object^ PdfDataSource::LoadPages(_In_ int index, _In_ PdfDocument^ pdfDocument)
	{
		Windows::Foundation::Size pageSizeLocal;

		pageSizeLocal.Width = pageSize.Width * zoomFactor;
		pageSizeLocal.Height = pageSize.Height * zoomFactor;

		ImageSource^ imageSource = ref new ImageSource(pageSizeLocal, surfaceType, this);
		imageSource->SetPageIndex(index);
		auto pageData = ref new PdfData();
		pageData->SetImageSource(imageSource);
		pageData->Width = pageSize.Width;
		pageData->Height = pageSize.Height;

		return pageData;
	}

	/// <summary>
	/// Dispatcher function to run task on UI thread
	/// All UI operations like creating surfaces (SIS/VSIS) and rendering can be invoked only on UI thread
	/// </summary>
	/// <param name="priority">Task priority</param>
	/// <param name="action">Lamda which defines set of operations to be executed</param>
	Windows::Foundation::IAsyncAction^ PdfDataSource::run_async_non_interactive(_In_ Windows::UI::Core::CoreDispatcherPriority priority, _In_ std::function<void ()> && action)
	{
		return dispatcher->RunAsync(
			priority,
			ref new Windows::UI::Core::DispatchedHandler([action]()
		{
			action();
		}));
	}

	/// <summary>
	/// Returns current zoom factor
	/// </summary>
	float PdfDataSource::GetZoomFactor()
	{
		return zoomFactor;
	}

	/// <summary>
	/// Returned page object of page at the given index from the PDF document
	/// </summary>
	PdfPage^ PdfDataSource::GetPage(_In_ unsigned int index)
	{
		if (index < pageCount)
		{
			return pdfDocument->GetPage(index);
		}
		else
		{
			return nullptr;
		}
	}

	/// <summary>
	/// Call this method when the app suspends to hint to the driver that the app is entering an idle state
	/// and that its memory can be used temporarily for other apps.
	/// </summary>
	void PdfDataSource::LoadItems(_In_ unsigned int count)
	{
		CreatePlaceholderItems(count);

		unsigned int requestCount = count / pagesToLoad;
		for (unsigned int i = 0; i < requestCount; i++)
		{
			LoadMoreItemsAsync(pagesToLoad);
		}
	}

	/// <summary>
	/// Call this method when the app suspends to hint to the driver that the app is entering an idle state
	/// and that its memory can be used temporarily for other apps.
	/// </summary>
	void PdfDataSource::Trim()
	{
		renderer->Trim();
	}

	/// <summary>
	/// Function to handle device lost
	/// </summary>
	void PdfDataSource::HandleDeviceLost()
	{
		// Rereate rendering resources
		// Creating new renderer which in turn creates all required reesources needed to render a page
		renderer = ref new Renderer(
			Window::Current->Bounds,
			DisplayInformation::GetForCurrentView()->LogicalDpi
			);

		// Setting current loaded pages to zero as all pages have to be re-rendered in case of device lost
		this->pagesCurrentlyLoaded = 0;

		// Clear storage so that LoadItemsAsyncOveride gets called to load each item again
		this->Clear();
	}

	/// <summary>
	/// Function to cancel any ongoing tasks before unloading current document
	/// </summary>
	void PdfDataSource::Unload()
	{
		// Cancel all tasks
		this->CancelTask();
		for (unsigned int index = 0, len = Size; index < len; index++)
		{
			PdfData^ pageData = dynamic_cast<PdfData^>(GetAt(index));
			if ((pageData != nullptr) && (!pageData->IsUnloaded()))
			{
				if (pageData->GetImageSource() != nullptr)
					pageData->GetImageSource()->Reset();
			}
		}
		
		// Clear storage
		this->Clear();
	}

	/// <summary>
	/// Function to update page based on current zoomFactor
	/// </summary>
	/// <param name="currentZoomFactor">Current zoom factor of scroll viewer</param>
	void PdfDataSource::UpdatePages(_In_ float currentZoomFactor)
	{
		if (zoomFactor != currentZoomFactor)
		{
			zoomFactor = currentZoomFactor;

			Windows::Foundation::Size pageSizeLocal;

			// Updating size of rendered items based on current zoom factor
			pageSizeLocal.Height = pageSize.Height * zoomFactor;
			pageSizeLocal.Width = pageSize.Width * zoomFactor;

			for (unsigned int index = 0, len = Size; index < len; index++)
			{

				switch (surfaceType)
				{
				case SurfaceType::SurfaceImageSource:
					break;
				case SurfaceType::VirtualSurfaceImageSource:
					PdfData^ pageData = dynamic_cast<PdfData^>(GetAt(index));
					if ((pageData != nullptr) && (!pageData->IsUnloaded()))
					{
						ImageSource^ currentImageSource = pageData->GetImageSource();

						// Recreating rendering resources with new page dimensions
						currentImageSource->SwapVsis(pageSizeLocal.Width, pageSizeLocal.Height);

						pageData->SetImageSource(currentImageSource);

						run_async_non_interactive(Windows::UI::Core::CoreDispatcherPriority::Low, [this, index, pageData](void)
						{
							this->SetAt(index, pageData);
						});
					}
					break;
				}
			}
		}
	}
};


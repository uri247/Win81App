// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "MainPage.xaml.h"

using namespace concurrency; 
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::DataTransfer;
using namespace Windows::Data::Pdf;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::System::Threading;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

using namespace PdfShowcase;

MainPage::MainPage()
{
	InitializeComponent();

	// Registering share handler
	DataTransferManager^ dataTransferManager = DataTransferManager::GetForCurrentView();
	auto dataRequestedToken = dataTransferManager->DataRequested += ref new TypedEventHandler<DataTransferManager^,
		DataRequestedEventArgs^>(this, &MainPage::EventHandlerShareItems);
	
	// Load and render PDF file from the APPX Assets
	LoadDefaultFile();
}

/// <summary>
/// This function loads PDF file from the assets
/// </summary>
void MainPage::LoadDefaultFile()
{
	// Getting installaed location of this app
	StorageFolder^ installedLocation = Package::Current->InstalledLocation;

	// Creating task to get the sample file from the assets folder
	create_task(installedLocation->GetFileAsync("Assets\\Sample.pdf")).then([this](StorageFile^ pdfFile)
	{
		loadedFile = pdfFile;
		MainPage::LoadPDF(pdfFile);

	});
}

/// <summary>
/// Function to load the PDF file selected by the user
/// </summary>
/// <param name="pdfFile">StorageFile object of the selected PDF file</param>
void MainPage::LoadPDF(_In_ StorageFile^ pdfFile)
{
	// Event for handling file load complete
	auto fileLoadedEvent = CreateEventEx(NULL, NULL, CREATE_EVENT_MANUAL_RESET, DELETE | SYNCHRONIZE | EVENT_MODIFY_STATE);
	// Creating task to load the PDF file and render pages in zoomed-in and zoomed-out view
	// For password protected documents one needs to call the function as is, handle the exception 
	// returned from LoadFromFileAsync and then call it again by providing the appropriate document 
	// password.
	create_task([this, pdfFile, fileLoadedEvent]()
	{
		auto loadFileTask = create_task([this, pdfFile, fileLoadedEvent](){return PdfDocument::LoadFromFileAsync(pdfFile); });

		loadFileTask.then([this, fileLoadedEvent](task<PdfDocument^> loadedDocTask) {
			try
			{
				pdfDocument = loadedDocTask.get();
				// Setting the file load complete event to trigger the loading 
				SetEvent(fileLoadedEvent);
			}
			catch (Platform::COMException^ e)
			{
				// Password protected file, a password should be provided to open such document
				SetEvent(fileLoadedEvent);
			}
		});
	});
	
	WaitForSingleObjectEx(fileLoadedEvent, INFINITE, FALSE);

	if (pdfDocument != nullptr)
	{
		InitializeZoomedInView();

		InitializeZoomedOutView();
	}

	CloseHandle(fileLoadedEvent);
}

/// <summary>
/// Function to initialize ZoomedInView of Semantic Zoom control
/// </summary>
void MainPage::InitializeZoomedInView()
{
	// If a file is already loaded, unloading it
	if (nullptr != pdfPageDataSourceZoomedInView)
	{
		pdfPageDataSourceZoomedInView->Unload();
	}
	// Page Size is set to zero for items in main view so that pages of original size are rendered
	Windows::Foundation::Size pageSize;

	pageSize.Width = Window::Current->Bounds.Width;
	pageSize.Height = Window::Current->Bounds.Height;

	// Main view items are rendered on a VSIS surface as they can be resized (optical zoom)
	zoomedInView = ref new ListView();
	zoomedInView->Style = zoomedInViewStyle;
	zoomedInView->ItemTemplate = zoomedInViewItemTemplate;
	zoomedInView->ItemsPanel = zoomedInViewItemsPanelTemplate;
	zoomedInView->Template = zoomedInViewControlTemplate;
	pdfPageDataSourceZoomedInView = ref new PdfDataSource(pdfDocument, pageSize, 5, SurfaceType::VirtualSurfaceImageSource);
	zoomedInView->ItemsSource = pdfPageDataSourceZoomedInView;

	semanticZoom->ZoomedInView = zoomedInView;
}

/// <summary>
/// Function to initialize ZoomedOutView of Semantic Zoom control
/// </summary>
void MainPage::InitializeZoomedOutView()
{
	// If a file is already loaded, unloading it
	if (nullptr != pdfPageDataSourceZoomedOutView)
	{
		pdfPageDataSourceZoomedOutView->Unload();
	}

	// Page Size is set to zero for items in main view so that pages of original size are rendered
	Windows::Foundation::Size pageSize;

	// Page size for thumbnail view is set to 300px as this gives good view of the thumbnails on all resolutions
	pageSize.Width = (float)safe_cast<double>(this->Resources->Lookup("thumbnailWidth"));
	pageSize.Height = (float)safe_cast<double>(this->Resources->Lookup("thumbnailHeight"));

	// Thumbnail view items are rendered on a SIS surface as they are of fixed size
	pdfPageDataSourceZoomedOutView = ref new PdfDataSource(pdfDocument, pageSize, 5, SurfaceType::SurfaceImageSource);

	zoomedOutView = ref new GridView();
	zoomedOutView->Style = zoomedOutViewStyle;
	zoomedOutView->ItemTemplate = zoomedOutViewItemTemplate;
	zoomedOutView->ItemsPanel = zoomedOutViewItemsPanelTemplate;
	zoomedOutView->ItemContainerStyle = zoomedOutViewItemContainerStyle;
	zoomedOutView->ItemsSource = pdfPageDataSourceZoomedOutView;
	semanticZoom->ZoomedOutView = zoomedOutView;
}

/// <summary>
/// Event Handler for handling application suspension
/// </summary>
void MainPage::OnSuspending(_In_ Object^ /*sender*/, _In_ SuspendingEventArgs^ /*args*/)
{
	// Hint to the driver that the app is entering an idle state and that its memory
	// can be temporarily used for other apps.
	pdfPageDataSourceZoomedInView->Trim();			

	pdfPageDataSourceZoomedOutView->Trim();			
}

/// <summary>
/// Open File click handler for command bar
/// This function loads the PDF file selecetd by the user
/// </summary>
void MainPage::OnOpenFileClick(_In_ Object^ /*sender*/, _In_ RoutedEventArgs^ /*e*/)
{
	// Launching FilePicker
	FileOpenPicker^ openPicker = ref new FileOpenPicker();
	openPicker->SuggestedStartLocation = PickerLocationId::DocumentsLibrary;
	openPicker->ViewMode = PickerViewMode::List;
	//openPicker->FileTypeFilter->Clear();
	openPicker->FileTypeFilter->Append(L".pdf");

	// Creating sync task for PickSingleFileAsync
	create_task(openPicker->PickSingleFileAsync()).then([this](StorageFile^ pdfFile) {
		if (pdfFile)
		{
			// Validating if selected file is not the same as file currently loaded
			if (loadedFile->Path != pdfFile->Path)
			{
				loadedFile = pdfFile;
				LoadPDF(pdfFile);
			}
		}
	});
}

/// <summary>
/// Event Handler for ViewChanged event of ScrollViewer for zoomedout view
/// This method is invoked to recreate VSIS surface of new width/height and re-render the page image at high resolution
/// </summary>
/// <param name="sender">Scroll Viewer</param>
/// <param name="e">ScrollViewerViewChangedEventArgs</param>
void MainPage::EventHandlerViewChanged(_In_ Object^ sender, _In_ ScrollViewerViewChangedEventArgs^ e)
{
	if (!e->IsIntermediate)
	{
		ScrollViewer^ scrollViewer;
		scrollViewer = safe_cast<ScrollViewer^>(sender);

		if (scrollViewer->ZoomFactor >= 1)
		{
			// Reloading pages at new zoomFactor
			pdfPageDataSourceZoomedInView->UpdatePages(scrollViewer->ZoomFactor);
		}
	}
}

/// <summary>
/// Event handler for ViewChangeStarted event for SemanticZoom
/// </summary>
/// <param name="e">SemanticZoomViewChangedEventArgs</param>
void MainPage::EventHandlerViewChangeStarted(_In_ Object^ /*sender*/, _In_ SemanticZoomViewChangedEventArgs^ e)
{
	auto sourceItem = e->SourceItem->Item;
	if (sourceItem != nullptr)
	{
		auto pageIndex = dynamic_cast<PdfData^>(sourceItem)->PageIndex;

		// Transitioning from Zooomed Out View to Zoomed In View
		if (semanticZoom->IsZoomedInViewActive)
		{
			if (pdfPageDataSourceZoomedInView->Size > pageIndex)
			{
				// Getting destination item from Zoomed-In-View
				auto destinationItem = pdfPageDataSourceZoomedInView->GetAt(pageIndex);

				if (destinationItem != nullptr)
				{
					e->DestinationItem->Item = destinationItem;
				}
			}
			else
			{
				auto size = pdfPageDataSourceZoomedInView->Size;
				unsigned int count = pageIndex + 1 - pdfPageDataSourceZoomedInView->Size;
				
				pdfPageDataSourceZoomedInView->LoadItems(count);
				auto destinationItem = pdfPageDataSourceZoomedInView->GetAt(pageIndex);

				if (destinationItem != nullptr)
				{
					e->DestinationItem->Item = destinationItem;
				}
			}
		}
		// Transitioning from Zooomed In View to Zoomed Out View
		else
		{
			if (pdfPageDataSourceZoomedOutView->Size > pageIndex)
			{
				// Getting destination item from Zoomed-In-View
				auto destinationItem = pdfPageDataSourceZoomedOutView->GetAt(pageIndex);

				if (destinationItem != nullptr)
				{
					e->DestinationItem->Item = destinationItem;
				}
			}
			else
			{
				unsigned int count = pageIndex + 1 - pdfPageDataSourceZoomedOutView->Size;

				pdfPageDataSourceZoomedOutView->LoadItems(count);
				auto destinationItem = pdfPageDataSourceZoomedOutView->GetAt(pageIndex);

				if (destinationItem != nullptr)
				{
					e->DestinationItem->Item = destinationItem;
				}
			}
		}
	}
}

/// <summary>
/// Function to handle share contract
/// </summary>
/// <param name="e">DataRequestedEventArgs</param>
void MainPage::EventHandlerShareItems(_In_ DataTransferManager^ /*sender*/, _In_ DataRequestedEventArgs^ e)
{
	DataRequest^ request = e->Request;
	request->Data->Properties->Title = "Sharing File";
	request->Data->Properties->Description = loadedFile->Name;
	
	try
	{
		auto storageItems = ref new Vector<IStorageItem^>();
		storageItems->Append(loadedFile);
		request->Data->SetStorageItems(storageItems);
	}
	catch (Exception^ ex)
	{
		request->FailWithDisplayText(ex->Message);
	}
}
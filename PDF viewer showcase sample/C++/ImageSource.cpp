// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ImageSource.h"

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml::Media::Imaging;

namespace PdfShowcase
{
	// This class is responsible for creating rendering surface for zoomed-in and zoomed-out view.
	// Thumbnails are rendered on a SIS surface as they don't need to be resized, VSIS surface is used for main view which gets resized when user zooms-in (optical zoom)
	ImageSource::ImageSource(_In_ Size pageSize, _In_ SurfaceType surfaceType, _In_ PdfDataSource^ pdfDataSource)
		:surfaceType(surfaceType), pdfDataSource(pdfDataSource)
	{
		// Creating required resources (SIS/VSIS surfaces)
		CreateResources(pageSize.Width, pageSize.Height);

		return;
	}

	/// <summary>
	/// Creates required resources SIS/VSIS
	/// </summary>
	void ImageSource::CreateResources(_In_ float widthLocal, _In_ float heightLocal)
	{
		width = widthLocal * DisplayInformation::GetForCurrentView()->LogicalDpi / 100;
		height = heightLocal * DisplayInformation::GetForCurrentView()->LogicalDpi / 100;
		ComPtr<IDXGIDevice> dxgiDevice = NULL;
		IInspectable* inspectable = NULL;
		HRESULT hr = S_OK;
		switch (surfaceType)
		{
		// Creating required resources for SIS surface
		case SurfaceType::SurfaceImageSource:
			sis = ref new SurfaceImageSource(static_cast<int>(width) , static_cast<int>(height) , true);
			inspectable = reinterpret_cast<IInspectable*>(sis) ;
			inspectable->QueryInterface(IID_PPV_ARGS(&sisNative));
			pdfDataSource->renderer->GetDXGIDevice(&dxgiDevice);
			sisNative->SetDevice(dxgiDevice.Get());
			break;
		// Creating required resources for VSIS surface
		case SurfaceType::VirtualSurfaceImageSource:
			vsisForeground = ref new VirtualSurfaceImageSource(static_cast<int>(width) , static_cast<int>(height) , false);
			inspectable = reinterpret_cast<IInspectable*>(vsisForeground) ;
			inspectable->QueryInterface(IID_PPV_ARGS(&vsisNative));
			if (vsisNative != nullptr)
			{

				pdfDataSource->renderer->GetDXGIDevice(&dxgiDevice);

				if (dxgiDevice != nullptr)
				{
					vsisNative->SetDevice(dxgiDevice.Get());

					Platform::WeakReference that(this);
					ComPtr<VSISCallBack> spCallBack = Make<VSISCallBack>(that) ;
					vsisNative->RegisterForUpdatesNeeded(spCallBack.Get());
				}
			}
			break;
		}
		return;
	}

	/// <summary>
	/// Destructor for ImageSource class to release the resources
	/// </summary>
	ImageSource::~ImageSource()
	{
		vsisNative.Reset();
		sisNative.Reset();
	}

	/// <summary>
	/// Following function resets the surface(SIS/VSIS)
	/// </summary>
	void ImageSource::Reset()
	{
		switch (surfaceType)
		{
		case SurfaceType::SurfaceImageSource:
			sis = nullptr;
			break;
		case SurfaceType::VirtualSurfaceImageSource:
			vsisBackground = nullptr;
			vsisForeground = nullptr;
			break;
		}
	}

	/// <summary>
	/// Following function creates a new VSIS resource and bind it to foreground image source.
	/// Existing VSIS surface is bound to background image source. 
	/// Page at current zoom level is rendered on the Foreground image source giving a crisper image
	/// </summary>
	/// <param name="widthLocal">Width of the scaled VSIS surface</param>
	/// <param name="heightLocal">Height of the scaled VSIS surface</param>
	void ImageSource::SwapVsis(_In_ float widthLocal, _In_ float heightLocal)
	{
		vsisNative.Reset();
		vsisNative.Detach();
		ComPtr<IDXGIDevice> dxgiDevice = NULL;
		IInspectable* inspectable = NULL;

		// Updating height and width with the new value
		width = widthLocal * DisplayInformation::GetForCurrentView()->LogicalDpi / 100;
		height = heightLocal * DisplayInformation::GetForCurrentView()->LogicalDpi / 100;

		// Assigning current VSIS surface to the background VSIS which is binded to a image source
		vsisBackground = vsisForeground;

		// Creating new VSIS with new dimensions
		vsisForeground = ref new VirtualSurfaceImageSource(static_cast<int>(width) , static_cast<int>(height) , false);
		inspectable = reinterpret_cast<IInspectable*>(vsisForeground) ;
		inspectable->QueryInterface(IID_PPV_ARGS(&vsisNative));

		if (vsisNative != nullptr)
		{

			pdfDataSource->renderer->GetDXGIDevice(&dxgiDevice);
			if (dxgiDevice != nullptr)
			{
				vsisNative->SetDevice(dxgiDevice.Get());

				Platform::WeakReference that(this);
				ComPtr<VSISCallBack> spCallBack = Make<VSISCallBack>(that) ;
				vsisNative->RegisterForUpdatesNeeded(spCallBack.Get());
			}
		}
	}

	/// <summary>
	/// Following function initializes page Index to which this ImageSource is bound
	/// Contents of this page is rendered on the surface bound to this image source
	/// </summary>
	/// <param name="pageIndexLocal">Page index</param>
	void ImageSource::SetPageIndex(_In_ unsigned int pageIndexLocal)
	{
		pageIndex = pageIndexLocal;
	}

	/// <summary>
	/// Following function returns the page index bound to this ImageSource instance
	/// </summary>
	/// <returns>Page Index</returns>
	unsigned int ImageSource::GetPageIndex()
	{
		return pageIndex;
	}

	/// <summary>
	/// Following function returns the current zoom factor
	/// </summary>
	/// <returns>Current Zoom Factor</returns>
	float ImageSource::GetZoomFactor()
	{
		return pdfDataSource->GetZoomFactor();
	}

	/// <summary>
	/// Following function returns the background VSIS surface
	/// </summary>
	/// <returns>Background VSIS surface</returns>
	VirtualSurfaceImageSource^ ImageSource::GetImageSourceVsisBackground()
	{
		return vsisBackground;
	}

	/// <summary>
	/// Following function set the VSIS surface
	/// </summary>
	/// <param name="vsisBackground">VSIS Surface</param>
	void ImageSource::SetImageSourceVsisBackground(_In_ VirtualSurfaceImageSource^ vsisBackgroundLocal)
	{
		vsisBackground = vsisBackgroundLocal;
	}

	/// <summary>
	/// Following function returns the foreground VSIS surface
	/// </summary>
	/// <returns>Foreground VSIS surface</returns>
	VirtualSurfaceImageSource^ ImageSource::GetImageSourceVsisForeground()
	{
		return vsisForeground;
	}

	/// <summary>
	/// Following function set the VSIS surface
	/// </summary>
	/// <param name="vsisForeground">VSIS Surface</param>
	void ImageSource::SetImageSourceVsisForeground(_In_ VirtualSurfaceImageSource^ vsisForegroundLocal)
	{
		vsisForeground = vsisForegroundLocal;
	}

	/// <summary>
	/// Following function returns the SIS surface
	/// </summary>
	/// <returns>SIS surface</returns>
	SurfaceImageSource^ ImageSource::GetImageSourceSis()
	{
		return sis;
	}

	/// <summary>
	/// Following function set the SIS surface
	/// </summary>
	/// <param name="sis">SIS Surface</param>
	void ImageSource::SetImageSourceSis(_In_ SurfaceImageSource^ sisLocal)
	{
		sis = sisLocal;
	}

	/// <summary>
	/// Following functions triggers rendering of page on the binded surface (SIS)
	/// No handling is needed for VSIS as it gets updated using the callback method already registered when this surface was created
	/// </summary>
	void ImageSource::RenderPage()
	{
		RECT updateRectangleNative = { 0, 0, static_cast<LONG>(width) , static_cast<LONG>(height) };

		switch (surfaceType)
		{
		case SurfaceType::SurfaceImageSource:
			RenderPageRectSis(updateRectangleNative);
			break;
		case SurfaceType::VirtualSurfaceImageSource:
			break;
		}
	}

	/// <summary>
	/// This function renders PDF page content on a SIS surface using pdfNativeRenderer
	/// </summary>
	/// <param name="rectangle">Page Rect to be rendered</param>
	void ImageSource::RenderPageRectSis(_In_ RECT rectangle)
	{
		SurfaceData sisData;
		if (sisNative != nullptr)
		{
			// Begin Draw
			HRESULT hr = sisNative->BeginDraw(rectangle, &(sisData.dxgiSurface), &(sisData.offset));

			if (hr == DXGI_ERROR_DEVICE_REMOVED || hr == DXGI_ERROR_DEVICE_RESET)
			{
				// Handle device lost
				pdfDataSource->HandleDeviceLost();
			}
			else
			{
				// Draw to IDXGISurface (the surface paramater)

				ComPtr<IPdfRendererNative> pdfRendererNative;
				pdfDataSource->renderer->GetPdfNativeRenderer(&pdfRendererNative);

				pdfPage = pdfDataSource->GetPage(pageIndex);
				unsigned int i = pdfPage->Index;
				Size pageSize = pdfPage->Size;
				float scale = min(static_cast<float>(width) / pageSize.Width, static_cast<float>(height) / pageSize.Height);
				IUnknown* pdfPageUnknown = reinterpret_cast<IUnknown*>(pdfPage) ;

				PDF_RENDER_PARAMS params;

				params.SourceRect = D2D1::RectF((rectangle.left / scale),
					(rectangle.top / scale),
					(rectangle.right / scale),
					(rectangle.bottom / scale));
				params.DestinationHeight = rectangle.bottom - rectangle.top;
				params.DestinationWidth = rectangle.right - rectangle.left;

				params.BackgroundColor = D2D1::ColorF(D2D1::ColorF::White);

				// When this flag is set to FALSE high contrast mode will be honored by PDF API's
				params.IgnoreHighContrast = FALSE;

				// Call PDF API RenderPageToSurface to render the content of page on SIS surface 
				hr = pdfRendererNative->RenderPageToSurface(pdfPageUnknown, sisData.dxgiSurface.Get(), sisData.offset, &params);

				// Releasing page
				delete pdfPage;
				// End Draw
				hr = sisNative->EndDraw();
			}
		}
	}

	/// <summary>
	/// Following function is a Callback registered for VSIS surface
	/// This method is invoked by the UI thread whenever corresponding page is in view
	/// </summary>
	void ImageSource::UpdatesNeeded()
	{
		pdfPage = pdfDataSource->GetPage(pageIndex);

		if ((pdfPage != nullptr) && (vsisNative != nullptr))
		{
			ULONG drawingBoundsCount = 0;

			vsisNative->GetUpdateRectCount(&drawingBoundsCount);

			std::unique_ptr<RECT []> drawingBounds(new RECT[drawingBoundsCount]);
			vsisNative->GetUpdateRects(drawingBounds.get(), drawingBoundsCount);

			for (ULONG i = 0; i < drawingBoundsCount; ++i)
			{
				RenderPageRectVsis(drawingBounds[i]);
			}
		}

		delete pdfPage;
	}

	/// <summary>
	/// This function renders PDF page content on a VSIS surface using pdfNativeRenderer
	/// </summary>
	/// <param name="rectangle">Page Rect to be rendered</param>
	void ImageSource::RenderPageRectVsis(_In_ RECT rectangle)
	{
		SurfaceData vsisData;

		// Begin Draw
		HRESULT hr = vsisNative->BeginDraw(rectangle, &(vsisData.dxgiSurface), &(vsisData.offset));

		if (hr == DXGI_ERROR_DEVICE_REMOVED || hr == DXGI_ERROR_DEVICE_RESET)
		{
			// Handle device lost
			pdfDataSource->HandleDeviceLost();
		}
		else
		{
			if (pdfPage != nullptr)
			{
				ComPtr<IPdfRendererNative> pdfRendererNative;
				pdfDataSource->renderer->GetPdfNativeRenderer(&pdfRendererNative);

				Size pageSize = pdfPage->Size;
				float scale = min(static_cast<float>(width) / pageSize.Width, static_cast<float>(height) / pageSize.Height);

				IUnknown* pdfPageUnknown = reinterpret_cast<IUnknown*>(pdfPage) ;
				PDF_RENDER_PARAMS params;
				params.SourceRect = D2D1::RectF((rectangle.left / scale),
					(rectangle.top / scale),
					(rectangle.right / scale),
					(rectangle.bottom / scale));

				params.DestinationHeight = rectangle.bottom - rectangle.top;
				params.DestinationWidth = rectangle.right - rectangle.left;

				params.BackgroundColor = D2D1::ColorF(D2D1::ColorF::White);

				// When this flag is set to FALSE high contrast mode will be honored by PDF API's
				params.IgnoreHighContrast = FALSE;

				// Call PDF API RenderPageToSurface to render the content of page on VSIS surface 
				HRESULT hr = pdfRendererNative->RenderPageToSurface(pdfPageUnknown, vsisData.dxgiSurface.Get(), vsisData.offset, &params);
			}
			// End Draw
			HRESULT hr = vsisNative->EndDraw();
		}
	}
}
// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "pch.h"

namespace PdfShowcase
{
	// Enumeration for defined surface types used for rendering thumbnails and main view
	public enum class SurfaceType {SurfaceImageSource, VirtualSurfaceImageSource};

	ref class PdfDataSource;

	// ImageSource class
    ref class ImageSource sealed
    {
    public:
		ImageSource(_In_ Windows::Foundation::Size pageSize, _In_ SurfaceType surfaceType, _In_ PdfDataSource^ pdfDataSource);
        
		void SetDimensions(_In_ Windows::Foundation::Size pageSize);
        void CancelPageUpdates();
		
		unsigned int GetPageIndex();
		Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ GetImageSourceVsisBackground();
		Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ GetImageSourceVsisForeground();
		Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^ GetImageSourceSis();
		
		void SetPageIndex(_In_ unsigned int pageIndex);
		void SetImageSourceVsisBackground(_In_ Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ vsisBackground);
		void SetImageSourceVsisForeground(_In_ Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ vsisForeground);
		void SetImageSourceSis(_In_ Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^ imageSourceSis);

		float GetZoomFactor();
		void CreateResources(_In_ float width, _In_ float height);
		void UpdatesNeeded();
		void RenderPage();
		void Reset();
		void SwapVsis(_In_ float width, _In_ float height);

    internal:
		HRESULT Resize(_In_ float width, _In_ float height);

    private:
        ~ImageSource();
		void RenderPageRectVsis(_In_ RECT rectangle);
		void RenderPageRectSis(_In_ RECT rectangle);

    private:
        Windows::Data::Pdf::PdfPage^                                    pdfPage;
		unsigned int													pageIndex;
		float															width;
		float															height;
        SurfaceType                                                     surfaceType;
        Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^   vsisBackground;
		Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^   vsisForeground;
        Microsoft::WRL::ComPtr<IVirtualSurfaceImageSourceNative>        vsisNative;
        Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^          sis;
		Microsoft::WRL::ComPtr<ISurfaceImageSourceNative>		        sisNative;
		PdfDataSource^													pdfDataSource;
    };

    struct SurfaceData
    {
        Microsoft::WRL::ComPtr<IDXGISurface> dxgiSurface;
        POINT offset;
        bool fContinue;

		SurfaceData() { offset.x = offset.y = 0; fContinue = false; }
    };

    class VSISCallBack : public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::ClassicCom>,
                                                                IVirtualSurfaceUpdatesCallbackNative>
    {
    public:
		
		VSISCallBack(Platform::WeakReference imageSource) : imageSourceLocal(imageSource) { }
        
		/// <summary>
		/// Callback function for UpdatesNeeded for VSIS surface
		/// This function is invoked whenever item is in view
		/// </summary>
        HRESULT STDMETHODCALLTYPE UpdatesNeeded()
        {
			ImageSource^ imageSource = imageSourceLocal.Resolve<ImageSource>();
            if (imageSource != nullptr)
            {
                imageSource->UpdatesNeeded();
            }
            return S_OK;
        }
        
    private:
		Platform::WeakReference        imageSourceLocal;
    };
}
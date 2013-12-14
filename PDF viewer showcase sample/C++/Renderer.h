// Copyright (c) Microsoft Corporation. All rights reserved

//
// Renderer.h
// Declaration of the Renderer class
//
#pragma once

#include <initguid.h>
#include <wrl.h>
#include <d3d11_2.h>
#include <dxgi1_3.h>
#include <d2d1_2.h>
#include <dwrite_2.h>
#include <wincodec.h>
#include "d2d1helper.h"
#include "windows.data.pdf.interop.h"

namespace PdfShowcase
{
	delegate void DeviceLostEventHandler();

	// Helper class that initializes and maintains the DirectX API objects in one place.
	ref class Renderer
	{
	internal:
		Renderer(
			_In_ Windows::Foundation::Rect windowBounds,
			_In_ float dpi
			);

		event DeviceLostEventHandler^ DeviceLost;

		void HandleDeviceLost();

		void UpdateWindowSize();

		void SetDpi(_In_ float dpi);

		void Trim();

		void GetD2DDeviceContext(_Outptr_ ID2D1DeviceContext** d2dDeviceContext);

		void GetPdfNativeRenderer(_Outptr_ IPdfRendererNative** nativePdfRenderer);

		void GetDXGIDevice(_Outptr_ IDXGIDevice** dxgiDevice);

		void GetD2DDevice(_Outptr_ ID2D1Device** d2dDevice);

		inline float GetDisplayWidth()
		{
			return windowBounds.Width;
		}

		inline float GetDisplayHeight()
		{
			return windowBounds.Height;
		}

		inline D2D1_SIZE_F GetDisplaySize()
		{
			return D2D1::SizeF(GetDisplayWidth(), GetDisplayHeight());
		}

		inline bool IsOrientationLandscape()
		{
			return GetDisplayWidth() >= GetDisplayHeight();
		}


	protected private:
		void CreateDeviceIndependentResources();
		void CreateDeviceResources();

		// Pdf Native Renderer
		Microsoft::WRL::ComPtr<IPdfRendererNative>		pdfRenderer;

		// Declare Direct2D objects
		Microsoft::WRL::ComPtr<ID2D1Factory1>			d2dFactory;
		Microsoft::WRL::ComPtr<ID2D1Device>				d2dDevice;
		Microsoft::WRL::ComPtr<ID2D1DeviceContext>		d2dDeviceContext;

		// Direct3D objects
		Microsoft::WRL::ComPtr<IDXGIDevice>				dxgiDevice;

		// Direct3D feature level
		D3D_FEATURE_LEVEL								featureLevel;

		// Bounds of the current window
		Windows::Foundation::Rect						windowBounds;

		float dpi;
	};

	// Helper utilities to make DX APIs work with exceptions in the samples apps.
	namespace DX
	{
		inline void ThrowIfFailed(_In_ HRESULT hr)
		{
			if (FAILED(hr))
			{
				// Set a breakpoint on this line to catch DX API errors.
				throw Platform::Exception::CreateException(hr);
			}
		}
	}
}


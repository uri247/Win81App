// Copyright (c) Microsoft Corporation. All rights reserved

//
// PdfDataSource.h
// Declaration of the PdfDataSource class
//
#pragma once
#include "pch.h"

namespace PdfShowcase
{
	ref class MainPage;

	// This class implements PdfDataSource. 
	// It has methods to load the PDF pages incrementally
	ref class PdfDataSource : public IncrementalLoadingBase
	{
#pragma region State
	internal:
		unsigned int										pageCount;
		unsigned int										pagesToLoad;
		unsigned int										pagesCurrentlyLoaded;
		SurfaceType											surfaceType;
		Windows::Foundation::Size							pageSize;
		Windows::Data::Pdf::PdfDocument^					pdfDocument;
		float												zoomFactor;
		Windows::UI::Core::CoreDispatcher^					dispatcher;
		CRITICAL_SECTION									criticalSectionPageLoad;
		HANDLE												vectorUpdateSemaphore;

#pragma endregion

	internal:
		PdfDataSource(_In_ Windows::Data::Pdf::PdfDocument^  pdfDocumentLocal, _In_ Windows::Foundation::Size pageSize, _In_ unsigned int pagesToLoad, _In_ SurfaceType surfaceType);

		virtual Concurrency::task<Windows::UI::Xaml::Data::LoadMoreItemsResult> LoadMoreItemsOverrideAsync(_In_ Concurrency::cancellation_token c, _In_ unsigned int count) override;

		virtual bool HasMoreItemsOverride() override;

	private:

		void SetItemSize(_In_ Windows::Foundation::Size pageSizeLocal);

		void CreatePlaceholderItems(_In_ unsigned int count);

		Platform::Object^ LoadPages(_In_ int index, _In_ Windows::Data::Pdf::PdfDocument^ pdfDocument);

		Windows::Foundation::IAsyncAction^  run_async_non_interactive(_In_ Windows::UI::Core::CoreDispatcherPriority priority, _In_ std::function<void ()> && action);

	public:

		virtual PdfDataSource::~PdfDataSource();

		// Rendering is defined as a property as it is referred from ImageSource class
		property Renderer^ renderer;

		float GetZoomFactor();

		Windows::Data::Pdf::PdfPage^ GetPage(_In_ unsigned int index);

		void LoadItems(_In_ unsigned int count);

		void Trim();

		void HandleDeviceLost();

		void Unload();

		void UpdatePages(_In_ float currentZoomFactor);
	};
}

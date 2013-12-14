// Copyright (c) Microsoft Corporation. All rights reserved

//
// IncrementalLoadingBase.h
// Declaration of the IncrementalLoadingBase class
//

#pragma once

namespace PdfShowcase
{
	// Implementing the ISupportIncrementalLoading interfaces to load pages in a PDF file incrementally
	// It loads more data automatically when the user scrolls to the end of of a GridView/ListView
	ref class IncrementalLoadingBase: Windows::UI::Xaml::Interop::IBindableObservableVector,Windows::UI::Xaml::Data::ISupportIncrementalLoading
	{
#pragma region Windows::UI::Xaml::Data::ISupportIncrementalLoading

	internal:
		IncrementalLoadingBase();

		property Platform::Object^ default[int] { Platform::Object^ get(int); void set(int, Platform::Object^); }

#pragma region Overridable methods

		virtual Concurrency::task<Windows::UI::Xaml::Data::LoadMoreItemsResult> LoadMoreItemsAsync(_In_ Concurrency::cancellation_token c, _In_ unsigned int count);

		virtual Concurrency::task<Windows::UI::Xaml::Data::LoadMoreItemsResult> LoadMoreItemsOverrideAsync(_In_ Concurrency::cancellation_token c, _In_ unsigned int count);

		virtual bool HasMoreItemsOverride();

#pragma endregion

	public:

		virtual Windows::Foundation::IAsyncOperation<Windows::UI::Xaml::Data::LoadMoreItemsResult>^ LoadMoreItemsAsync(_In_ unsigned int count);

		virtual void CancelTask();

		property bool HasMoreItems { virtual bool get(); };

#pragma endregion

#pragma region IBindableObservableVector

		virtual event Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ VectorChanged
		{
			virtual Windows::Foundation::EventRegistrationToken add(_In_ Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ e);

			virtual void remove(_In_ Windows::Foundation::EventRegistrationToken t);

			virtual void raise(_In_ Windows::UI::Xaml::Interop::IBindableObservableVector^ vector, _In_ Platform::Object^ e);
		}

#pragma endregion

#pragma region Windows::UI::Xaml::Interop::IBindableIterator

		virtual Windows::UI::Xaml::Interop::IBindableIterator^ First();

#pragma endregion

#pragma region Windows::UI::Xaml::Interop::IBindableVector

		virtual void Append(_In_ Platform::Object^ value);

		virtual void Clear();

		virtual Platform::Object^ GetAt(_In_ unsigned int index);

		virtual Windows::UI::Xaml::Interop::IBindableVectorView^ GetView();

		virtual bool IndexOf(_In_ Platform::Object^ value, _In_ unsigned int* index);

		virtual void InsertAt(_In_ unsigned int index, _In_ Platform::Object^ value);

		virtual void RemoveAt(_In_ unsigned int index);

		virtual void RemoveAtEnd();

		virtual void SetAt(_In_ unsigned int index, _In_ Platform::Object^ value);

		virtual property unsigned int Size {unsigned int get();}

#pragma endregion

#pragma region State

	private:
		Platform::Collections::Vector<Platform::Object^>^						storage;
		bool																	isVectorChangedObserved;
		event Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^	privateVectorChanged;
		Concurrency::cancellation_token_source									cts;

		/// <summary>
		/// Event handler for store vector changed
		/// </summary>
		/// <param name="e">IVectorChangedEventArgs</param>
		void StorageVectorChanged(_In_ Windows::Foundation::Collections::IObservableVector<Platform::Object^>^ /*sender*/, _In_ Windows::Foundation::Collections::IVectorChangedEventArgs^ e);
#pragma endregion
	};
}
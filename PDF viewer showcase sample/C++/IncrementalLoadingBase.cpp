// Copyright (c) Microsoft Corporation. All rights reserved

//
// IncrementalLoadingBase.h
// Declaration of the IncrementalLoadingBase class
//

#pragma once
#include "pch.h"

using namespace Concurrency;
using namespace Windows::UI::Xaml::Data;
using namespace Platform;
using namespace Windows::Foundation;

namespace PdfShowcase
{
	Object^ IncrementalLoadingBase::default::get(_In_ int index)
	{
		return storage->GetAt(index);
	}

	void IncrementalLoadingBase::default::set(_In_ int index, _In_ Object^ value)
	{
		storage->SetAt(index, value);
	}

	/// <summary>
	/// This function loads PDF pages incrementally using async call
	/// </summary>
	/// <param name="c">Token which can be used for canceling tasks if needed</param>
	/// <param name="count">Number of items to be loaded</param>
	/// <returns>Number of items loaded</returns>
	task<LoadMoreItemsResult> IncrementalLoadingBase::LoadMoreItemsAsync(_In_ cancellation_token c, _In_ unsigned int count)
	{
		// Creating a concurrent task to invoke override method to load the pages
		return task<void>([this]() {})
			.then([this, c, count]() {
				return LoadMoreItemsOverrideAsync(c, count);
		}).then([](LoadMoreItemsResult result)->LoadMoreItemsResult {
				// Check for cancelation. 
				if (is_task_cancellation_requested())
				{
					// Cancel the current task.
					cancel_current_task();
				}
				else
				{
					return result;
				}
		}, task_continuation_context::use_current());
	}

	/// <summary>
	/// Override method for LoadMoreItems
	/// </summary>
	/// <param name="c">Token which can be used for canceling tasks if needed</param>
	/// <param name="count">Number of items to be loaded</param>
	/// <returns>Array of loaded items</returns>
	task<LoadMoreItemsResult> IncrementalLoadingBase::LoadMoreItemsOverrideAsync(_In_ cancellation_token c, _In_ unsigned int count)
	{
		return task<LoadMoreItemsResult>(
			[]()->LoadMoreItemsResult {
				LoadMoreItemsResult result;
				result.Count = 0;
				return result;
		});
	}

	/// <summary>
	/// Override method to determine if there are any more items to be loaded
	/// </summary>
	/// <returns>Returns true if there are more items to be loaded</returns>
	bool IncrementalLoadingBase::HasMoreItemsOverride()
	{
		return false;
	}

	/// <summary>
	/// IncrementalLoadingBase class constructor
	/// </summary>
	IncrementalLoadingBase::IncrementalLoadingBase()
	{
		// Initializing private members
		storage = ref new Platform::Collections::Vector<Object^>();
		storage->VectorChanged += ref new Windows::Foundation::Collections::VectorChangedEventHandler<Object^>(this, &IncrementalLoadingBase::StorageVectorChanged);
		isVectorChangedObserved = false;
	}

	/// <summary>
	/// Override for Load more items
	/// </summary>
	/// <param name="count">Number of items requested</param>
	/// <returns>Async task</returns>
	IAsyncOperation<LoadMoreItemsResult>^ IncrementalLoadingBase::LoadMoreItemsAsync(_In_ unsigned int count)
	{
		// Creating cancelation token which will be used for canceling tasks if required
		auto token = cts.get_token();

		// Creating async task for loading items
		return create_async([this, count](cancellation_token token) {
			return LoadMoreItemsAsync(token, count);
		});
	}

	/// <summary>
	/// Function to cancel asynchronous tasks
	/// This token is passed to all async tasks created
	/// </summary>
	void IncrementalLoadingBase::CancelTask()
	{
		cts.cancel();
	}

	/// <summary>
	/// This function returns true if there are more items to be loaded
	/// </summary>
	bool IncrementalLoadingBase::HasMoreItems::get() 
	{ 
		return HasMoreItemsOverride(); 
	}

	Windows::Foundation::EventRegistrationToken IncrementalLoadingBase::VectorChanged::add(_In_ Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ e)
	{
		isVectorChangedObserved = true;
		return privateVectorChanged += e;
	}

	void IncrementalLoadingBase::VectorChanged::remove(_In_ Windows::Foundation::EventRegistrationToken t)
	{
		privateVectorChanged -= t;
	}

	void IncrementalLoadingBase::VectorChanged::raise(_In_ Windows::UI::Xaml::Interop::IBindableObservableVector^ vector, _In_ Object^ e)
	{
		if (isVectorChangedObserved)
		{
			privateVectorChanged(vector, e);
		}
	}

	Windows::UI::Xaml::Interop::IBindableIterator^ IncrementalLoadingBase::First()
	{
		return dynamic_cast<Windows::UI::Xaml::Interop::IBindableIterator^>(storage->First());
	}

	void IncrementalLoadingBase::Append(_In_ Object^ value)
	{
		storage->Append(value);
	}

	void IncrementalLoadingBase::Clear()
	{
		isVectorChangedObserved = false;
		for (int i = 0, len = Size; i < len; i++)
		{
			storage->RemoveAtEnd();
		}
	}

	Object^ IncrementalLoadingBase::GetAt(_In_ unsigned int index)
	{
		return storage->GetAt(index);
	}

	Windows::UI::Xaml::Interop::IBindableVectorView^ IncrementalLoadingBase::GetView()
	{
		return safe_cast<Windows::UI::Xaml::Interop::IBindableVectorView^>(storage->GetView());
	}

	bool IncrementalLoadingBase::IndexOf(_In_ Object^ value, _In_ unsigned int* index)
	{
		return storage->IndexOf(value, index);
	}

	void IncrementalLoadingBase::InsertAt(_In_ unsigned int index, _In_ Object^ value)
	{
		storage->InsertAt(index, value);
	}

	void IncrementalLoadingBase::RemoveAt(_In_ unsigned int index)
	{
		storage->RemoveAt(index);
	}

	void IncrementalLoadingBase::RemoveAtEnd()
	{
		storage->RemoveAtEnd();
	}

	void IncrementalLoadingBase::SetAt(_In_ unsigned int index, _In_ Object^ value)
	{
		storage->SetAt(index, value);
	}

	unsigned int IncrementalLoadingBase::Size::get() 
	{ 
		return storage->Size; 
	}

	/// <summary>
	/// Event handler for store vector changed
	/// </summary>
	/// <param name="e">IVectorChangedEventArgs</param>
	void IncrementalLoadingBase::StorageVectorChanged(_In_ Windows::Foundation::Collections::IObservableVector<Object^>^ /*sender*/, _In_ Windows::Foundation::Collections::IVectorChangedEventArgs^ e)
	{
		if (isVectorChangedObserved)
		{
			VectorChanged(this, e);
		}
	}
}
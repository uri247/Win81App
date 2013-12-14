// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include "pch.h"

using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Data;

namespace PdfShowcase
{
	PdfData::PdfData()
	{
		isPropertyChangedObserved = false;
		unloaded = false;
		imageSource = nullptr;
	}

	PdfData::PdfData(_In_ float Width, _In_ float Height)
	{
		height = Height;
		width = Width;
		isPropertyChangedObserved = false;
		unloaded = false;
		imageSource = nullptr;
	}

	unsigned int PdfData::PageIndex::get()
	{
		if (imageSource != nullptr)
		{
			return imageSource->GetPageIndex();
		}
		else
		{
			return 0;
		}
	}

	void PdfData::PageIndex::set(_In_ unsigned int value)
	{
		if (imageSource != nullptr)
		{
			imageSource->SetPageIndex(value);
			PdfData::OnPropertyChanged("PageIndex");
		}
	}

	float PdfData::Width::get()
	{
		return width;
	}

	void PdfData::Width::set(_In_ float value)
	{
		width = value;
		PdfData::OnPropertyChanged("Width");
	}

	float PdfData::Height::get()
	{
		return height;
	}

	void PdfData::Height::set(_In_ float value)
	{
		height = value;
		PdfData::OnPropertyChanged("Height");
	}

	VirtualSurfaceImageSource^ PdfData::ImageSourceVsisBackground::get()
	{
		if (imageSource != nullptr)
		{
			return imageSource->GetImageSourceVsisBackground();
		}
		else
		{
			return nullptr;
		}
	}

	void PdfData::ImageSourceVsisBackground::set(_In_ VirtualSurfaceImageSource^ value)
	{
		if (imageSource != nullptr)
		{
			imageSource->SetImageSourceVsisBackground(value);
			PdfData::OnPropertyChanged("ImageSourceVsisBackground");
		}
	}
	
	VirtualSurfaceImageSource^ PdfData::ImageSourceVsisForeground::get()
	{
		if (imageSource != nullptr)
		{
			if (unloaded)
			{
				unloaded = false;
				auto currentZoomFactor = imageSource->GetZoomFactor();
				imageSource->CreateResources(width * currentZoomFactor, height * currentZoomFactor);
				return imageSource->GetImageSourceVsisForeground();
			}
			else
			{
				return imageSource->GetImageSourceVsisForeground();
			}
		}
		else
		{
			return nullptr;
		}
	}

	void PdfData::ImageSourceVsisForeground::set(_In_ VirtualSurfaceImageSource^ value)
	{
		imageSource->SetImageSourceVsisForeground(value);
		PdfData::OnPropertyChanged("ImageSourceVsisForeground");
	}

	SurfaceImageSource^ PdfData::ImageSourceSis::get()
	{
		if (imageSource != nullptr)
		{
			if (unloaded)
			{
				unloaded = false;
				imageSource->CreateResources(width, height);
			}
			imageSource->RenderPage();
			return imageSource->GetImageSourceSis();
		}
		else
		{
			return nullptr;
		}
	}

	void PdfData::ImageSourceSis::set(_In_ SurfaceImageSource^ value)
	{
		if (imageSource != nullptr)
		{
			imageSource->SetImageSourceSis(value);
			PdfData::OnPropertyChanged("ImageSourceSis");
		}
	}

	/// <summary>
	/// Notifies listeners that a property value has changed.
	/// </summary>
	/// <param name="propertyName">Name of the property used to notify listeners.</param>
	void PdfData::OnPropertyChanged(_In_ Platform::String^ propertyName)
	{
		if (isPropertyChangedObserved)
		{
			PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
		}
	}

	void PdfData::SetImageSource(_In_ ImageSource^ imageSourceLocal)
	{
		imageSource = imageSourceLocal;
		PdfData::OnPropertyChanged("ImageSourceVsisBackground");
		PdfData::OnPropertyChanged("ImageSourceVsisForeground");
	}

	ImageSource^ PdfData::GetImageSource()
	{
		return imageSource;
	}

	bool PdfData::IsUnloaded()
	{
		return unloaded;
	}

	// In c++, it is not neccessary to include definitions of add, remove, and raise.
	// These definitions have been made explicitly here so that we can check if the 
	// event has listeners before firing the event
	Windows::Foundation::EventRegistrationToken PdfData::PropertyChanged::add(_In_ PropertyChangedEventHandler^ e)
	{
		isPropertyChangedObserved = true;
		return privatePropertyChanged += e;
	}

	void PdfData::PropertyChanged::remove(_In_ Windows::Foundation::EventRegistrationToken t)
	{
		privatePropertyChanged -= t;
		if (!unloaded)
		{
			if (imageSource != nullptr)
			{
				imageSource->Reset();
			}
			unloaded = true;
		}
	}

	void PdfData::PropertyChanged::raise(_In_ Object^ sender, _In_ PropertyChangedEventArgs^ e)
	{
		if (isPropertyChangedObserved)
		{
			privatePropertyChanged(sender, e);
		}
	}
};

// Copyright (c) Microsoft Corporation. All rights reserved

//
// PdfData.h
// Declaration of the PdfData class
//
#pragma once

namespace PdfShowcase
{
	[Windows::Foundation::Metadata::WebHostHidden]
	[Windows::UI::Xaml::Data::Bindable] // in c++, adding this attribute to ref classes enables data binding for more info search for 'Bindable' on the page http://go.microsoft.com/fwlink/?LinkId=254639 
	public ref class PdfData sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
	{
	public:
		property unsigned int PageIndex  { unsigned int get(); void set(unsigned int); };
		property float Width  { float get(); void set(float); };
		property float Height { float get(); void set(float); };
		property Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^	ImageSourceVsisBackground { Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ get(); void set(Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^); };
		property Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^	ImageSourceVsisForeground { Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^ get(); void set(Windows::UI::Xaml::Media::Imaging::VirtualSurfaceImageSource^); };
		property Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^			ImageSourceSis			  { Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^ get(); void set(Windows::UI::Xaml::Media::Imaging::SurfaceImageSource^); }	;

	private:
		ImageSource^															imageSource;
		float																	width;
		float																	height;
		bool																	unloaded;
#pragma region INotifyPropertyChanged
	private:
		bool isPropertyChangedObserved;
		event Windows::UI::Xaml::Data::PropertyChangedEventHandler^				privatePropertyChanged;
	protected:
		void OnPropertyChanged(_In_ Platform::String^ propertyName);

	internal:
		virtual void SetImageSource(_In_ ImageSource^ imageSourceLocal);

		virtual ImageSource^ GetImageSource();

		virtual bool IsUnloaded();

	public:
		PdfData();

		PdfData(_In_ float Width, _In_ float Height);

		virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged
		{
			Windows::Foundation::EventRegistrationToken add(_In_ Windows::UI::Xaml::Data::PropertyChangedEventHandler^ e);
			void remove(_In_ Windows::Foundation::EventRegistrationToken t);
		protected:
			void raise(_In_ Object^ sender, _In_ Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e);
		}
#pragma endregion
	};
}

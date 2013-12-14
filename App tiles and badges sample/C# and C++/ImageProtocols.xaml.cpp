//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// ImageProtocols.xaml.cpp
// Implementation of the ImageProtocols class
//

#include "pch.h"
#include "ImageProtocols.xaml.h"

using namespace SDKSample::Tiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage;
using namespace concurrency;

ImageProtocols::ImageProtocols()
{
	InitializeComponent();
}

void ImageProtocols::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = MainPage::Current;
	LocalFolder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	HTTP->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	ProtocolList->SelectedIndex = 0;
	imageRelativePath = "copy a file first";
}

void ImageProtocols::ProtocolList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	LocalFolder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	HTTP->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

	if (ProtocolList->SelectedIndex == 1)
	{
		LocalFolder->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
	else if (ProtocolList->SelectedIndex == 2)
	{
		HTTP->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void ImageProtocols::SendTileNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	IWide310x150TileNotificationContent^ wide310x150TileContent;
	if (ProtocolList->SelectedIndex == 0) //using the ms-appx:/// protocol
	{
		auto wide310x150ImageAndTextContent = TileContentFactory::CreateTileWide310x150ImageAndText01();
		wide310x150ImageAndTextContent->TextCaptionWrap->Text = "The image is in the appx package";
		wide310x150ImageAndTextContent->Image->Src = "ms-appx:///redWide310x150.png";
		wide310x150ImageAndTextContent->Image->Alt = "Red image";

		wide310x150TileContent = wide310x150ImageAndTextContent;
	}
	else if (ProtocolList->SelectedIndex == 1) //using the ms-appdata:///local/ protocol
	{
		auto wide310x150ImageContent = TileContentFactory::CreateTileWide310x150Image();
		wide310x150ImageContent->Image->Src = imageRelativePath;
		wide310x150ImageContent->Image->Alt = "App data";
		wide310x150TileContent = wide310x150ImageContent;
	}
	else if (ProtocolList->SelectedIndex == 2) //using http:// protocol
	{
		// Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
		auto wide310x150PeekImageCollectionContent = TileContentFactory::CreateTileWide310x150PeekImageCollection04();
		wide310x150PeekImageCollectionContent->RequireSquare150x150Content = false;
		wide310x150PeekImageCollectionContent->BaseUri = HTTPBaseURI->Text;
		wide310x150PeekImageCollectionContent->TextBodyWrap->Text = "The base URI is " + HTTPBaseURI->Text;
		wide310x150PeekImageCollectionContent->ImageMain->Src = HTTPImage1->Text;
		wide310x150PeekImageCollectionContent->ImageSmallColumn1Row1->Src = HTTPImage2->Text;
		wide310x150PeekImageCollectionContent->ImageSmallColumn1Row2->Src = HTTPImage3->Text;
		wide310x150PeekImageCollectionContent->ImageSmallColumn2Row1->Src = HTTPImage4->Text;
		wide310x150PeekImageCollectionContent->ImageSmallColumn2Row2->Src = HTTPImage5->Text;

		wide310x150TileContent = wide310x150PeekImageCollectionContent;
	}

	if (wide310x150TileContent != nullptr)
	{
		wide310x150TileContent->RequireSquare150x150Content = false;
		TileUpdateManager::CreateTileUpdaterForApplication()->Update(wide310x150TileContent->CreateNotification());

		OutputTextBlock->Text = wide310x150TileContent->GetContent();
	}
}

void ImageProtocols::PickImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	auto openPicker = ref new FileOpenPicker();
	openPicker->ViewMode = PickerViewMode::Thumbnail;
	openPicker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
	openPicker->FileTypeFilter->Append(".png");
	openPicker->FileTypeFilter->Append(".gif");

	task<StorageFile^> pickImageTask(openPicker->PickSingleFileAsync());
	TextBlock^ outputBlock = OutputTextBlock;
	pickImageTask.then([this, outputBlock](StorageFile^ file)
	{
		if (file != nullptr)
		{
			// Get the returned file and copy it
			auto local = ApplicationData::Current->LocalFolder;
			task<StorageFile^> copyFileTask(file->CopyAsync(local, file->Name, NameCollisionOption::ReplaceExisting));
			imageRelativePath = file->Name;
			copyFileTask.then([outputBlock, file](task<StorageFile^> copiedFileTask)
			{
				StorageFile^ copiedFile;
				try
				{
					copiedFile = copiedFileTask.get();
				}
				catch (Platform::Exception^ e)
				{
					outputBlock->Text = "Error copying file";
				}

				if (copiedFile != nullptr)
				{
					outputBlock->Text = "File " + copiedFile->Name + " was copied from " + file->Name;
				}
			});
		}
		else
		{
			outputBlock->Text = "File selection cancelled by user.";
		}
	});
}
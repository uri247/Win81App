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
// ScenarioSingle.xaml.cpp
// Implementation of the ScenarioSingle class
//

#include "pch.h"
#include "ScenarioSingle.xaml.h"

using namespace ContactPicker;
using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;


ScenarioSingle::ScenarioSingle()
{
    InitializeComponent();
    PickAContactButton->Click += ref new RoutedEventHandler(this, &ScenarioSingle::PickAContactButton_Click);
}

void ScenarioSingle::PickAContactButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage^ page = MainPage::Current;

    auto contactPicker = ref new Contacts::ContactPicker();
    contactPicker->CommitButtonText = "Select";

    create_task(contactPicker->PickContactAsync()).then([this](Contacts::Contact^ contact)
    {
        if (contact != nullptr)
        {
            OutputFields->Visibility = Windows::UI::Xaml::Visibility::Visible;
            OutputEmpty->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

            OutputName->Text = contact->DisplayName;
            
            String^ output = "";
            // Append emails
            if (contact->Emails->Size > 0)
            {
                std::for_each(begin(contact->Emails), end(contact->Emails), [&output](Contacts::ContactEmail^ email)
                {
                    output += email->Address + "\n";
                });

                OutputEmailHeader->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputEmails->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputEmails->Text = output;
            }
            else
            {
                OutputEmailHeader->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                OutputEmails->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            }
           
            // Append phones
            output = "";
            if (contact->Phones->Size > 0)
            {
                std::for_each(begin(contact->Phones), end(contact->Phones), [&output](Contacts::ContactPhone^ phone)
                {
                    output += phone->Number + "\n";
                });

                OutputPhoneNumberHeader->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputPhoneNumbers->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputPhoneNumbers->Text = output;
            }
            else
            {
                OutputPhoneNumberHeader->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                OutputPhoneNumbers->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            }

            // Append addresses
            output = "";
            if (contact->Addresses->Size > 0)
            {
                std::for_each(begin(contact->Addresses), end(contact->Addresses), [&output](Contacts::ContactAddress^ address)
                {
                    output += address->StreetAddress + ", " + address->Locality + ", " + address->Region + ", " + address->PostalCode + " (" + address->Kind.ToString() + ")\n";
                });

                OutputAddressHeader->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputAddresses->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputAddresses->Text = output;
            }
            else
            {
                OutputAddressHeader->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                OutputAddresses->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            }

            if (contact->Thumbnail != nullptr)
            {
                create_task(contact->Thumbnail->OpenReadAsync()).then([this](IRandomAccessStreamWithContentType^ stream)
                {
                    if (stream != nullptr && stream->Size > 0)
                    {
                        BitmapImage^ bitmap = ref new BitmapImage();
                        bitmap->SetSource(stream);
                        OutputThumbnail->Source = bitmap;
                    }
                    else
                    {
                        OutputThumbnail->Source = nullptr;
                    }
                });
            }
        }
        else
        {
            OutputEmpty->Visibility = Windows::UI::Xaml::Visibility::Visible;
            OutputFields->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            OutputThumbnail->Source = nullptr;
        }
    });
}
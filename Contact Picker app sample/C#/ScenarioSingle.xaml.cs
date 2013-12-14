//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Text;
using SDKTemplate;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace ContactPicker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioSingle : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public ScenarioSingle()
        {
            this.InitializeComponent();
            PickAContactButton.Click += PickAContactButton_Click;
        }

        private async void PickAContactButton_Click(object sender, RoutedEventArgs e)
        {
            var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
            contactPicker.CommitButtonText = "Select";
            Contact contact = await contactPicker.PickContactAsync();

            if (contact != null)
            {
                OutputFields.Visibility = Visibility.Visible;
                OutputEmpty.Visibility = Visibility.Collapsed;

                OutputName.Text = contact.DisplayName;
                AppendContactFieldValues(OutputEmailHeader, OutputEmails, contact.Emails);
                AppendContactFieldValues(OutputPhoneNumberHeader, OutputPhoneNumbers, contact.Phones);
                AppendContactFieldValues(OutputAddressHeader, OutputAddresses, contact.Addresses);

                if( contact.Thumbnail != null)
                {
                    IRandomAccessStreamWithContentType stream = await contact.Thumbnail.OpenReadAsync();
                    if (stream != null && stream.Size > 0)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.SetSource(stream);
                        OutputThumbnail.Source = bitmap;
                    }
                    else
                    {
                        OutputThumbnail.Source = null;
                    }
                }
            }
            else
            {
                OutputEmpty.Visibility = Visibility.Visible;
                OutputFields.Visibility = Visibility.Collapsed;
                OutputThumbnail.Source = null;
            }
            
        }

        private void AppendContactFieldValues<T>(TextBlock header, TextBlock content, IList<T> fields)
        {
            if (fields.Count > 0)
            {
                StringBuilder output = new StringBuilder();
                if(fields[0].GetType() == typeof(ContactEmail))
                {
                    foreach (ContactEmail email in fields as IList<ContactEmail>)
                    {
                        output.AppendFormat("Email Address: {0} ({1})\n", email.Address, email.Kind);
                    }
                }
                else if (fields[0].GetType() == typeof(ContactPhone))
                {
                    foreach (ContactPhone phone in fields as IList<ContactPhone>)
                    {
                        output.AppendFormat("Phone: {0} ({1})\n", phone.Number, phone.Kind);
                    }
                }
                else if(fields[0].GetType() == typeof(ContactAddress))
                {
                    List<String> addressParts = null;
                    string unstructuredAddress = "";

                    foreach (ContactAddress address in fields as IList<ContactAddress>)
                    {
                        addressParts = (new List<string> { address.StreetAddress, address.Locality, address.Region, address.PostalCode });
                        unstructuredAddress = string.Join(", ", addressParts.FindAll(s => !string.IsNullOrEmpty(s)));
                        output.AppendFormat("Address: {0} ({1})\n", unstructuredAddress, address.Kind);
                    }
                }
                
                header.Visibility = Visibility.Visible;
                content.Visibility = Visibility.Visible;
                content.Text = output.ToString();
            }
            else
            {
                header.Visibility = Visibility.Collapsed;
                content.Visibility = Visibility.Collapsed;
            }
        }
    }
}

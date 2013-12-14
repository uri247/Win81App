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
using SDKTemplate;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace ContactPicker
{
    public sealed partial class ScenarioMultiple : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;
        public IList<Contact> contacts;

        public ScenarioMultiple()
        {
            this.InitializeComponent();
            PickContactsButton.Click += PickContactsButton_Click;
            OutputContacts.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void PickContactsButton_Click(object sender, RoutedEventArgs e)
        {
            var contactPicker = new Windows.ApplicationModel.Contacts.ContactPicker();
            contactPicker.CommitButtonText = "Select";
            contacts = await contactPicker.PickContactsAsync();

            OutputContacts.Items.Clear();

            if (contacts.Count > 0)
            {
                OutputContacts.Visibility = Windows.UI.Xaml.Visibility.Visible;
                OutputEmpty.Visibility = Visibility.Collapsed;

                foreach (Contact contact in contacts)
                {
                    OutputContacts.Items.Add(new ContactItemAdapter(contact));
                }
            }
            else
            {
                OutputEmpty.Visibility = Visibility.Visible;
            }         
        }
    }

    public class ContactItemAdapter
    {
        public string Name { get; private set; }
        public string SecondaryText { get; private set; }
        public BitmapImage Thumbnail { get; private set; }

        public ContactItemAdapter(Contact contact)
        {
            Name = contact.DisplayName;
            if (contact.Emails.Count > 0)
            {
                SecondaryText = contact.Emails[0].Address;
            }
            else if (contact.Phones.Count > 0)
            {
                SecondaryText = contact.Phones[0].Number;
            }
            else if (contact.Addresses.Count > 0)
            {
                List<string> addressParts = (new List<string> { contact.Addresses[0].StreetAddress, contact.Addresses[0].Locality, contact.Addresses[0].Region, contact.Addresses[0].PostalCode });
                string unstructuredAddress = string.Join(", ", addressParts.FindAll(s => !string.IsNullOrEmpty(s)));
                SecondaryText = unstructuredAddress;
            }
            GetThumbnail(contact);
        }

        private async void GetThumbnail(Contact contact)
        {
            if (contact.Thumbnail != null)
            {
                IRandomAccessStreamWithContentType stream = await contact.Thumbnail.OpenReadAsync();
                if (stream != null && stream.Size > 0)
                {
                    Thumbnail = new BitmapImage();
                    Thumbnail.SetSource(stream);
                }
            }
        }
    }
}

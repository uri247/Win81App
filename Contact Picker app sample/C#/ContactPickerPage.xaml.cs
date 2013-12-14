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
using Windows.ApplicationModel.Contacts.Provider;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ContactPicker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactPickerPage : SDKTemplate.Common.LayoutAwarePage
    {
        ContactPickerUI contactPickerUI = MainPagePicker.Current.contactPickerUI;
        CoreDispatcher dispatcher = Window.Current.Dispatcher;

        public ContactPickerPage()
        {
            this.InitializeComponent();
            ContactList.ItemsSource = contactSet;
            ContactList.SelectionChanged += ContactList_SelectionChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            contactPickerUI.ContactRemoved += contactPickerUI_ContactRemoved;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            contactPickerUI.ContactRemoved -= contactPickerUI_ContactRemoved;
        }

        async void contactPickerUI_ContactRemoved(ContactPickerUI sender, ContactRemovedEventArgs args)
        {
            // The event handler may be invoked on a background thread, so use the Dispatcher to run the UI-related code on the UI thread.
            string removedId = args.Id;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (SampleContact contact in ContactList.SelectedItems)
                {
                    if (contact.Id == removedId)
                    {
                        ContactList.SelectedItems.Remove(contact);
                        OutputText.Text += "\n" + contact.DisplayName + " was removed from the basket";
                        break;
                    }
                }
            });
        }

        void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (SampleContact added in e.AddedItems)
            {
                AddSampleContact(added);
            }

            foreach (SampleContact removed in e.RemovedItems)
            {
                if (contactPickerUI.ContainsContact(removed.Id))
                {
                    contactPickerUI.RemoveContact(removed.Id);
                    OutputText.Text = removed.DisplayName + " was removed from the basket";
                }
            }
        }

        void AddSampleContact(SampleContact sampleContact)
        {
            Contact contact = new Contact();
            contact.Id = sampleContact.Id;
            contact.FirstName = sampleContact.FirstName;
            contact.LastName = sampleContact.LastName;

            if (!string.IsNullOrEmpty(sampleContact.HomeEmail))
            {
                ContactEmail homeEmail = new ContactEmail();
                homeEmail.Address = sampleContact.HomeEmail;
                homeEmail.Kind = ContactEmailKind.Personal;
                contact.Emails.Add(homeEmail);
            }

            if (!string.IsNullOrEmpty(sampleContact.WorkEmail))
            {
                ContactEmail workEmail = new ContactEmail();
                workEmail.Address = sampleContact.WorkEmail;
                workEmail.Kind = ContactEmailKind.Work;
                contact.Emails.Add(workEmail);
            }

            if (!string.IsNullOrEmpty(sampleContact.HomePhone))
            {
                ContactPhone homePhone = new ContactPhone();
                homePhone.Number = sampleContact.HomePhone;
                homePhone.Kind = ContactPhoneKind.Home;
                contact.Phones.Add(homePhone);
            }

            if (!string.IsNullOrEmpty(sampleContact.MobilePhone))
            {
                ContactPhone mobilePhone = new ContactPhone();
                mobilePhone.Number = sampleContact.MobilePhone;
                mobilePhone.Kind = ContactPhoneKind.Mobile;
                contact.Phones.Add(mobilePhone);
            }

            if (!string.IsNullOrEmpty(sampleContact.WorkPhone))
            {
                ContactPhone workPhone = new ContactPhone();
                workPhone.Number = sampleContact.WorkPhone;
                workPhone.Kind = ContactPhoneKind.Work;
                contact.Phones.Add(workPhone);
            }

            if(!string.IsNullOrEmpty(sampleContact.Street))
            {
                ContactAddress homeAddress = new ContactAddress();
                homeAddress.StreetAddress = sampleContact.Street;
                homeAddress.Locality = sampleContact.City;
                homeAddress.Region = sampleContact.State;
                homeAddress.PostalCode = sampleContact.ZipCode;
                homeAddress.Kind = ContactAddressKind.Home;
                contact.Addresses.Add(homeAddress);
            }

            switch (contactPickerUI.AddContact(contact))
            {
                case AddContactResult.Added:
                    // Notify the user that the contact was added
                    OutputText.Text = contact.DisplayName + " was added to the basket";
                    break;
                case AddContactResult.AlreadyAdded:
                    // Notify the user that the contact is already added
                    OutputText.Text = contact.DisplayName + " is already in the basket";
                    break;
                case AddContactResult.Unavailable:
                default:
                    // Notify the user that the basket is unavailable
                    OutputText.Text = contact.DisplayName + " could not be added to the basket";
                    break;
            }
        }

        // Example contacts to pick from
        List<SampleContact> contactSet = new List<SampleContact>()
        {
            new SampleContact()
            {
                FirstName = "David",
                LastName = "Jaffe",
                HomeEmail = "david@contoso.com",
                WorkEmail = "david@cpandl.com",
                HomePhone = "248-555-0150",
                Street = "",
                City = "",
                State = "",
                ZipCode = ""
            },
            new SampleContact()
            {
                FirstName = "Kim",
                LastName = "Abercrombie",
                HomeEmail = "kim@contoso.com",
                WorkEmail = "kim@adatum.com",
                HomePhone = "444 555-0001",
                WorkPhone = "245 555-0123",
                MobilePhone = "921 555-0187",
                Street = "123 Main St",
                City = "Redmond",
                State = "WA",
                ZipCode = "23456"
            },
            new SampleContact()
            {
                FirstName = "Jeff",
                LastName = "Phillips",
                HomeEmail = "jeff@contoso.com",
                WorkEmail = "jeff@fabrikam.com",
                HomePhone = "987-555-0199",
                MobilePhone = "543-555-0111",
                Street = "456 2nd Ave",
                City = "Dallas",
                State = "TX",
                ZipCode = "12345"
            },
            new SampleContact()
            {
                FirstName = "Arlene",
                LastName = "Huff",
                HomeEmail = "arlene@contoso.com",
                MobilePhone = "234-555-0156"
            },
            new SampleContact()
            {
                FirstName = "Miles",
                LastName = "Reid",
                HomeEmail = "miles@contoso.com",
                WorkEmail = "miles@proseware.com",
                Street = "678 Elm St",
                City = "New York",
                State = "New York",
                ZipCode = "95111"
            }
        };

    }

    internal class SampleContact
    {
        public string Id { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomeEmail { get; set; }
        public string WorkEmail { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public SampleContact()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string DisplayName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }
    }

}

//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var contactPickerUI;

    var page = WinJS.UI.Pages.define("/html/contactPickerScenario.html", {
        processed: function (element, uri) {
            // During an initial activation this event is called before the system splash screen is torn down.
            sampleContacts.forEach(createContactUI);
        },

        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI setup.
            contactPickerUI = options.contactPickerUI;
            contactPickerUI.addEventListener("contactremoved", onContactRemoved, false);
        },

        unload: function () {
            contactPickerUI.removeEventListener("contactremoved", onContactRemoved, false);
        }
    });

    function addContactToBasket(sampleContact) {
        // Programmatically add the contact to the basket

        // Apply the sample data onto the contact object
        var contact = new Windows.ApplicationModel.Contacts.Contact();
        contact.firstName = sampleContact.firstName;
        contact.lastName = sampleContact.lastName;
        contact.id = sampleContact.id;

        // Adds a personal and  work email address to the contact emails vector
        if (sampleContact.personalEmail) {
            var personalEmail = new Windows.ApplicationModel.Contacts.ContactEmail();
            personalEmail.address = sampleContact.personalEmail;
            personalEmail.kind = Windows.ApplicationModel.Contacts.ContactEmailKind.personal;
            contact.emails.append(personalEmail);
        }

        if (sampleContact.workEmail) {
            var workEmail = new Windows.ApplicationModel.Contacts.ContactEmail();
            workEmail.address = sampleContact.workEmail;
            workEmail.kind = Windows.ApplicationModel.Contacts.ContactEmailKind.work;
            contact.emails.append(workEmail);
        }

        // Adds a home, work, and mobile phone number to the contact phones vector
        if (sampleContact.homePhone) {
            var homePhone = new Windows.ApplicationModel.Contacts.ContactPhone();
            homePhone.number = sampleContact.homePhone;
            homePhone.kind = Windows.ApplicationModel.Contacts.ContactPhoneKind.home;
            contact.phones.append(homePhone);
        }

        if (sampleContact.workPhone) {
            var workPhone = new Windows.ApplicationModel.Contacts.ContactPhone();
            workPhone.number = sampleContact.workPhone;
            workPhone.kind = Windows.ApplicationModel.Contacts.ContactPhoneKind.work;
            contact.phones.append(workPhone);
        }

        if (sampleContact.mobilePhone) {
            var mobilePhone = new Windows.ApplicationModel.Contacts.ContactPhone();
            mobilePhone.number = sampleContact.mobilePhone;
            mobilePhone.kind = Windows.ApplicationModel.Contacts.ContactPhoneKind.mobile;
            contact.phones.append(mobilePhone);
        }

        // Adds a new address to the contact addresses vector
        if (sampleContact.address) {
            if (sampleContact.address.street) {
                var homeAddress = new Windows.ApplicationModel.Contacts.ContactAddress();
                homeAddress.streetAddress = sampleContact.address.street;
                homeAddress.locality = sampleContact.address.city;
                homeAddress.region = sampleContact.address.state;
                homeAddress.postalCode = sampleContact.address.zipCode;
                homeAddress.kind = Windows.ApplicationModel.Contacts.ContactAddressKind.home;
                contact.addresses.append(homeAddress);
            }
        }

        // Add the contact to the basket
        var statusMessage = document.getElementById("statusMessage");
        switch (contactPickerUI.addContact(contact)) {
            case Windows.ApplicationModel.Contacts.Provider.AddContactResult.added:
                // Notify user that the contact was added to the basket
                statusMessage.innerText = contact.displayName + " was added to the basket";
                break;
            case Windows.ApplicationModel.Contacts.Provider.AddContactResult.alreadyAdded:
                // Notify the user that the contact is already in the basket
                statusMessage.innerText = contact.displayName + " is already in the basket";
                break;
            case Windows.ApplicationModel.Contacts.Provider.AddContactResult.unavailable:
            default:
                // Notify the user that the basket is not currently available
                statusMessage.innerText = contact.displayName + " could not be added to the basket";
                break;
        }
    }

    function removeContactFromBasket(sampleContact) {
        // Programmatically remove the contact from the basket
        if (contactPickerUI.containsContact(sampleContact.id)) {
            contactPickerUI.removeContact(sampleContact.id);
            document.getElementById("statusMessage").innerText = sampleContact.displayName + " was removed from the basket";
        }
    }

    function onContactRemoved(e) {
        // Add any code to be called when a contact is removed from the basket by the user
        var contactElement = document.getElementById(e.id);
        var sampleContact = sampleContacts[contactElement.value];
        contactElement.checked = false;
        document.getElementById("statusMessage").innerText += "\n" + sampleContact.displayName + " was removed from the basket";
    }

    function createContactUI(sampleContact, index) {
        // Adds a UI checkbox for a contact so that it can added or removed from the basket
        var element = document.createElement("div");
        document.getElementById("contactList").appendChild(element);
        element.innerHTML = "<div class='contact'><label>" +
                            "<input id='" + sampleContact.id + "' value='" + index + "' type='checkbox' />" +
                            sampleContact.displayName + "</label></div>";

        element.firstElementChild.addEventListener("change", function (ev) {
            if (ev.target.checked) {
                addContactToBasket(sampleContact);
            } else {
                removeContactFromBasket(sampleContact);
            }
        }, false);
    }

    // Sample set of contacts to pick from
    var sampleContacts = [
        {
            displayName: "David Jaffe",
            firstName: "David",
            lastName: "Jaffe",
            personalEmail: "david@contoso.com",
            workEmail: "david@cpandl.com",
            workPhone: "",
            homePhone: "248-555-0150",
            mobilePhone: "",
            address: {
                full: "",
                street: "",
                city: "",
                state: "",
                zipCode: ""
            },
            id: "761cb6fb-0270-451e-8725-bb575eeb24d5"
        },

        {
            displayName: "Kim Abercrombie",
            firstName: "Kim",
            lastName: "Abercrombie",
            personalEmail: "kim@contoso.com",
            workEmail: "kim@adatum.com",
            homePhone: "444 555-0001",
            workPhone: "245 555-0123",
            mobilePhone: "921 555-0187",
            address: {
                full: "123 Main St, Redmond WA, 23456",
                street: "123 Main St",
                city: "Redmond",
                state: "WA",
                zipCode: "23456"
            },
            id: "49b0652e-8f39-48c5-853b-e5e94e6b8a11"
        },

        {
            displayName: "Jeff Phillips",
            firstName: "Jeff",
            lastName: "Phillips",
            personalEmail: "jeff@contoso.com",
            workEmail: "jeff@fabrikam.com",
            homePhone: "987-555-0199",
            workPhone: "",
            mobilePhone: "543-555-0111",
            address: {
                full: "456 2nd Ave, Dallas TX, 12345",
                street: "456 2nd Ave",
                city: "Dallas",
                state: "TX",
                zipCode: "12345"
            },
            id: "864abfb4-8998-4355-8236-8d69e47ec832"
        },

        {
            displayName: "Arlene Huff",
            firstName: "Arlene",
            lastName: "Huff",
            personalEmail: "arlene@contoso.com",
            workEmail: "",
            homePhone: "",
            workPhone: "",
            mobilePhone: "234-555-0156",
            address: null,
            id: "27347af8-0e92-45b8-b14c-dd70fcd3b4a6"
        },

        {
            displayName: "Miles Reid",
            firstName: "Miles",
            lastName: "Reid",
            personalEmail: "miles@contoso.com",
            workEmail: "miles@proseware.com",
            homePhone: "",
            workPhone: "",
            mobilePhone: "",
            address: {
                full: "678 Elm St, New York, New York, 95111",
                street: "678 Elm St",
                city: "New York",
                state: "New York",
                zipCode: "95111"
            },
            id: "e3d24a99-0e29-41af-9add-18f5e3cfc518"
        },
    ];

})();

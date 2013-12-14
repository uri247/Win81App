//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenarioSingle.html", {
        ready: function (element, options) {
            document.getElementById("open").addEventListener("click", pickContact, false);
        }
    });

    function pickContact() {
        // Clean scenario output
        WinJS.log && WinJS.log("", "sample", "status");

        // Create the picker
        var picker = new Windows.ApplicationModel.Contacts.ContactPicker();
        picker.commitButtonText = "Select";

        // Open the picker for the user to select a contact
        picker.pickContactAsync().done(function (contact) {
            if (contact !== null) {
                // Create UI to display the contact information for the selected contact
                var contactElement = document.createElement("div");
                contactElement.className = "contact";

                // Display the name
                contactElement.appendChild(createTextElement("h3", contact.displayName));

                // Add the different types of contact data
                if (contact.emails.length > 0) {
                    contactElement.appendChild(createTextElement("h4", "Emails:"));
                    contact.emails.forEach(function (email) {
                        switch (email.kind) {
                            case Windows.ApplicationModel.Contacts.ContactEmailKind.personal:
                                contactElement.appendChild(createTextElement("div", email.address + " (personal)"));
                                break;
                            case Windows.ApplicationModel.Contacts.ContactEmailKind.work:
                                contactElement.appendChild(createTextElement("div", email.address + " (work)"));
                                break;
                            case Windows.ApplicationModel.Contacts.ContactEmailKind.other:
                                contactElement.appendChild(createTextElement("div", email.address + " (other)"));
                                break;
                        }
                    });
                }
                
                if (contact.phones.length > 0) {
                    contactElement.appendChild(createTextElement("h4", "Phone Numbers:"));
                    contact.phones.forEach(function (phone) {
                        switch (phone.kind) {
                            case Windows.ApplicationModel.Contacts.ContactPhoneKind.home:
                                contactElement.appendChild(createTextElement("div", phone.number + " (home)" ));
                                break;
                            case Windows.ApplicationModel.Contacts.ContactPhoneKind.work:
                                contactElement.appendChild(createTextElement("div", phone.number + " (work)" ));
                                break;
                            case Windows.ApplicationModel.Contacts.ContactPhoneKind.mobile:
                                contactElement.appendChild(createTextElement("div", phone.number + " (mobile)" ));
                                break;
                            case Windows.ApplicationModel.Contacts.ContactPhoneKind.other:
                                contactElement.appendChild(createTextElement("div", phone.number + " (other)" ));
                                break;
                        }
                    });
                }

                if (contact.addresses.length > 0) {
                    contactElement.appendChild(createTextElement("h4", "Addresses:"));
                    contact.addresses.forEach(function (address) {
                        switch (address.kind) {
                            case Windows.ApplicationModel.Contacts.ContactAddressKind.home:
                                contactElement.appendChild(createTextElement("div", getUnstructuredAddress(address) + " (home)" ));
                                break;
                            case Windows.ApplicationModel.Contacts.ContactAddressKind.work:
                                contactElement.appendChild(createTextElement("div", getUnstructuredAddress(address) + " (work)"));
                                break;
                            case Windows.ApplicationModel.Contacts.ContactAddressKind.other:
                                contactElement.appendChild(createTextElement("div", getUnstructuredAddress(address) + " (other)"));
                                break;
                        }
                    });
                }

                // Add the contact element to the page
                document.getElementById("output").appendChild(contactElement);
            } else {
                // The picker was dismissed without selecting a contact
                WinJS.log && WinJS.log("No contact was selected", "sample", "status");
            }
        });
    }

    function createTextElement(tag, text) {
        var element = document.createElement(tag);
        element.className = "singleLineText";
        element.innerText = text;
        return element;
    }

    function getUnstructuredAddress(contactAddress) {
        var unstructuredAddress = contactAddress.streetAddress + ", " + contactAddress.locality + ", " + contactAddress.region + ", " + contactAddress.postalCode;
        return unstructuredAddress;
    }


})();

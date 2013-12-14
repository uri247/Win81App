//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var Notifications = Windows.UI.Notifications;

    var page = WinJS.UI.Pages.define("/html/previewAllTemplates.html", {
        ready: function (element, options) {
            document.getElementById("tileTemplateSelector").addEventListener("change", tileTemplateSelector, false);
            document.getElementById("clearTileNotification").addEventListener("click", clearTileNotification, false);
            document.getElementById("sendTileNotification").addEventListener("click", sendTileNotification, false);
            document.getElementById("copyImages").addEventListener("click", copyImages, false);
            document.getElementById("viewAvailableImages").addEventListener("click", viewAvailableImages, false);
            document.getElementById("hideAvailableImages").addEventListener("click", hideAvailableImages, false);
            document.getElementById("tileTemplateSelector").selectedIndex = 10;
            tileTemplateSelector();
        }
    });

    function clearInputFields() {
        var input;
        for (input = 1; input < 7; input++) {
            document.getElementById("imageInput" + input).style.display = "none";
        }
        for (input = 1; input < 23; input++) {
            document.getElementById("textInput" + input).style.display = "none";
        }
    }

    function tileTemplateSelector() {
        clearInputFields();

        var template = document.getElementById("tileTemplateSelector").selectedIndex;
        var tileXml = Notifications.TileUpdateManager.getTemplateContent(template);

        var tileTextAttributes = tileXml.getElementsByTagName("text");
        for (var textAttribute = 1; textAttribute < tileTextAttributes.length + 1; textAttribute++) {
            document.getElementById("textInput" + /*@static_cast(String)*/textAttribute).style.display = "block";
        }

        var tileImageAttributes = tileXml.getElementsByTagName("image");
        for (var imageAttribute = 1; imageAttribute < tileImageAttributes.length + 1; imageAttribute++) {
            document.getElementById("imageInput" + /*@static_cast(String)*/imageAttribute).style.display = "block";
        }
        document.getElementById("tilePreview").setAttribute("src", "images/tiles/" + document.getElementById("tileTemplateSelector").options[document.getElementById("tileTemplateSelector").selectedIndex].value + ".png");
    }

    function copyImages() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.replaceAll([".jpg", ".jpeg", ".png", ".gif"]);
        picker.commitButtonText = "Copy";
        picker.pickMultipleFilesAsync().done(function (files) {
            if (files) {
                files.forEach(function (file) {
                    file.copyAsync(Windows.Storage.ApplicationData.current.localFolder, file.name, Windows.Storage.NameCollisionOption.generateUniqueName).done(function () { }, function (e) {
                        WinJS.log && WinJS.log(e, "sample", "error");
                    });
                });
            } else {
                WinJS.log && WinJS.log("Invalid files.", "sample", "error");
            }
        }, function (e) {
            WinJS.log && WinJS.log(e, "sample", "error");
        });
    }

    function viewAvailableImages() {
        document.getElementById("copiedFiles").innerHTML = "";
        Windows.Storage.ApplicationData.current.localFolder.getItemsAsync().done(function (items) {
            items.forEach(function (storageItem) {
                if (storageItem.fileType === ".png" || storageItem.fileType === ".jpg" || storageItem.fileType === ".jpeg" || storageItem.fileType === ".gif") {
                    document.getElementById("copiedFiles").innerHTML += "ms-appdata:///local/" + storageItem.name + "<br/>";
                }
            }, function (e) {
                WinJS.log && WinJS.log(e, "sample", "error");
            });
        });

        document.getElementById("availableFiles").style.display = "block";
        document.getElementById("viewAvailableImages").style.display = "none";
        document.getElementById("hideAvailableImages").style.display = "block";
    }

    function hideAvailableImages() {
        document.getElementById("availableFiles").style.display = "none";
        document.getElementById("viewAvailableImages").style.display = "block";
        document.getElementById("hideAvailableImages").style.display = "none";
    }

    function clearTileNotification() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Tile cleared", "sample", "status");
    }

    function sendTileNotification() {
        // This example uses the GetTemplateContent method to get the notification as xml instead of using NotificationExtensions.

        var template = document.getElementById("tileTemplateSelector").selectedIndex;

        var tileXml = Notifications.TileUpdateManager.getTemplateContent(template);
        var tileImageAttributes = tileXml.getElementsByTagName("image");
        var text;

        // Iterate over all the image input fields and set image attributes.
        for (var imageAttribute = 1; imageAttribute < tileImageAttributes.length + 1; imageAttribute++) {
            text = document.getElementById("imageInput" + /*@static_cast(String)*/imageAttribute + "Element").value;
            if (text !== "") {
                tileImageAttributes[imageAttribute - 1].setAttribute("src", text);
            }
        }

        var tileTextAttributes = tileXml.getElementsByTagName("text");

        // Iterate over all the text input fields and set text attributes.
        for (var textAttribute = 1; textAttribute < tileTextAttributes.length + 1; textAttribute++) {
            text = document.getElementById("textInput" + /*@static_cast(String)*/textAttribute + "Element").value;
            tileTextAttributes[textAttribute - 1].appendChild(tileXml.createTextNode(text));
        }

        // Set the branding on the notification as specified in the input.
        // The logo and display name are declared in the manifest.
        // Branding defaults to logo if omitted.
        var branding = document.getElementById("brandingSelector").options[document.getElementById("brandingSelector").selectedIndex].value;
        var binding = tileXml.getElementsByTagName("binding");
        binding[0].setAttribute("branding", branding);

        // Set the language of the notification. Though this is optional, it is recommended to specify the language.
        var lang = document.getElementById("fontInput").value;
        if (lang !== "") {
            // Specify the language of the text in the notification.
            // This ensures the correct font is used to render the text.
            var visualElement = tileXml.getElementsByTagName("visual");
            visualElement[0].setAttribute("lang", lang);
        }

        var tileNotification = new Notifications.TileNotification(tileXml);
        Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        WinJS.log && WinJS.log(tileXml.getXml(), "sample", "status");
    }
})();
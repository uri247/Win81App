//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario11.html", {
        ready: function (element, options) {
            document.getElementById("getFile").addEventListener("click", getFile, false);
        }
    });

    // Gets a file without throwing an exception
    function getFile() {
        var outputDiv = document.getElementById("output");
        Windows.Storage.KnownFolders.picturesLibrary.tryGetItemAsync("sample.dat").done(function (file) {
            if (file !== null) {
                outputDiv.innerHTML = "Operation result: " + file.name;
            } else {
                outputDiv.innerHTML = "Operation result: null";
            }
        });
    }
})();

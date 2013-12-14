//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("getParent").addEventListener("click", getParent, false);
            // To test if "sample.dat" is created.
            SdkSample.validateFileExistence();
        }
    });

    // Gets the file's parent folder
    function getParent() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            SdkSample.sampleFile.getParentAsync().done(function (parentFolder) {
                outputDiv.innerHTML = "Item: " + SdkSample.sampleFile.name + " (" + SdkSample.sampleFile.path + ")";
                outputDiv.innerHTML += "<br />";
                outputDiv.innerHTML += "Parent: " + parentFolder.name + " (" + parentFolder.path + ")";
            },
            function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
        }
    }
})();

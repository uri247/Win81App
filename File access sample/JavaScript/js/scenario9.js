//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario9.html", {
        ready: function (element, options) {
            document.getElementById("compareFiles").addEventListener("click", compareFiles, false);
            // To test if "sample.dat" is created.
            SdkSample.validateFileExistence();
        }
    });

    // Compares a picked file with sample.dat
    function compareFiles() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
            picker.fileTypeFilter.replaceAll(["*"]);
            picker.pickSingleFileAsync().done(function (comparand) {
                if (comparand !== null) {
                    if (SdkSample.sampleFile.isEqual(comparand)) {
                        outputDiv.innerHTML = "Files are equal";
                    } else {
                        outputDiv.innerHTML = "Files are not equal";
                    }
                } else {
                    outputDiv.innerHTML = "Operation cancelled";
                }
            },
            function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
        }
    }
})();

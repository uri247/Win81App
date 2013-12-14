//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appBar;

    var page = WinJS.UI.Pages.define("/html/custom-content.html", {
        ready: function (element, options) {
            appBar = document.getElementById("customContentAppBar").winControl;           
            appBar.getCommandById("cmdAdd").addEventListener("click", doClickAdd, false);
            appBar.getCommandById("cmdFavorites").addEventListener("click", doClickFavorites, false);

            WinJS.log && WinJS.log("To show the bar, swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, tap in the application, swipe, right-click, or press Windows Logo + z again.", "sample", "status");           
        },
        unload: function () {
            AppBarSampleUtils.removeAppBars();
        }
    });

    // Command button functions
    function doClickAdd() {
        WinJS.log && WinJS.log("Add button pressed", "sample", "status");
    }

    function doClickFavorites() {
        WinJS.log && WinJS.log("Favorites button pressed", "sample", "status");
    }
})();

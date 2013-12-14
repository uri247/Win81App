//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var isPaused = true;
    var appBar;

    var page = WinJS.UI.Pages.define("/html/custom-icons.html", {
        ready: function (element, options) {
            appBar = document.getElementById("customIconsAppBar").winControl;
            appBar.getCommandById("cmdAudio").addEventListener("click", doClickAudio, false);
            appBar.getCommandById("cmdPlay").addEventListener("click", doClickPlay, false);
            appBar.getCommandById("cmdAccept").addEventListener("click", doClickAccept, false);
            WinJS.log && WinJS.log("To show the bar, swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, tap in the application, swipe, right-click, or press Windows Logo + z again.", "sample", "status");
        },
        unload: function () {
            AppBarSampleUtils.removeAppBars();
        }
    });

    // Command button functions
    function doClickAudio() {
        var cmd = appBar.getCommandById('cmdAudio');
        WinJS.log && WinJS.log("Song details button pressed", "sample", "status");
    }
    function doClickPlay() {
        var cmd = appBar.getCommandById('cmdPlay');
        // Still need to change icon and label when selected - if selected, set label, etc.
        if (!isPaused) {
            isPaused = true; // paused
            cmd.icon = 'play';
            cmd.label = 'Play';
            cmd.tooltip = 'Play this song';
            WinJS.log && WinJS.log("Play button pressed.", "sample", "status");
        } else {
            isPaused = false; // playing
            cmd.icon = 'pause';
            cmd.label = 'Pause';
            cmd.tooltip = 'Pause this song';
            WinJS.log && WinJS.log("Pause button pressed.", "sample", "status");
        }
    }
    function doClickAccept() {
        WinJS.log && WinJS.log("Accept button pressed", "sample", "status");
    }

})();

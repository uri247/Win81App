//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Proximity JavaScript Sample";

    var scenarios = [
        { url: "/html/PeerFinder.html", title: "Use PeerFinder to connect to peers" },
        { url: "/html/PeerWatcher.html", title: "Use PeerWatcher to scan for peers" },
        { url: "/html/ProximityDevice.html", title: "Use ProximityDevice to publish and subscribe for messages" },
        { url: "/html/ProximityDeviceEvents.html", title: "Display ProximityDevice events" }
    ];

    var g_tapLaunch = false;
    var g_appRole = Windows.Networking.Proximity.PeerRole.peer;

    function getAppRole() {
        return g_appRole;
    }

    function isLaunchedByTap() {
        var tapLaunch = g_tapLaunch;
        g_tapLaunch = false;
        return tapLaunch;
    }

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                // This boolean value captures whether this app was launched by tap or not. 
                // If it was launched by tap, the sample automatically kicks off PeerFinder.

                if (eventObject.detail.arguments !== null) {
                    var arg = new String(eventObject.detail.arguments);
                    if (arg.search("Windows.Networking.Proximity.PeerFinder:StreamSocket") !== -1) {

                        g_tapLaunch = true;

                        if (arg.search("Role=Host") !== -1) {
                            g_appRole = Windows.Networking.Proximity.PeerRole.host;
                        }
                        else if (arg.search("Role=Client") !== -1) {
                            g_appRole = Windows.Networking.Proximity.PeerRole.client;
                        }
                        else {
                            g_appRole = Windows.Networking.Proximity.PeerRole.peer;
                        }
                    }
                }
                
                g_tapLaunch = ((eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) && (g_tapLaunch === true));
                if (g_tapLaunch) {
                    url = scenarios[0].url; // Force scenario 0 if launched by tap to start the PeerFinder.
                }



                return WinJS.Navigation.navigate(url, g_tapLaunch);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    var g_proximityDevice = null;
    function initializeProximityDevice() {
        if (!g_proximityDevice) {
            g_proximityDevice = Windows.Networking.Proximity.ProximityDevice.getDefault();
            // getDefault() will return null if no proximity devices are installed.
            if (!g_proximityDevice) {
                ProximityHelpers.displayError("Failed to get default proximity device, likely none installed.");
                return null;
            }
        }
        return g_proximityDevice;
    }

    WinJS.Namespace.define("ProximityHelpers", {
        displayError : function (msg) {
            WinJS.log && WinJS.log(msg, "sample", "error");
        },

        displayStatus: function (msg) {
            WinJS.log && WinJS.log(msg, "sample", "status");
        },

        clearLastError: function (msg) {
        },

        initializeProximityDevice: initializeProximityDevice,

        logInfo: function(logElement, message) {
            logElement.innerHTML += message + "<br/>";
        },

        clearLog: function(logElement) {
            logElement.innerHTML = "";
        },

        id : function (elementId) {
            return document.getElementById(elementId);
        },

        isLaunchedByTap: isLaunchedByTap,

        getAppRole: getAppRole
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();

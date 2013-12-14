//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "HttpClient";

    var scenarios = [
        { url: "/html/S1-get-text.html", title: "GET Text With Cache Control" },
        { url: "/html/S2-get-stream.html", title: "GET Stream" },
        { url: "/html/S3-get-list.html", title: "GET List" },
        { url: "/html/S4-post-text.html", title: "POST Text" },
        { url: "/html/S5-post-stream.html", title: "POST Stream" },
        { url: "/html/S6-post-multipart.html", title: "POST Multipart" },
        { url: "/html/S7-post-stream-with-progress.html", title: "POST Stream With Progress" },
        { url: "/html/S8-get-cookies.html", title: "Get Cookies" },
        { url: "/html/S9-set-cookie.html", title: "Set Cookie" },
        { url: "/html/S10-delete-cookie.html", title: "Delete Cookie" },
        { url: "/html/S11-metered-connection-filter.html", title: "Metered Connection Filter" },
        { url: "/html/S12-retry-filter.html", title: "Retry Filter" },
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
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

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();

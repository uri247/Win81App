//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var filter;
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/S1-get-text.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            httpClient = new Windows.Web.Http.HttpClient(filter);
        }
    });

    function start() {
        var outputField = document.getElementById("outputField");

        // The value of 'AddressField' is set by the user and is therefore untrusted input. If we can't create a
        // valid, absolute URI, we'll notify the user about the incorrect input.
        // Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set,
        // since the user may provide URIs for servers located on the intErnet or intrAnet. If apps only
        // communicate with servers on the intErnet, only the "Internet (Client)" capability should be set.
        // Similarly if an app is only intended to communicate on the intrAnet, only the "Home and Work
        // Networking" capability should be set.
        var resourceUri = Helpers.tryGetUri(document.getElementById("addressField").value.trim());
        if (!resourceUri) {
            return;
        }

        Helpers.scenarioStarted();
        WinJS.log && WinJS.log("In progress", "sample", "status");

        if (document.getElementById("readDefaultRadio").checked) {
            filter.cacheControl.readBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.default;
        } else if (document.getElementById("readMostRecentRadio").checked) {
            filter.cacheControl.readBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.mostRecent;
        } else if (document.getElementById("readOnlyFromCacheRadio").checked) {
            filter.cacheControl.readBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.onlyFromCache;
        }

        if (document.getElementById("writeDefaultRadio").checked) {
            filter.cacheControl.writeBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.default;
        } else if (document.getElementById("writeNoCacheRadio").checked) {
            filter.cacheControl.writeBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.noCache;
        }

        httpPromise = httpClient.getAsync(resourceUri).then(function (response) {
            return Helpers.displayTextResultAsync(response, outputField);
        });
        httpPromise.done(function (response) {
            WinJS.log && WinJS.log("Completed. Response came from " +
                Helpers.getEnumValueName(Windows.Web.Http.HttpResponseMessageSource, response.source) +
                ".", "sample", "status");
            Helpers.scenarioCompleted();
        }, Helpers.onError);
    }

    function cancel() {
        if (httpPromise) {
            httpPromise.cancel();
        }
    }
})();

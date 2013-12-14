//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/S11-metered-connection-filter.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);

            var baseProtocolFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            Helpers.meteredConnectionFilter = new HttpFilters.HttpMeteredConnectionFilter(baseProtocolFilter);
            httpClient = new Windows.Web.Http.HttpClient(Helpers.meteredConnectionFilter);

            // Set an application settings flyout.
            WinJS.Application.onsettings = function (e) {
                e.detail.applicationcommands = {
                    filter: {
                        title: "Metered Connection Filter",
                        href: "/html/metered-connection-filter-settings.html"
                    }
                };
                WinJS.UI.SettingsFlyout.populateSettings(e);
            };
            WinJS.Application.start();
        }
    });

    function start() {
        Helpers.scenarioStarted();
        WinJS.log && WinJS.log("In progress", "sample", "status");

        var outputField = document.getElementById("outputField");

        // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
        // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
        var resourceAddress = new Windows.Foundation.Uri(document.getElementById("addressField").value);

        var request = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.get, resourceAddress);

        var priority = HttpFilters.MeteredConnectionPriority.low;
        if (document.getElementById("mediumRadio").checked) {
            priority = HttpFilters.MeteredConnectionPriority.medium;
        } else if (document.getElementById("highRadio").checked) {
            priority = HttpFilters.MeteredConnectionPriority.high;
        }
        request.properties["meteredConnectionPriority"] = priority;

        httpPromise = httpClient.sendRequestAsync(request).then(function (response) {
            return Helpers.displayTextResultAsync(response, outputField);
        });
        httpPromise.done(function () {
            WinJS.log && WinJS.log("Completed", "sample", "status");
            Helpers.scenarioCompleted();
        }, Helpers.onError);
    }

    function cancel() {
        if (httpPromise) {
            httpPromise.cancel();
        }
    }
})();

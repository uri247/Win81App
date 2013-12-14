//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/S3-get-list.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            httpClient = new Windows.Web.Http.HttpClient();
        }
    });

    function start() {
        Helpers.scenarioStarted();
        WinJS.log && WinJS.log("In progress", "sample", "status");

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
            WinJS.log && WinJS.log("Invalid URI.", "sample", "error");
            return;
        }

        httpPromise = httpClient.getAsync(resourceUri).then(function (response) {
            return Helpers.displayTextResultAsync(response, outputField).then(function () {
                response.ensureSuccessStatusCode();
                return response.content.readAsStringAsync();
            });
        }).then(function (responseBodyAsText) {
            var outputList = document.getElementById("outputList");

            var parser = new window.DOMParser();
            var xml = parser.parseFromString(responseBodyAsText, "text/xml");
            var items = xml.firstChild;
            for (var i = 0; i < items.childNodes.length; i++) {
                if (items.childNodes[i].nodeType === Node.ELEMENT_NODE) {
                    var item = items.childNodes[i].getAttribute("name");
                    var option = document.createElement("option");
                    option.text = item;
                    option.value = item;
                    outputList.options.add(option);
                }
            }
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

//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/S6-post-multipart.html", {
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

        var form = new Windows.Web.Http.HttpMultipartFormDataContent();
        form.add(new Windows.Web.Http.HttpStringContent(document.getElementById("requestBodyField").value), "data");

        // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
        // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
        var resourceAddress = new Windows.Foundation.Uri(document.getElementById("addressField").value);

        // Do an asynchronous POST.
        httpPromise = httpClient.postAsync(resourceAddress, form).then(function (response) {
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

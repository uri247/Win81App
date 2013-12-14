//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/S5-post-stream.html", {
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

        // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
        // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
        var resourceAddress = new Windows.Foundation.Uri(document.getElementById("addressField").value);

        var contentLength = 1000;

        httpPromise = generateSampleStreamAsync(contentLength).then(function (stream) {
            var streamContent = new Windows.Web.Http.HttpStreamContent(stream);

            var request = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.post, resourceAddress);
            request.content = streamContent;

            // Do an asynchronous POST.
            return httpClient.sendRequestAsync(request);
        }).then(function (response) {
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

    function generateSampleStreamAsync(size) {
        var data = [];
        for (var i = 0; i < size; i++) {
            data[i] = 64;
        }

        var buffer = Windows.Security.Cryptography.CryptographicBuffer.createFromByteArray(data);
        var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
        return stream.writeAsync(buffer).then(function (bytesWritten) {
            // Rewind the stream.
            stream.seek(0);

            return stream;
        });
    }
})();

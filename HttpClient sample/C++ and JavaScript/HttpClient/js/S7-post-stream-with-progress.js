//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/S7-post-stream-with-progress.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            document.getElementById("chunkedResponseToggle").addEventListener("change", updateAddressField, false);

            httpClient = new Windows.Web.Http.HttpClient();
            updateAddressField();
        }
    });

    function updateAddressField() {
        // Tell the server we want a transfer-encoding chunked response.
        var queryString = "";
        var x = document.getElementById("chunkedResponseToggle");
        if (document.getElementById("chunkedResponseToggle").winControl.checked) {
            queryString = "?chunkedResponse=1";
        }

        Helpers.replaceQueryString(queryString);
    }

    function start() {
        Helpers.scenarioStarted();
        resetFields();
        WinJS.log && WinJS.log("In progress", "sample", "status");

        var outputField = document.getElementById("outputField");

        // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
        // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
        var resourceAddress = new Windows.Foundation.Uri(document.getElementById("addressField").value);

        var contentLength = 1000;

        httpPromise = generateSampleStreamAsync(contentLength).then(function (stream) {
            var streamContent = new Windows.Web.Http.HttpStreamContent(stream);

            // Do an asynchronous POST.
            return httpClient.postAsync(resourceAddress, streamContent);
        });
        httpPromise.done(function (response) {
            WinJS.log && WinJS.log("Completed", "sample", "status");
            Helpers.scenarioCompleted();
        },
        Helpers.onError,
        function (progress) {
            setInnerText("stageField", Helpers.getEnumValueName(
                Windows.Web.Http.HttpProgressStage, progress.stage));
            setInnerText("retriesField", progress.retries);
            setInnerText("bytesSentField", progress.bytesSent);
            setInnerText("bytesReceivedField", progress.bytesReceived);

            var totalBytesToSend = 0;
            if (progress.totalBytesToSend !== null) {
                totalBytesToSend = progress.totalBytesToSend;
                setInnerText("totalBytesToSendField", totalBytesToSend);
            } else {
                setInnerText("totalBytesToSendField", "unknown");
            }

            var totalBytesToReceive = 0;
            if (progress.totalBytesToReceive !== null) {
                totalBytesToReceive = progress.totalBytesToReceive;
                setInnerText("totalBytesToReceiveField", totalBytesToReceive);
            } else {
                setInnerText("totalBytesToReceiveField", "unknown");
            }

            var requestProgress = 0;
            if (progress.stage === Windows.Web.Http.HttpProgressStage.sendingContent && totalBytesToSend > 0) {
                requestProgress = progress.bytesSent * 50 / totalBytesToSend;
            } else if (progress.stage === Windows.Web.Http.HttpProgressStage.receivingContent) {
                // Start with 50 percent, request content was already sent.
                requestProgress += 50;

                if (totalBytesToReceive > 0) {
                    requestProgress += progress.bytesReceived * 50 / totalBytesToReceive;
                }
            } else {
                return;
            }

            document.getElementById("requestProgressBar").value = requestProgress;
        });
    }

    function resetFields() {
        setInnerText("stageField", "ASDF");
        setInnerText("retriesField", "0");
        setInnerText("bytesSentField", "0");
        setInnerText("totalBytesToSendField", "0");
        setInnerText("bytesReceivedField", "0");
        setInnerText("totalBytesToReceiveField", "0");

        document.getElementById("requestProgressBar").value = 0;
    }

    function setInnerText(id, value) {
        var field = document.getElementById(id);
        while (field.childNodes.length >= 1) {
            field.removeChild(field.firstChild);
        }
        field.appendChild(field.ownerDocument.createTextNode(value));
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

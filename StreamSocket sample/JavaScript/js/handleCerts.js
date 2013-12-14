//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/handleCerts.html", {
        ready: function (element, options) {
            document.getElementById("ConnectSocket").addEventListener("click", openClient, false);
            document.getElementById("hostNameConnect").addEventListener("change", socketsSample.getValues, false);
            document.getElementById("serviceNameConnect").addEventListener("change", socketsSample.getValues, false);
            socketsSample.hostNameConnect = "localhost";
            socketsSample.serviceNameConnect = "443";
            socketsSample.setValues();
        }
    });

    function openClient() {
        if (socketsSample.clientSocket) {
            socketsSample.displayStatus("Already have a client; call close to close the listener and the client.");
            return;
        }

        var serviceName = document.getElementById("serviceNameConnect").value;
        if (serviceName === "") {
            socketsSample.displayStatus("Please provide a service name.");
            return;
        }

        // By default 'hostNameConnect' is disabled and host name validation is not required. When enabling the text
        // box validating the host name is required since it was received from an untrusted source (user input).
        // The host name is validated by catching exceptions thrown by the HostName constructor.
        // Note that when enabling the text box users may provide names for hosts on the Internet that require the
        // "Internet (Client)" capability.
        var hostName;
        try {
            hostName = new Windows.Networking.HostName(document.getElementById("hostNameConnect").value);
        } catch (error) {
            socketsSample.displayStatus("Error: Invalid host name.");
            return;
        }

        connectSocketWithRetry(hostName, serviceName);
    }

    function connectSocketWithRetry(hostName, serviceName) {
        socketsSample.closing = false;
        socketsSample.clientSocket = new Windows.Networking.Sockets.StreamSocket();
        socketsSample.displayStatus("Connecting to: " + hostName.displayName);

        var abortedByUser = false;

        // Connect to the server (in our case the local IIS server).
        socketsSample.clientSocket.connectAsync(
            hostName,
            serviceName,
            Windows.Networking.Sockets.SocketProtectionLevel.tls12)
        .then(function () {
            // No SSL errors: return an empty promise and continue processing in the .done function
            return;
        }, function (reason) {
            // If the exception was caused by an SSL error that is ignorable we are going to prompt the user
            // with an enumeration of the errors and ask for permission to ignore.
            if (socketsSample.clientSocket.information.serverCertificateErrorSeverity ===
                Windows.Networking.Sockets.SocketSslErrorSeverity.ignorable) {
                return shouldIgnoreCertificateErrorsAsync(
                    socketsSample.clientSocket.information.serverCertificateErrors)
                    .then(function (userAcceptedRetry) {
                        if (userAcceptedRetry) {
                            return connectSocketWithRetryHandleSslErrorAsync(hostName, serviceName);
                        }

                        throw new Error("Connection aborted by user (certificate not trusted)");
                    });
            }

            // Handle other exceptions in done().
            throw reason;
        })
        .done(function () {
            // Get detailed certificate information.
            var certInformation = getCertificateInformation(
                socketsSample.clientSocket.information.serverCertificate,
                socketsSample.clientSocket.information.serverIntermediateCertificates);

            socketsSample.displayStatus("Connected to server. Certificate information: <br />" + certInformation);

            socketsSample.clientSocket.close();
            socketsSample.clientSocket = null;
        }, function (reason) {
            // If this is an unknown status it means that the error is fatal and retry will likely fail.
            if (("number" in reason) &&
                (Windows.Networking.Sockets.SocketError.getStatus(reason.number) ===
                Windows.Networking.Sockets.SocketErrorStatus.unknown)) {
                throw reason;
            }

            socketsSample.displayStatus(reason);
            socketsSample.clientSocket.close();
            socketsSample.clientSocket = null;
        });
    }

    function connectSocketWithRetryHandleSslErrorAsync(hostName, serviceName) {
        // ---------------------------------------------------------------------------
        // WARNING: Only test applications may ignore SSL errors.
        // In real applications, ignoring server certificate errors can lead to MITM
        // attacks (while the connection is secure, the server is not authenticated).
        // ---------------------------------------------------------------------------

        socketsSample.clientSocket.control.ignorableServerCertificateErrors.clear();
        for (var i = 0; i < socketsSample.clientSocket.information.serverCertificateErrors.length; i++) {
            socketsSample.clientSocket.control.ignorableServerCertificateErrors.append(
                socketsSample.clientSocket.information.serverCertificateErrors[i]);
        }

        socketsSample.displayStatus("Retrying connection");
        return socketsSample.clientSocket.connectAsync(
            hostName,
            serviceName,
            Windows.Networking.Sockets.SocketProtectionLevel.tls12);
    }

    function shouldIgnoreCertificateErrorsAsync(connectionErrors) {
        var connectionErrorString = getCertificateErrorDescription(connectionErrors[0]);
        for (var i = 1; i < connectionErrors.length; i++) {
            connectionErrorString += ", " + getCertificateErrorDescription(connectionErrors[i]);
        }

        var dialogMessage =
            "The remote server certificate validation failed with the following errors: " +
            connectionErrorString + "\r\nSecurity certificate problems may" +
            " indicate an attempt to fool you or intercept any data you send to the server.";

        var dialog = new Windows.UI.Popups.MessageDialog(dialogMessage, "Server Certificate Validation Errors");

        var continueButtonId = 1;
        var abortButtonId = 0;
        dialog.commands.append(new Windows.UI.Popups.UICommand("Continue (not recommended)", null, continueButtonId));
        dialog.commands.append(new Windows.UI.Popups.UICommand("Cancel", null, abortButtonId));
        dialog.defaultCommandIndex = 1;
        dialog.cancelCommandIndex = 1;

        return dialog.showAsync().then(function (selected) {
            return (selected.id === continueButtonId);
        });
    }

    function getCertificateInformation(serverCert, intermediateCertificates) {
        var sb = "";
        sb += "Friendly Name: " + serverCert.friendlyName + "<br />";
        sb += "Subject: " + serverCert.subject + "<br />";
        sb += "Issuer: " + serverCert.issuer + "<br />";
        sb += "Validity: " + serverCert.validFrom + " - " + serverCert.validTo + "<br />";

        // Enumerate the entire certificate chain
        if (intermediateCertificates.length > 0) {
            sb += "Certificate chain: <br />";

            for (var i = 0; i < intermediateCertificates.length; i++) {
                var cert = intermediateCertificates[i];
                sb += "Intermediate Certificate Subject: " + cert.subject + "<br />";
            }
        } else {
            sb += "No certificates within the intermediate chain. <br />";
        }

        return sb;
    }

    function getCertificateErrorDescription(errorNum)
    {
        for (var stringName in Windows.Security.Cryptography.Certificates.ChainValidationResult) {
            if (Windows.Security.Cryptography.Certificates.ChainValidationResult[stringName] === errorNum) {
                return stringName;
            }
        }

        return null;
    }

})();

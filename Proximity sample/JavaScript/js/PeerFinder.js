//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ProxNS = Windows.Networking.Proximity;
    var discoveredPeers;      // List of peerinformation objects returned by findAllPeersAsync
    
    var requestingPeer;
    var triggeredConnectSupported = false;
    var browseConnectSupported = false;
    var peerFinderStarted = false;
    var role = ProxNS.PeerRole.peer;

    function triggeredConnectionStateChangedEventHandler(e) {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "triggeredConnectionStateChangedEventHandler - " +
                    ProximitySockets.getConnectState(e.state));

        if (e.state === ProxNS.TriggeredConnectState.peerFound) {
            ProximityHelpers.clearLastError();
            // Use this state to indicate to users that the tap is complete and
            // they can pull there devices away.
            ProximityHelpers.displayError("Tap complete, socket connection starting!");
        }

        if (e.state === ProxNS.TriggeredConnectState.completed) {
            ProximityHelpers.displayStatus("Socket connect success!");
            // Grab and use the socket that just connected.
            startSendReceive(e.socket, null);
        }

        // The socket conenction failed
        if (e.state === ProxNS.TriggeredConnectState.failed) {
            ProximityHelpers.displayError("Socket connect failed!");
        }
    }

    function socketError(errMessage) {
        ProximityHelpers.displayError(errMessage);
        ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "inline";

        // Browse and DiscoveryData controls are valid for Browse support
        if (browseConnectSupported) {
            ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "inline";
        }
        ProximityHelpers.id("peerFinder_Send").style.display = "none";
        ProximityHelpers.id("peerFinder_Message").style.display = "none";

        // Clear the SendToPeerList
        ProximityHelpers.id("peerFinder_SendToPeerList").style.display = "none";
        var sendToPeerList = ProximityHelpers.id("peerFinder_SendToPeerList");
        for (var i = sendToPeerList.options.length; i > 0; i--) {
            sendToPeerList.options[i] = null;
        }

        socketHelper.closeSocket();
    }

    var socketHelper = new ProximitySockets.socketHelper(socketError);

    // This gets called when we receive a connect request from a Peer
    function connectionRequestedEventHandler(e) {
        requestingPeer = e.peerInformation;
        ProximityHelpers.displayStatus("Connection Requested from peer: " + requestingPeer.displayName);

        ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "none";
        ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "none";
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "inline";
        ProximityHelpers.id("peerFinder_Send").style.display = "none";
        ProximityHelpers.id("peerFinder_Message").style.display = "none";
        ProximityHelpers.id("peerFinder_SendToPeerList").style.display = "none";
        ProximityHelpers.id("peerFinder_SelectRole").style.display = "none";
        ProximityHelpers.id("peerFinder_DiscoveryData").style.display = "none";
        ProximityHelpers.id("peerFinder_Connect").style.display = "none";
        ProximityHelpers.id("peerFinder_FoundPeersList").style.display = "none";
    }

    function peerFinder_AcceptRequest() {
        // Accept the connection if the user clicks okay.
        ProximityHelpers.displayStatus("Connecting to " + requestingPeer.displayName + " ...");
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "none";

        // Connect to the incoming peer
        ProxNS.PeerFinder.connectAsync(requestingPeer).done(
            function (proximitySocket) {
                ProximityHelpers.displayStatus("Connect to " + requestingPeer.displayName + " succeeded");
                startSendReceive(proximitySocket, requestingPeer);
            },
            function (err) {
                ProximityHelpers.displayError("Connect to " + requestingPeer.displayName + " failed with " + err);
                ProximityHelpers.id("peerFinder_Connect").style.display = "none";
            });
    }

    function peerFinder_StartAdvertising() {
        // If PeerFinder is started, stop it, so that new properties
        // selected by the user (Role/DiscoveryData) can be updated.
        if (peerFinderStarted) {
            ProxNS.PeerFinder.stop();

            peerFinderStarted = false;
        }

        if (!peerFinderStarted) {
            // Set the PeerFinder.role property

            if (ProximityHelpers.isLaunchedByTap()) {
                ProxNS.PeerFinder.role = ProximityHelpers.getAppRole();
            }
            else {
                switch (ProximityHelpers.id("peerFinder_SelectRole").value) {
                    case "peer":
                        ProxNS.PeerFinder.role = ProxNS.PeerRole.peer;
                        break;
                    case "host":
                        ProxNS.PeerFinder.role = ProxNS.PeerRole.host;
                        break;
                    case "client":
                        ProxNS.PeerFinder.role = ProxNS.PeerRole.client;
                        break;
                }
            }

            // Set discoveryData property if the user entered some text
            if (ProximityHelpers.id("peerFinder_DiscoveryData").value !== "What's happening today?") {
                var discoveryDataWriter = Windows.Storage.Streams.DataWriter(new Windows.Storage.Streams.InMemoryRandomAccessStream());
                discoveryDataWriter.writeString(ProximityHelpers.id("peerFinder_DiscoveryData").value);
                ProxNS.PeerFinder.discoveryData = discoveryDataWriter.detachBuffer();
            }

            // Then start listening for proximate peers
            ProxNS.PeerFinder.start();
            peerFinderStarted = true;            
        }

        if (browseConnectSupported && triggeredConnectSupported) {
            ProximityHelpers.displayStatus("Tap another device to connect to a peer or click Browse for Peers button.");
            ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "inline";
            ProximityHelpers.id("peerFinder_SelectRole").style.display = "inline";
            ProximityHelpers.id("peerFinder_DiscoveryData").style.display = "inline";
        } else if (triggeredConnectSupported) {
            ProximityHelpers.displayStatus("Tap another device to connect to a peer.");
        } else if (browseConnectSupported) {
            ProximityHelpers.displayStatus("Click browse for peers button");
            ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "inline";
            ProximityHelpers.id("peerFinder_SelectRole").style.display = "inline";
            ProximityHelpers.id("peerFinder_DiscoveryData").style.display = "inline";
        }
    }

    function peerFinder_BrowsePeers() {
        ProximityHelpers.displayStatus("Finding Peers ...");
        var foundPeersList = ProximityHelpers.id("peerFinder_FoundPeersList");

        // Empty the current option list
        for (var i = foundPeersList.options.length; i >= 0; i--) {
            foundPeersList.options[i] = null;
        }

        // Find all discoverable peers with compatible roles
        ProxNS.PeerFinder.findAllPeersAsync().done(
            function (peerInfoCollection) {
                var statusMessage = "Found " + peerInfoCollection.size + " peers";
                ProximityHelpers.displayStatus();
                var optionElement;
                var textElement;
                // add newly found peers into the drop down list.
                for (i = 0; i < peerInfoCollection.size; i++) {
                    var peerInformation = peerInfoCollection[i];
                    optionElement = document.createElement("option");
                    optionElement.setAttribute("value", i + 1);

                    var displayName = peerInformation.displayName;

                    // Append the discoveryData text to the DisplayName
                    if (peerInformation.discoveryData) {
                        var discoveryDataReader = Windows.Storage.Streams.DataReader.fromBuffer(peerInformation.discoveryData);
                        var discoveryData = discoveryDataReader.readString(peerInformation.discoveryData.length);
                        displayName += " '" + discoveryData + "'";
                    }

                    textElement = document.createTextNode(displayName);
                    optionElement.appendChild(textElement);
                    foundPeersList.appendChild(optionElement);
                    statusMessage += "\n" + peerInformation.displayName;
                }
                if (peerInfoCollection.size === 0) {
                    // Indicate that no peers were found by adding a "None Found"
                    // item in the peer list.
                    optionElement = document.createElement("option");
                    optionElement.setAttribute("value", 1);
                    textElement = document.createTextNode("None Found");
                    optionElement.appendChild(textElement);
                    foundPeersList.appendChild(optionElement);
                    ProximityHelpers.id("peerFinder_Connect").style.display = "none";
                } else {
                    ProximityHelpers.id("peerFinder_Connect").style.display = "inline";
                }
                ProximityHelpers.id("peerFinder_FoundPeersList").style.display = "inline";

                ProximityHelpers.displayStatus(statusMessage);
                discoveredPeers = peerInfoCollection;
            },
            function (err) {
                ProximityHelpers.displayError(err);
            });
    }

    function startSendReceive(proximitySocket, peerInformation) {
        var index = socketHelper.connectedPeers.length;
        socketHelper.connectedPeers[index] = new ProximitySockets.connectedPeer(proximitySocket, false, new Windows.Storage.Streams.DataWriter(proximitySocket.outputStream));

        if (!peerFinderStarted) {
            socketHelper.closeSocket();
            return;
        }

        // show the send button and the list of peers to send to
        ProximityHelpers.id("peerFinder_Send").style.display = "inline";
        ProximityHelpers.id("peerFinder_Message").style.display = "inline";
        ProximityHelpers.id("peerFinder_SendToPeerList").style.display = "inline";

        if (peerInformation) {
            // Add a new peer to the list of peers.
            var optionElement = document.createElement("option");
            optionElement.setAttribute("value", index);
            var textElement = document.createTextNode(peerInformation.displayName);
            optionElement.appendChild(textElement);
            ProximityHelpers.id("peerFinder_SendToPeerList").appendChild(optionElement);
        }

        // Hide the controls related to setting up a connection
        ProximityHelpers.id("peerFinder_Connect").style.display = "none";
        ProximityHelpers.id("peerFinder_FoundPeersList").style.display = "none";
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "none";
        ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "none";
        ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "none";
        ProximityHelpers.id("peerFinder_SelectRole").style.display = "none";
        ProximityHelpers.id("peerFinder_DiscoveryData").style.display = "none";

        socketHelper.startReader(socketHelper.connectedPeers[index]);
    }

    function peerFinder_Connect() {
        var foundPeersList = ProximityHelpers.id("peerFinder_FoundPeersList");
        if (discoveredPeers && discoveredPeers.length > 0) {
            var peerToConnect = discoveredPeers[foundPeersList.selectedIndex];
            ProximityHelpers.displayStatus("Connecting to " + peerToConnect.displayName + " ...");
            ProxNS.PeerFinder.connectAsync(peerToConnect).done(
                function (proximitySocket) {
                    ProximityHelpers.displayStatus("Connect succeeded");
                    startSendReceive(proximitySocket, peerToConnect);
                },
                function (err) {
                    ProximityHelpers.displayError("Connect failed with " + err);
                });
        }
    }

    // Send message to the selected peer(s)
    function peerFinder_Send() {
        ProximityHelpers.displayError("");
        var msg = ProximityHelpers.id("peerFinder_Message");
        var sendToPeerList = ProximityHelpers.id("peerFinder_SendToPeerList");

        var string = msg.value;
        msg.value = ""; // clear the message box after sending it.
        string.trim();

        if (string.length > 0) {
            // Send message to all peers
            if (sendToPeerList.value === "all") {
                for (var i in socketHelper.connectedPeers) {
                    socketHelper.sendMessageToPeer(string, socketHelper.connectedPeers[i]);
                }
            } else if (sendToPeerList.value >= 0 && sendToPeerList.value < socketHelper.connectedPeers.length) {
                // Send message to selected peer
                socketHelper.sendMessageToPeer(string, socketHelper.connectedPeers[sendToPeerList.value]);
            }
        } else {
            ProximityHelpers.displayError("Please enter a message");
        }
    }

    var page = WinJS.UI.Pages.define("/html/PeerFinder.html", {
        ready: function (element, options) {
            var peerFinderErrorMsg = null;
            var supportedDiscoveryTypes = ProxNS.PeerFinder.supportedDiscoveryTypes;

            ProximityHelpers.id("peerFinder_Connect").style.display = "none";
            ProximityHelpers.id("peerFinder_FoundPeersList").style.display = "none";
            ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "none";
            ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "none";
            ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "none";
            ProximityHelpers.id("peerFinder_SelectRole").style.display = "none";

            ProximityHelpers.id("peerFinder_Send").style.display = "none";
            ProximityHelpers.id("peerFinder_Message").style.display = "none";
            ProximityHelpers.id("peerFinder_SendToPeerList").style.display = "none";

            // First attach the event handlers (there can only be one handler for each event).
            ProxNS.PeerFinder.ontriggeredconnectionstatechanged = triggeredConnectionStateChangedEventHandler;
            ProxNS.PeerFinder.onconnectionrequested = connectionRequestedEventHandler;

            // Enable triggered related buttons only if the hardware support is present
            if (supportedDiscoveryTypes & ProxNS.PeerDiscoveryTypes.triggered) {
                triggeredConnectSupported = true;
            } else {
                peerFinderErrorMsg = "Tap based discovery of peers not supported \n";
            }

            // Enable browse related buttons only if the hardware support is present
            if (supportedDiscoveryTypes & ProxNS.PeerDiscoveryTypes.browse) {
                browseConnectSupported = true;
                ProximityHelpers.id("peerFinder_AcceptRequest").addEventListener("click", peerFinder_AcceptRequest, false);
                ProximityHelpers.id("peerFinder_BrowsePeers").addEventListener("click", peerFinder_BrowsePeers, false);
                ProximityHelpers.id("peerFinder_Connect").addEventListener("click", peerFinder_Connect, false);
            } else {
                if (peerFinderErrorMsg) {
                    peerFinderErrorMsg += "Browsing for peers not supported";
                } else {
                    peerFinderErrorMsg = "Browsing for peers not supported";
                }
            }

            if (triggeredConnectSupported || browseConnectSupported) {
                ProximityHelpers.id("peerFinder_StartAdvertising").addEventListener("click", peerFinder_StartAdvertising, false);
                ProximityHelpers.id("peerFinder_Send").addEventListener("click", peerFinder_Send, false);

                ProximityHelpers.id("peerFinder_SelectRole").style.display = "inline";
                ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "inline";
            } else {
                ProximityHelpers.id("peerFinder_SelectRole").style.display = "none";
                ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "none";
            }

            if (peerFinderErrorMsg) {
                ProximityHelpers.displayError(peerFinderErrorMsg);
            }

            if (options === true) {
                // If this sample app was launched by a proximity tap, start the peerfinder
                // right away to get connected.
                peerFinder_StartAdvertising();
            }
        },

        unload: function () {
            if (peerFinderStarted) {
                ProxNS.PeerFinder.ontriggeredconnectionstatechanged = null;
                ProxNS.PeerFinder.onconnectionrequested = null;
                ProxNS.PeerFinder.stop();
                socketHelper.closeSocket();
                peerFinderStarted = false;
            }
        }
    });


})();

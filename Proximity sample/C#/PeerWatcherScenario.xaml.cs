//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using SDKTemplate;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Proximity
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PeerWatcherScenario : SDKTemplate.Common.LayoutAwarePage, System.IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as rootPage.NotifyUser()
        MainPage rootPage = MainPage.Current;

        private PeerWatcher _peerWatcher;
        private bool _peerWatcherIsRunning = false;
        
        private PeerInformation _requestingPeer;
        private bool _triggeredConnectSupported = false;
        private bool _browseConnectSupported = false;

        // The ListView will display peers from this collection
        ObservableCollection<PeerInformation> _discoveredPeers = new ObservableCollection<PeerInformation>();

        // Helper to encapsulate the StreamSocket work
        private SocketHelper _socketHelper = new SocketHelper();

        public PeerWatcherScenario()
        {
            this.InitializeComponent();

            _socketHelper.RaiseSocketErrorEvent += SocketErrorHandler;
            _socketHelper.RaiseMessageEvent += MessageHandler;

            // Scenario 1 init
            _triggeredConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Triggered) ==
                                         PeerDiscoveryTypes.Triggered;
            _browseConnectSupported = (PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) ==
                                      PeerDiscoveryTypes.Browse;
            if (_triggeredConnectSupported || _browseConnectSupported)
            {
                // This scenario demonstrates "PeerFinder" to tap or browse for peers to connect to using a StreamSocket
                PeerFinder_AdvertiseButton.Visibility = Visibility.Visible;
                PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed;
                PeerFinder_SelectRole.Visibility = Visibility.Visible;

                peerListCVS.Source = _discoveredPeers;
            }
        }

        // Handles PeerFinder.TriggeredConnectionStateChanged event
        async private void TriggeredConnectionStateChangedEventHandler(object sender, TriggeredConnectionStateChangedEventArgs eventArgs)
        {
            rootPage.UpdateLog("TriggeredConnectionStateChangedEventHandler - " + Enum.GetName(typeof(ConnectState), (int)eventArgs.State), PeerFinderOutputText);

            if (eventArgs.State == TriggeredConnectState.PeerFound)
            {
                // Use this state to indicate to users that the tap is complete and
                // they can pull their devices away.
                rootPage.NotifyUser("Tap complete, socket connection starting!", NotifyType.StatusMessage);
            }

            if (eventArgs.State == TriggeredConnectState.Completed)
            {
                rootPage.NotifyUser("Socket connect success!", NotifyType.StatusMessage);
                // Start using the socket that just connected.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.PeerFinder_StartSendReceive(eventArgs.Socket, null);
                });
            }

            if (eventArgs.State == TriggeredConnectState.Failed)
            {
                // The socket conenction failed
                rootPage.NotifyUser("Socket connect failed!", NotifyType.ErrorMessage);
            }
        }

        private bool _peerFinderStarted = false;

        private void SocketErrorHandler(object sender, SocketEventArgs e)
        {
            rootPage.NotifyUser(e.Message, NotifyType.ErrorMessage);
            PeerFinder_AdvertiseButton.Visibility = Visibility.Visible;
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed;
            PeerFinder_AdvertiseGrid.Visibility = Visibility.Visible;

            // Browse and DiscoveryData controls are valid for Browse support
            if (_browseConnectSupported)
            {
                PeerFinder_BrowseGrid.Visibility = Visibility.Visible;
            }
            PeerFinder_ConnectionGrid.Visibility = Visibility.Collapsed;

            // Clear the SendToPeerList
            PeerFinder_SendToPeerList.Visibility = Visibility.Collapsed;
            PeerFinder_SendToPeerList.Items.Clear();

            _socketHelper.CloseSocket();
        }

        private void MessageHandler(object sender, MessageEventArgs e)
        {
            rootPage.NotifyUser(e.Message, NotifyType.StatusMessage);
        }

        // Send message to the selected peer(s)
        // Handles PeerFinder_SendButton click
        private void PeerFinder_Send(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            String message = PeerFinder_MessageBox.Text;
            PeerFinder_MessageBox.Text = ""; // clear the input now that the message is being sent.
            int idx = PeerFinder_SendToPeerList.SelectedIndex - 1;

            if (message.Length > 0)
            {
                // Send message to all peers
                if (((ComboBoxItem)(PeerFinder_SendToPeerList.SelectedItem)).Content.ToString() == "All Peers")
                {
                    foreach (ConnectedPeer obj in _socketHelper.ConnectedPeers)
                    {
                        _socketHelper.SendMessageToPeer(message, obj);
                    }
                }
                else if ((idx >= 0) && (idx < _socketHelper.ConnectedPeers.Count))
                {
                    // Sned message to selected peer
                    _socketHelper.SendMessageToPeer(message, (_socketHelper.ConnectedPeers)[idx]);
                }
            } else
            {
                rootPage.NotifyUser("Please type a message", NotifyType.ErrorMessage);
            }
        }

        // Handles PeerFinder_AcceptButton click
        async private void PeerFinder_Accept(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("Connecting to " + _requestingPeer.DisplayName + "....", NotifyType.StatusMessage);
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
            try
            {
                // Connect to the incoming peer
                StreamSocket socket = await PeerFinder.ConnectAsync(_requestingPeer);
                rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage);
                PeerFinder_StartSendReceive(socket, _requestingPeer);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Connection to " + _requestingPeer.DisplayName + " failed: " + err.Message, NotifyType.ErrorMessage);
            }
        }

        // This gets called when we receive a connect request from a Peer
        private void PeerConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            _requestingPeer = args.PeerInformation;
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser("Connection requested from peer " + args.PeerInformation.DisplayName, NotifyType.StatusMessage);
                this.PeerFinder_AdvertiseGrid.Visibility = Visibility.Collapsed;
                this.PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed;
                this.PeerFinder_ConnectionGrid.Visibility = Visibility.Visible;
                this.PeerFinder_AcceptButton.Visibility = Visibility.Visible;
                this.PeerFinder_SendButton.Visibility = Visibility.Collapsed;
                this.PeerFinder_MessageBox.Visibility = Visibility.Collapsed;
                this.PeerFinder_SendToPeerList.Visibility = Visibility.Collapsed;
            });
        }

        // Start the send receive operations
        void PeerFinder_StartSendReceive(StreamSocket socket, PeerInformation peerInformation)
        {
            ConnectedPeer connectedPeer = new ConnectedPeer(socket, false, new Windows.Storage.Streams.DataWriter(socket.OutputStream));
            _socketHelper.Add(connectedPeer);

            if (!_peerFinderStarted)
            {
                _socketHelper.CloseSocket();
                return;
            }

            PeerFinder_ConnectionGrid.Visibility = Visibility.Visible;
            PeerFinder_SendButton.Visibility = Visibility.Visible;
            PeerFinder_MessageBox.Visibility = Visibility.Visible;
            PeerFinder_SendToPeerList.Visibility = Visibility.Visible;

            if (peerInformation != null)
            {
                // Add a new peer to the list of peers.
                ComboBoxItem item = new ComboBoxItem();
                item.Content = peerInformation.DisplayName;
                PeerFinder_SendToPeerList.Items.Add(item);
                PeerFinder_SendToPeerList.SelectedIndex = 0;
            }

            // Hide the controls related to setting up a connection
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
            PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed;
            PeerFinder_AdvertiseGrid.Visibility = Visibility.Collapsed;

            _socketHelper.StartReader(connectedPeer);
        }

        // Handles PeerFinder_ConnectButton click
        async void PeerFinder_Connect(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            PeerInformation peerToConnect = null;

            if (PeerFinder_FoundPeersList.Items.Count == 0)
            {
                rootPage.NotifyUser("Cannot connect, there were no peers found!", NotifyType.ErrorMessage);
            }
            else
            {
                try
                {
                    peerToConnect = (PeerInformation)PeerFinder_FoundPeersList.SelectedItem;
                    if (peerToConnect == null)
                    {
                        peerToConnect = (PeerInformation)PeerFinder_FoundPeersList.Items[0];
                    }

                    rootPage.NotifyUser("Connecting to " + peerToConnect.DisplayName + "....", NotifyType.StatusMessage);
                    StreamSocket socket = await PeerFinder.ConnectAsync(peerToConnect);
                    rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage);
                    PeerFinder_StartSendReceive(socket, peerToConnect);
                }
                catch (Exception err)
                {
                    rootPage.NotifyUser("Connection to " + peerToConnect.DisplayName + " failed: " + err.Message, NotifyType.ErrorMessage);
                }
            }
        }

        // Handles PeerFinder_AdvertiseButton click
        void PeerFinder_StartAdvertising(object sender, RoutedEventArgs e)
        {
            // If PeerFinder is started, stop it, so that new properties
            // selected by the user (Role/DiscoveryData) can be updated.
            PeerFinder_StopAdvertising(sender, e);

            rootPage.NotifyUser("", NotifyType.ErrorMessage);
            if (!_peerFinderStarted)
            {
                // attach the callback handler (there can only be one PeerConnectProgress handler).
                PeerFinder.TriggeredConnectionStateChanged += new TypedEventHandler<object, TriggeredConnectionStateChangedEventArgs>(TriggeredConnectionStateChangedEventHandler);
                // attach the incoming connection request event handler
                PeerFinder.ConnectionRequested += new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerConnectionRequested);

                // Set the PeerFinder.Role property
                switch (PeerFinder_SelectRole.SelectionBoxItem.ToString())
                {
                    case "Peer":
                        PeerFinder.Role = PeerRole.Peer;
                        break;
                    case "Host":
                        PeerFinder.Role = PeerRole.Host;
                        break;
                    case "Client":
                        PeerFinder.Role = PeerRole.Client;
                        break;
                }

                // Set DiscoveryData property if the user entered some text
                if ((PeerFinder_DiscoveryData.Text.Length > 0) && (PeerFinder_DiscoveryData.Text != "What's happening today?"))
                {
                    using (var discoveryDataWriter = new Windows.Storage.Streams.DataWriter(new Windows.Storage.Streams.InMemoryRandomAccessStream()))
                    {
                        discoveryDataWriter.WriteString(PeerFinder_DiscoveryData.Text);
                        PeerFinder.DiscoveryData = discoveryDataWriter.DetachBuffer();
                    }
                }

                // start listening for proximate peers
                PeerFinder.Start();
                _peerFinderStarted = true;
                PeerFinder_StopAdvertiseButton.Visibility = Visibility.Visible;
                PeerFinder_ConnectButton.Visibility = Visibility.Visible;

                if (_browseConnectSupported && _triggeredConnectSupported)
                {
                    rootPage.NotifyUser("Click Browse for Peers button or tap another device to connect to a peer.", NotifyType.StatusMessage);
                    PeerFinder_BrowseGrid.Visibility = Visibility.Visible;
                }
                else if (_triggeredConnectSupported)
                {
                    rootPage.NotifyUser("Tap another device to connect to a peer.", NotifyType.StatusMessage);
                }
                else if (_browseConnectSupported)
                {
                    rootPage.NotifyUser("Click Browse for Peers button.", NotifyType.StatusMessage);
                    PeerFinder_BrowseGrid.Visibility = Visibility.Visible;
                }
            }
        }

        // Handles PeerFinder_StopAdvertiseButton click
        void PeerFinder_StopAdvertising(object sender, RoutedEventArgs e)
        {
            if (_peerFinderStarted)
            {
                PeerFinder.Stop();
                _peerFinderStarted = false;

                rootPage.NotifyUser("Stopped Advertising.", NotifyType.StatusMessage);
                PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed;
                PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed;
                PeerFinder_ConnectButton.Visibility = Visibility.Collapsed;
            }
        }

        // Handles PeerFinder_StartPeerWatcherButton click
        void PeerFinder_StartPeerWatcher(object sender, RoutedEventArgs e)
        {
            if (_peerWatcherIsRunning)
            {
                rootPage.UpdateLog("Can't start PeerWatcher while it is running!", PeerFinderOutputText);
                return;
            }

            rootPage.NotifyUser("Starting PeerWatcher...", NotifyType.StatusMessage);
            try
            {
                if (_peerWatcher == null)
                {
                    _peerWatcher = PeerFinder.CreateWatcher();
                    // Hook up events, this should only be done once
                    _peerWatcher.Added += PeerWatcher_Added;
                    _peerWatcher.Removed += PeerWatcher_Removed;
                    _peerWatcher.Updated += PeerWatcher_Updated;
                    _peerWatcher.EnumerationCompleted += PeerWatcher_EnumerationCompleted;
                    _peerWatcher.Stopped += PeerWatcher_Stopped;
                }

                _discoveredPeers.Clear();

                PeerFinder_PeerListNoPeers.Visibility = Visibility.Collapsed;
                PeerFinder_ConnectButton.Visibility = Visibility.Visible;

                _peerWatcher.Start();

                _peerWatcherIsRunning = true;
                rootPage.UpdateLog("PeerWatcher is running!", PeerFinderOutputText);

                PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Collapsed;
                PeerFinder_StopPeerWatcherButton.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // This could happen if the user clicks start multiple times or tries to start while PeerWatcher is stopping
                Debug.WriteLine("PeerWatcher.Start throws exception" + ex.Message);
            }
        }

        // Handles PeerFinder_StopPeerWatcherButton click
        void PeerFinder_StopPeerWatcher(object sender, RoutedEventArgs e)
        {
            rootPage.UpdateLog("Stopping PeerWatcher... wait for Stopped Event", PeerFinderOutputText);
            try
            {
                _peerWatcher.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PeerWatcher.Stop throws exception" + ex.Message);
            }
        }

        // Helper for more readable logging
        private String GetTruncatedPeerId(String id)
        {
            String truncated = id;
            if (id.Length > 10)
            {
                truncated = id.Substring(0, 5) + "..." + id.Substring(id.Length - 5);
            }
            return truncated;
        }

        // PeerWatcher events
        private void PeerWatcher_Added(PeerWatcher sender, PeerInformation peerInfo)
        {
            rootPage.UpdateLog("Peer added: "
                + GetTruncatedPeerId(peerInfo.Id)
                + ", name:" + peerInfo.DisplayName, PeerFinderOutputText);
            // Update the UI
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                lock (this)
                {
                    PeerFinder_PeerListNoPeers.Visibility = Visibility.Collapsed;
                    _discoveredPeers.Add(peerInfo);
                }
            });
        }

        private void PeerWatcher_Removed(PeerWatcher sender, PeerInformation peerInfo)
        {
            rootPage.UpdateLog("Peer removed: "
                + GetTruncatedPeerId(peerInfo.Id)
                + ", name:" + peerInfo.DisplayName, PeerFinderOutputText);
            // Update the UI
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                lock (this)
                {
                    for (int i = 0; i < _discoveredPeers.Count; i++)
                    {
                        if (_discoveredPeers[i].Id == peerInfo.Id)
                        {
                            _discoveredPeers.RemoveAt(i);
                        }
                    }
                }
            });
        }

        private void PeerWatcher_Updated(PeerWatcher sender, PeerInformation peerInfo)
        {
            rootPage.UpdateLog("Peer updated: "
                + GetTruncatedPeerId(peerInfo.Id)
                + ", name:" + peerInfo.DisplayName, PeerFinderOutputText);
            // Update the UI
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                lock (this)
                {
                    for (int i = 0; i < _discoveredPeers.Count; i++)
                    {
                        if (_discoveredPeers[i].Id == peerInfo.Id)
                        {
                            _discoveredPeers[i] = peerInfo;
                        }
                    }
                }
            });
        }

        private void PeerWatcher_EnumerationCompleted(PeerWatcher sender, object o)
        {
            rootPage.UpdateLog("PeerWatcher Enumeration Completed", PeerFinderOutputText);
            // All peers that were visible at the start of the scan have been found
            // Stopping PeerWatcher here is similar to FindAllPeersAsync

            // Notify the user that no peers were found after we have done an initial scan
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                lock (this)
                {
                    if (_discoveredPeers.Count == 0)
                    {
                        PeerFinder_PeerListNoPeers.Visibility = Visibility.Visible;
                    }
                }
            });
        }

        private void PeerWatcher_Stopped(PeerWatcher sender, object o)
        {
            // This indicates that the PeerWatcher was stopped explicitly through PeerWatcher.Stop, or it was aborted
            // The Status property indicates the cause of the event
            rootPage.UpdateLog("PeerWatcher Stopped. Status: " + _peerWatcher.Status, PeerFinderOutputText);
            // PeerWatcher is now actually stopped and we can start it again, update the UI button state accordingly
            _peerWatcherIsRunning = false;
            var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Visible;
                PeerFinder_StopPeerWatcherButton.Visibility = Visibility.Collapsed;
            });
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PeerFinder_AdvertiseButton.Visibility = Visibility.Collapsed;
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed;

            PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed;

            PeerFinder_StopPeerWatcherButton.Visibility = Visibility.Collapsed;
            PeerFinder_ConnectButton.Visibility = Visibility.Collapsed;

            PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Collapsed;

            PeerFinder_ConnectionGrid.Visibility = Visibility.Collapsed;
            PeerFinder_SendButton.Visibility = Visibility.Collapsed;
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed;
            PeerFinder_MessageBox.Visibility = Visibility.Collapsed;
            PeerFinder_PeerListNoPeers.Visibility = Visibility.Collapsed;

            PeerFinder_SelectRole.Visibility = Visibility.Collapsed;
            PeerFinder_DiscoveryData.Visibility = Visibility.Collapsed;

            if (_triggeredConnectSupported || _browseConnectSupported)
            {
                // Initially only the advertise button, Role list and DiscoveryData box should be visible.
                PeerFinder_AdvertiseButton.Visibility = Visibility.Visible;

                // These are hidden in the grid
                if (_browseConnectSupported)
                {
                    PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Visible;
                }

                PeerFinder_MessageBox.Text = "Hello World";
                PeerFinder_SelectRole.Visibility = Visibility.Visible;
                PeerFinder_DiscoveryData.Visibility = Visibility.Visible;
                PeerFinder_DiscoveryData.Text = "What's happening today?";   
            }
            if (!_browseConnectSupported)
            {
                rootPage.NotifyUser("Browsing for peers not supported", NotifyType.ErrorMessage);
            }
        }

        // Invoked when the main page navigates to a different scenario
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // If PeerFinder was started, stop it when navigating to a different page.
            if (_peerFinderStarted)
            {
                // detach the callback handler (there can only be one PeerConnectProgress handler).
                PeerFinder.TriggeredConnectionStateChanged -= new TypedEventHandler<object, TriggeredConnectionStateChangedEventArgs>(TriggeredConnectionStateChangedEventHandler);
                // detach the incoming connection request event handler
                PeerFinder.ConnectionRequested -= new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerConnectionRequested);
                PeerFinder.Stop();
                _socketHelper.CloseSocket();
                _peerFinderStarted = false;
            }
            if (_peerWatcher != null)
            {
                // unregister for events
                _peerWatcher.Added -= PeerWatcher_Added;
                _peerWatcher.Removed -= PeerWatcher_Removed;
                _peerWatcher.Updated -= PeerWatcher_Updated;
                _peerWatcher.EnumerationCompleted -= PeerWatcher_EnumerationCompleted;
                _peerWatcher.Stopped -= PeerWatcher_Stopped;

                _peerWatcher = null;
            }
        }

        public void Dispose()
        {
            _socketHelper.CloseSocket();
        }
    }
}

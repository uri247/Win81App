'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Networking.Proximity
Imports Windows.Networking.Sockets

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PeerWatcherScenario
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Implements System.IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as rootPage.NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private _peerWatcher As PeerWatcher
    Private _peerWatcherIsRunning As Boolean = False

    Private _requestingPeer As PeerInformation
    Private _triggeredConnectSupported As Boolean = False
    Private _browseConnectSupported As Boolean = False

    ' The ListView will display peers from this collection
    Private _discoveredPeers As New ObservableCollection(Of PeerInformation)()

    ' Helper to encapsulate the StreamSocket work
    Private _socketHelper As New SocketHelper()

    Public Sub New()
        Me.InitializeComponent()

        AddHandler _socketHelper.RaiseSocketErrorEvent, AddressOf SocketErrorHandler
        AddHandler _socketHelper.RaiseMessageEvent, AddressOf MessageHandler

        ' Scenario 1 init
        _triggeredConnectSupported = (PeerFinder.SupportedDiscoveryTypes And PeerDiscoveryTypes.Triggered) = PeerDiscoveryTypes.Triggered
        _browseConnectSupported = (PeerFinder.SupportedDiscoveryTypes And PeerDiscoveryTypes.Browse) = PeerDiscoveryTypes.Browse
        If _triggeredConnectSupported OrElse _browseConnectSupported Then
            ' This scenario demonstrates "PeerFinder" to tap or browse for peers to connect to using a StreamSocket
            PeerFinder_AdvertiseButton.Visibility = Visibility.Visible
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed
            PeerFinder_SelectRole.Visibility = Visibility.Visible

            peerListCVS.Source = _discoveredPeers
        End If
    End Sub

    ' Handles PeerFinder.TriggeredConnectionStateChanged event
    Private Async Sub TriggeredConnectionStateChangedEventHandler(ByVal sender As Object, ByVal eventArgs As TriggeredConnectionStateChangedEventArgs)
        rootPage.UpdateLog("TriggeredConnectionStateChangedEventHandler - " & System.Enum.GetName(GetType(ConnectState), CInt(eventArgs.State)), PeerFinderOutputText)

        If eventArgs.State = TriggeredConnectState.PeerFound Then
            ' Use this state to indicate to users that the tap is complete and
            ' they can pull their devices away.
            rootPage.NotifyUser("Tap complete, socket connection starting!", NotifyType.StatusMessage)
        End If

        If eventArgs.State = TriggeredConnectState.Completed Then
            rootPage.NotifyUser("Socket connect success!", NotifyType.StatusMessage)
            ' Start using the socket that just connected.
            Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() Me.PeerFinder_StartSendReceive(eventArgs.Socket, Nothing))
        End If

        If eventArgs.State = TriggeredConnectState.Failed Then
            ' The socket conenction failed
            rootPage.NotifyUser("Socket connect failed!", NotifyType.ErrorMessage)
        End If
    End Sub

    Private _peerFinderStarted As Boolean = False

    Private Sub SocketErrorHandler(ByVal sender As Object, ByVal e As SocketEventArgs)
        rootPage.NotifyUser(e.Message, NotifyType.ErrorMessage)
        PeerFinder_AdvertiseButton.Visibility = Visibility.Visible
        PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed
        PeerFinder_AdvertiseGrid.Visibility = Visibility.Visible

        ' Browse and DiscoveryData controls are valid for Browse support
        If _browseConnectSupported Then
            PeerFinder_BrowseGrid.Visibility = Visibility.Visible
        End If
        PeerFinder_ConnectionGrid.Visibility = Visibility.Collapsed

        ' Clear the SendToPeerList
        PeerFinder_SendToPeerList.Visibility = Visibility.Collapsed
        PeerFinder_SendToPeerList.Items.Clear()

        _socketHelper.CloseSocket()
    End Sub

    Private Sub MessageHandler(ByVal sender As Object, ByVal e As MessageEventArgs)
        rootPage.NotifyUser(e.Message, NotifyType.StatusMessage)
    End Sub

    ' Send message to the selected peer(s)
    ' Handles PeerFinder_SendButton click
    Private Sub PeerFinder_Send(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.ErrorMessage)
        Dim message As String = PeerFinder_MessageBox.Text
        PeerFinder_MessageBox.Text = "" ' clear the input now that the message is being sent.
        Dim idx As Integer = PeerFinder_SendToPeerList.SelectedIndex - 1

        If message.Length > 0 Then
            ' Send message to all peers
            If CType(PeerFinder_SendToPeerList.SelectedItem, ComboBoxItem).Content.ToString() = "All Peers" Then
                For Each obj As ConnectedPeer In _socketHelper.ConnectedPeers
                    _socketHelper.SendMessageToPeer(message, obj)
                Next obj
            ElseIf (idx >= 0) AndAlso (idx < _socketHelper.ConnectedPeers.Count) Then
                ' Sned message to selected peer
                _socketHelper.SendMessageToPeer(message, (_socketHelper.ConnectedPeers)(idx))
            End If
        Else
            rootPage.NotifyUser("Please type a message", NotifyType.ErrorMessage)
        End If
    End Sub

    ' Handles PeerFinder_AcceptButton click
    Private Async Sub PeerFinder_Accept(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.NotifyUser("Connecting to " & _requestingPeer.DisplayName & "....", NotifyType.StatusMessage)
        PeerFinder_AcceptButton.Visibility = Visibility.Collapsed
        Try
            ' Connect to the incoming peer
            Dim socket As StreamSocket = Await PeerFinder.ConnectAsync(_requestingPeer)
            rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage)
            PeerFinder_StartSendReceive(socket, _requestingPeer)
        Catch err As Exception
            rootPage.NotifyUser("Connection to " & _requestingPeer.DisplayName & " failed: " & err.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    ' This gets called when we receive a connect request from a Peer
    Private Sub PeerConnectionRequested(ByVal sender As Object, ByVal args As ConnectionRequestedEventArgs)
        _requestingPeer = args.PeerInformation
        Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                             rootPage.NotifyUser("Connection requested from peer " & args.PeerInformation.DisplayName, NotifyType.StatusMessage)
                                                                             Me.PeerFinder_AdvertiseGrid.Visibility = Visibility.Collapsed
                                                                             Me.PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed
                                                                             Me.PeerFinder_ConnectionGrid.Visibility = Visibility.Visible
                                                                             Me.PeerFinder_AcceptButton.Visibility = Visibility.Visible
                                                                             Me.PeerFinder_SendButton.Visibility = Visibility.Collapsed
                                                                             Me.PeerFinder_MessageBox.Visibility = Visibility.Collapsed
                                                                             Me.PeerFinder_SendToPeerList.Visibility = Visibility.Collapsed
                                                                         End Sub)
    End Sub

    ' Start the send receive operations
    Private Sub PeerFinder_StartSendReceive(ByVal socket As StreamSocket, ByVal peerInformation As PeerInformation)
        Dim connectedPeer As New ConnectedPeer(socket, False, New Windows.Storage.Streams.DataWriter(socket.OutputStream))
        _socketHelper.Add(connectedPeer)

        If Not _peerFinderStarted Then
            _socketHelper.CloseSocket()
            Return
        End If

        PeerFinder_ConnectionGrid.Visibility = Visibility.Visible
        PeerFinder_SendButton.Visibility = Visibility.Visible
        PeerFinder_MessageBox.Visibility = Visibility.Visible
        PeerFinder_SendToPeerList.Visibility = Visibility.Visible

        If peerInformation IsNot Nothing Then
            ' Add a new peer to the list of peers.
            Dim item As New ComboBoxItem()
            item.Content = peerInformation.DisplayName
            PeerFinder_SendToPeerList.Items.Add(item)
            PeerFinder_SendToPeerList.SelectedIndex = 0
        End If

        ' Hide the controls related to setting up a connection
        PeerFinder_AcceptButton.Visibility = Visibility.Collapsed
        PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed
        PeerFinder_AdvertiseGrid.Visibility = Visibility.Collapsed

        _socketHelper.StartReader(connectedPeer)
    End Sub

    ' Handles PeerFinder_ConnectButton click
    Private Async Sub PeerFinder_Connect(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.ErrorMessage)
        Dim peerToConnect As PeerInformation = Nothing

        If PeerFinder_FoundPeersList.Items.Count = 0 Then
            rootPage.NotifyUser("Cannot connect, there were no peers found!", NotifyType.ErrorMessage)
        Else
            Try
                peerToConnect = CType(PeerFinder_FoundPeersList.SelectedItem, PeerInformation)
                If peerToConnect Is Nothing Then
                    peerToConnect = CType(PeerFinder_FoundPeersList.Items(0), PeerInformation)
                End If

                rootPage.NotifyUser("Connecting to " & peerToConnect.DisplayName & "....", NotifyType.StatusMessage)
                Dim socket As StreamSocket = Await PeerFinder.ConnectAsync(peerToConnect)
                rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage)
                PeerFinder_StartSendReceive(socket, peerToConnect)
            Catch err As Exception
                rootPage.NotifyUser("Connection to " & peerToConnect.DisplayName & " failed: " & err.Message, NotifyType.ErrorMessage)
            End Try
        End If
    End Sub

    ' Handles PeerFinder_AdvertiseButton click
    Private Sub PeerFinder_StartAdvertising(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' If PeerFinder is started, stop it, so that new properties
        ' selected by the user (Role/DiscoveryData) can be updated.
        PeerFinder_StopAdvertising(sender, e)

        rootPage.NotifyUser("", NotifyType.ErrorMessage)
        If Not _peerFinderStarted Then
            ' attach the callback handler (there can only be one PeerConnectProgress handler).
            AddHandler PeerFinder.TriggeredConnectionStateChanged, AddressOf TriggeredConnectionStateChangedEventHandler
            ' attach the incoming connection request event handler
            AddHandler PeerFinder.ConnectionRequested, AddressOf PeerConnectionRequested

            ' Set the PeerFinder.Role property
            Select Case PeerFinder_SelectRole.SelectionBoxItem.ToString()
                Case "Peer"
                    PeerFinder.Role = PeerRole.Peer
                Case "Host"
                    PeerFinder.Role = PeerRole.Host
                Case "Client"
                    PeerFinder.Role = PeerRole.Client
            End Select

            ' Set DiscoveryData property if the user entered some text
            If (PeerFinder_DiscoveryData.Text.Length > 0) AndAlso (PeerFinder_DiscoveryData.Text <> "What's happening today?") Then
                Using discoveryDataWriter = New Windows.Storage.Streams.DataWriter(New Windows.Storage.Streams.InMemoryRandomAccessStream())
                    discoveryDataWriter.WriteString(PeerFinder_DiscoveryData.Text)
                    PeerFinder.DiscoveryData = discoveryDataWriter.DetachBuffer()
                End Using
            End If

            ' start listening for proximate peers
            PeerFinder.Start()
            _peerFinderStarted = True
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Visible
            PeerFinder_ConnectButton.Visibility = Visibility.Visible

            If _browseConnectSupported AndAlso _triggeredConnectSupported Then
                rootPage.NotifyUser("Click Browse for Peers button or tap another device to connect to a peer.", NotifyType.StatusMessage)
                PeerFinder_BrowseGrid.Visibility = Visibility.Visible
            ElseIf _triggeredConnectSupported Then
                rootPage.NotifyUser("Tap another device to connect to a peer.", NotifyType.StatusMessage)
            ElseIf _browseConnectSupported Then
                rootPage.NotifyUser("Click Browse for Peers button.", NotifyType.StatusMessage)
                PeerFinder_BrowseGrid.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    ' Handles PeerFinder_StopAdvertiseButton click
    Private Sub PeerFinder_StopAdvertising(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If _peerFinderStarted Then
            PeerFinder.Stop()
            _peerFinderStarted = False

            rootPage.NotifyUser("Stopped Advertising.", NotifyType.StatusMessage)
            PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed
            PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed
            PeerFinder_ConnectButton.Visibility = Visibility.Collapsed
        End If
    End Sub

    ' Handles PeerFinder_StartPeerWatcherButton click
    Private Sub PeerFinder_StartPeerWatcher(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If _peerWatcherIsRunning Then
            rootPage.UpdateLog("Can't start PeerWatcher while it is running!", PeerFinderOutputText)
            Return
        End If

        rootPage.NotifyUser("Starting PeerWatcher...", NotifyType.StatusMessage)
        Try
            If _peerWatcher Is Nothing Then
                _peerWatcher = PeerFinder.CreateWatcher()
                ' Hook up events, this should only be done once
                AddHandler _peerWatcher.Added, AddressOf PeerWatcher_Added
                AddHandler _peerWatcher.Removed, AddressOf PeerWatcher_Removed
                AddHandler _peerWatcher.Updated, AddressOf PeerWatcher_Updated
                AddHandler _peerWatcher.EnumerationCompleted, AddressOf PeerWatcher_EnumerationCompleted
                AddHandler _peerWatcher.Stopped, AddressOf PeerWatcher_Stopped
            End If

            _discoveredPeers.Clear()

            PeerFinder_PeerListNoPeers.Visibility = Visibility.Collapsed
            PeerFinder_ConnectButton.Visibility = Visibility.Visible

            _peerWatcher.Start()

            _peerWatcherIsRunning = True
            rootPage.UpdateLog("PeerWatcher is running!", PeerFinderOutputText)

            PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Collapsed
            PeerFinder_StopPeerWatcherButton.Visibility = Visibility.Visible
        Catch ex As Exception
            ' This could happen if the user clicks start multiple times or tries to start while PeerWatcher is stopping
            Debug.WriteLine("PeerWatcher.Start throws exception" & ex.Message)
        End Try
    End Sub

    ' Handles PeerFinder_StopPeerWatcherButton click
    Private Sub PeerFinder_StopPeerWatcher(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.UpdateLog("Stopping PeerWatcher... wait for Stopped Event", PeerFinderOutputText)
        Try
            _peerWatcher.Stop()
        Catch ex As Exception
            Debug.WriteLine("PeerWatcher.Stop throws exception" & ex.Message)
        End Try
    End Sub

    ' Helper for more readable logging
    Private Function GetTruncatedPeerId(ByVal id As String) As String
        Dim truncated As String = id
        If id.Length > 10 Then
            truncated = id.Substring(0, 5) & "..." & id.Substring(id.Length - 5)
        End If
        Return truncated
    End Function

    ' PeerWatcher events
    Private Sub PeerWatcher_Added(ByVal sender As PeerWatcher, ByVal peerInfo As PeerInformation)
        rootPage.UpdateLog("Peer added: " & GetTruncatedPeerId(peerInfo.Id) & ", name:" & peerInfo.DisplayName, PeerFinderOutputText)
        ' Update the UI
        Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, Sub()
                                                                          SyncLock Me
                                                                              PeerFinder_PeerListNoPeers.Visibility = Visibility.Collapsed
                                                                              _discoveredPeers.Add(peerInfo)
                                                                          End SyncLock
                                                                      End Sub)
    End Sub

    Private Sub PeerWatcher_Removed(ByVal sender As PeerWatcher, ByVal peerInfo As PeerInformation)
        rootPage.UpdateLog("Peer removed: " & GetTruncatedPeerId(peerInfo.Id) & ", name:" & peerInfo.DisplayName, PeerFinderOutputText)
        ' Update the UI
        Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, Sub()
                                                                          SyncLock Me
                                                                              Dim i As Integer = 0
                                                                              Do While i < _discoveredPeers.Count
                                                                                  If _discoveredPeers(i).Id = peerInfo.Id Then
                                                                                      _discoveredPeers.RemoveAt(i)
                                                                                  End If
                                                                                  i += 1
                                                                              Loop
                                                                          End SyncLock
                                                                      End Sub)
    End Sub

    Private Sub PeerWatcher_Updated(ByVal sender As PeerWatcher, ByVal peerInfo As PeerInformation)
        rootPage.UpdateLog("Peer updated: " & GetTruncatedPeerId(peerInfo.Id) & ", name:" & peerInfo.DisplayName, PeerFinderOutputText)
        ' Update the UI
        Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, Sub()
                                                                          SyncLock Me
                                                                              For i As Integer = 0 To _discoveredPeers.Count - 1
                                                                                  If _discoveredPeers(i).Id = peerInfo.Id Then
                                                                                      _discoveredPeers(i) = peerInfo
                                                                                  End If
                                                                              Next i
                                                                          End SyncLock
                                                                      End Sub)
    End Sub

    Private Sub PeerWatcher_EnumerationCompleted(ByVal sender As PeerWatcher, ByVal o As Object)
        rootPage.UpdateLog("PeerWatcher Enumeration Completed", PeerFinderOutputText)
        ' All peers that were visible at the start of the scan have been found
        ' Stopping PeerWatcher here is similar to FindAllPeersAsync

        ' Notify the user that no peers were found after we have done an initial scan
        Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, Sub()
                                                                          SyncLock Me
                                                                              If _discoveredPeers.Count = 0 Then
                                                                                  PeerFinder_PeerListNoPeers.Visibility = Visibility.Visible
                                                                              End If
                                                                          End SyncLock
                                                                      End Sub)
    End Sub

    Private Sub PeerWatcher_Stopped(ByVal sender As PeerWatcher, ByVal o As Object)
        ' This indicates that the PeerWatcher was stopped explicitly through PeerWatcher.Stop, or it was aborted
        ' The Status property indicates the cause of the event
        rootPage.UpdateLog("PeerWatcher Stopped. Status: " & _peerWatcher.Status, PeerFinderOutputText)
        ' PeerWatcher is now actually stopped and we can start it again, update the UI button state accordingly
        _peerWatcherIsRunning = False
        Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Low, Sub()
                                                                          PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Visible
                                                                          PeerFinder_StopPeerWatcherButton.Visibility = Visibility.Collapsed
                                                                      End Sub)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        PeerFinder_AdvertiseButton.Visibility = Visibility.Collapsed
        PeerFinder_StopAdvertiseButton.Visibility = Visibility.Collapsed

        PeerFinder_BrowseGrid.Visibility = Visibility.Collapsed

        PeerFinder_StopPeerWatcherButton.Visibility = Visibility.Collapsed
        PeerFinder_ConnectButton.Visibility = Visibility.Collapsed

        PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Collapsed

        PeerFinder_ConnectionGrid.Visibility = Visibility.Collapsed
        PeerFinder_SendButton.Visibility = Visibility.Collapsed
        PeerFinder_AcceptButton.Visibility = Visibility.Collapsed
        PeerFinder_MessageBox.Visibility = Visibility.Collapsed
        PeerFinder_PeerListNoPeers.Visibility = Visibility.Collapsed

        PeerFinder_SelectRole.Visibility = Visibility.Collapsed
        PeerFinder_DiscoveryData.Visibility = Visibility.Collapsed

        If _triggeredConnectSupported OrElse _browseConnectSupported Then
            ' Initially only the advertise button, Role list and DiscoveryData box should be visible.
            PeerFinder_AdvertiseButton.Visibility = Visibility.Visible

            ' These are hidden in the grid
            If _browseConnectSupported Then
                PeerFinder_StartPeerWatcherButton.Visibility = Visibility.Visible
            End If

            PeerFinder_MessageBox.Text = "Hello World"
            PeerFinder_SelectRole.Visibility = Visibility.Visible
            PeerFinder_DiscoveryData.Visibility = Visibility.Visible
            PeerFinder_DiscoveryData.Text = "What's happening today?"
        End If
        If Not _browseConnectSupported Then
            rootPage.NotifyUser("Browsing for peers not supported", NotifyType.ErrorMessage)
        End If
    End Sub

    ' Invoked when the main page navigates to a different scenario
    Protected Overrides Sub OnNavigatingFrom(ByVal e As NavigatingCancelEventArgs)
        ' If PeerFinder was started, stop it when navigating to a different page.
        If _peerFinderStarted Then
            ' detach the callback handler (there can only be one PeerConnectProgress handler).
            RemoveHandler PeerFinder.TriggeredConnectionStateChanged, AddressOf TriggeredConnectionStateChangedEventHandler
            ' detach the incoming connection request event handler
            RemoveHandler PeerFinder.ConnectionRequested, AddressOf PeerConnectionRequested
            PeerFinder.Stop()
            _socketHelper.CloseSocket()
            _peerFinderStarted = False
        End If
        If _peerWatcher IsNot Nothing Then
            ' unregister for events
            RemoveHandler _peerWatcher.Added, AddressOf PeerWatcher_Added
            RemoveHandler _peerWatcher.Removed, AddressOf PeerWatcher_Removed
            RemoveHandler _peerWatcher.Updated, AddressOf PeerWatcher_Updated
            RemoveHandler _peerWatcher.EnumerationCompleted, AddressOf PeerWatcher_EnumerationCompleted
            RemoveHandler _peerWatcher.Stopped, AddressOf PeerWatcher_Stopped

            _peerWatcher = Nothing
        End If
    End Sub

    Public Sub Dispose() Implements System.IDisposable.Dispose
        _socketHelper.CloseSocket()
    End Sub
End Class


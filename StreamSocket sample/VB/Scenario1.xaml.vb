'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Runtime.InteropServices
Imports Windows.ApplicationModel.Core
Imports Windows.Networking
Imports Windows.Networking.Connectivity
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams

''' <summary>
''' A page for first scenario.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' List containing all available local HostName endpoints
    Private localHostItems As New List(Of LocalHostItem)()

    Public Sub New()
        Me.InitializeComponent()
        PopulateAdapterList()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        BindToAny.IsChecked = True
    End Sub

    Private Sub BindToAny_Checked(ByVal sender As Object, ByVal e As RoutedEventArgs)
        AdapterList.IsEnabled = False
    End Sub

    Private Sub BindToAny_Unchecked(ByVal sender As Object, ByVal e As RoutedEventArgs)
        AdapterList.IsEnabled = True
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'StartListener' button.
    ''' </summary>
    ''' <param name="sender">Object for which the event was generated.</param>
    ''' <param name="e">Event's parameters.</param>
    Private Async Sub StartListener_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Overriding the listener here is safe as it will be deleted once all references to it are gone. 
        ' However, in many cases this is a dangerous pattern to override data semi-randomly (each time user 
        ' clicked the button) so we block it here.
        If CoreApplication.Properties.ContainsKey("listener") Then
            rootPage.NotifyUser("This step has already been executed. Please move to the next one.", NotifyType.ErrorMessage)
            Return
        End If

        If String.IsNullOrEmpty(ServiceNameForListener.Text) Then
            rootPage.NotifyUser("Please provide a service name.", NotifyType.ErrorMessage)
            Return
        End If

        CoreApplication.Properties.Remove("serverAddress")
        CoreApplication.Properties.Remove("adapter")

        Dim selectedLocalHost As LocalHostItem = Nothing
        If (BindToAddress.IsChecked = True) OrElse (BindToAdapter.IsChecked = True) Then
            selectedLocalHost = CType(AdapterList.SelectedItem, LocalHostItem)
            If selectedLocalHost Is Nothing Then
                rootPage.NotifyUser("Please select an address / adapter.", NotifyType.ErrorMessage)
                Return
            End If

            ' The user selected an address. For demo purposes, we ensure that connect will be using the same 
            ' address.
            CoreApplication.Properties.Add("serverAddress", selectedLocalHost.LocalHost.CanonicalName)
        End If

        Dim listener As New StreamSocketListener()
        AddHandler listener.ConnectionReceived, AddressOf OnConnection

        ' Save the socket, so subsequent steps can use it.
        CoreApplication.Properties.Add("listener", listener)

        ' Start listen operation.
        Try
            If BindToAny.IsChecked = True Then
                ' Don't limit traffic to an address or an adapter.
                Await listener.BindServiceNameAsync(ServiceNameForListener.Text)
                rootPage.NotifyUser("Listening", NotifyType.StatusMessage)
            ElseIf BindToAddress.IsChecked = True Then
                ' Try to bind to a specific address.
                Await listener.BindEndpointAsync(selectedLocalHost.LocalHost, ServiceNameForListener.Text)
                rootPage.NotifyUser("Listening on address " & selectedLocalHost.LocalHost.CanonicalName, NotifyType.StatusMessage)
            ElseIf BindToAdapter.IsChecked = True Then
                ' Try to limit traffic to the selected adapter. 
                ' This option will be overriden by interfaces with weak-host or forwarding modes enabled.
                Dim selectedAdapter As NetworkAdapter = selectedLocalHost.LocalHost.IPInformation.NetworkAdapter

                ' For demo purposes, ensure that use the same adapter in the client connect scenario.
                CoreApplication.Properties.Add("adapter", selectedAdapter)

                Await listener.BindServiceNameAsync(ServiceNameForListener.Text, SocketProtectionLevel.PlainSocket, selectedAdapter)

                rootPage.NotifyUser("Listening on adapter " & selectedAdapter.NetworkAdapterId.ToString(), NotifyType.StatusMessage)
            End If
        Catch exception As Exception
            CoreApplication.Properties.Remove("listener")

            ' If this is an unknown status it means that the error is fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            rootPage.NotifyUser("Start listening failed with error: " & exception.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' Invoked once a connection is accepted by StreamSocketListener.
    ''' </summary>
    ''' <param name="sender">The listener that accepted the connection.</param>
    ''' <param name="args">Parameters associated with the accepted connection.</param>
    Private Async Sub OnConnection(ByVal sender As StreamSocketListener, ByVal args As StreamSocketListenerConnectionReceivedEventArgs)
        Dim reader As New DataReader(args.Socket.InputStream)
        Try
            Do
                ' Read first 4 bytes (length of the subsequent string).
                Dim sizeFieldCount As UInteger = Await reader.LoadAsync(CUInt(Marshal.SizeOf(New UInteger)))
                If sizeFieldCount <> Marshal.SizeOf(New UInteger) Then
                    ' The underlying socket was closed before we were able to read the whole data.
                    Return
                End If

                ' Read the string.
                Dim stringLength As UInteger = reader.ReadUInt32()
                Dim actualStringLength As UInteger = Await reader.LoadAsync(stringLength)
                If stringLength <> actualStringLength Then
                    ' The underlying socket was closed before we were able to read the whole data.
                    Return
                End If

                ' Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                ' the text back to the UI thread.
                NotifyUserFromAsyncThread(String.Format("Received data: ""{0}""", reader.ReadString(actualStringLength)), NotifyType.StatusMessage)
            Loop
        Catch exception As Exception
            ' If this is an unknown status it means that the error is fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            NotifyUserFromAsyncThread("Read stream failed with error: " & exception.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub NotifyUserFromAsyncThread(ByVal strMessage As String, ByVal type As NotifyType)
        Dim ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser(strMessage, type))
    End Sub

    ''' <summary>
    ''' Populates the NetworkAdapter list
    ''' </summary>
    Private Sub PopulateAdapterList()
        localHostItems.Clear()
        AdapterList.ItemsSource = localHostItems
        AdapterList.DisplayMemberPath = "DisplayString"

        For Each localHostInfo As HostName In NetworkInformation.GetHostNames()
            If localHostInfo.IPInformation IsNot Nothing Then
                Dim adapterItem As New LocalHostItem(localHostInfo)
                localHostItems.Add(adapterItem)
            End If
        Next localHostInfo
    End Sub
End Class

''' <summary>
''' Helper class describing a NetworkAdapter and its associated IP address
''' </summary>
Friend Class LocalHostItem
    Private privateDisplayString As String
    Public Property DisplayString() As String
        Get
            Return privateDisplayString
        End Get
        Private Set(ByVal value As String)
            privateDisplayString = value
        End Set
    End Property

    Private privateLocalHost As HostName
    Public Property LocalHost() As HostName
        Get
            Return privateLocalHost
        End Get
        Private Set(ByVal value As HostName)
            privateLocalHost = value
        End Set
    End Property

    Public Sub New(ByVal localHostName As HostName)
        If localHostName Is Nothing Then
            Throw New ArgumentNullException("localHostName")
        End If

        If localHostName.IPInformation Is Nothing Then
            Throw New ArgumentException("Adapter information not found")
        End If

        Me.LocalHost = localHostName
        Me.DisplayString = "Address: " & localHostName.DisplayName.ToString() & " Adapter: " & localHostName.IPInformation.NetworkAdapter.NetworkAdapterId.ToString()
    End Sub
End Class

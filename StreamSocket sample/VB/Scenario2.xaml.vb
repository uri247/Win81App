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
Imports Windows.ApplicationModel.Core
Imports Windows.Networking
Imports Windows.Networking.Connectivity
Imports Windows.Networking.Sockets

''' <summary>
''' A page for second scenario.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' Limit traffic to the same adapter that the listener is using to demonstrate client adapter-binding.
    Private adapter As NetworkAdapter = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Make sure we're using the correct server address if an adapter was selected in scenario 1.
        Dim serverAddress As Object = Nothing
        If CoreApplication.Properties.TryGetValue("serverAddress", serverAddress) Then
            If TypeOf serverAddress Is String Then
                HostNameForConnect.Text = TryCast(serverAddress, String)
            End If
        End If

        adapter = Nothing
        Dim networkAdapter As Object = Nothing
        If CoreApplication.Properties.TryGetValue("adapter", networkAdapter) Then
            adapter = TryCast(networkAdapter, NetworkAdapter)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'ConnectSocket' button.
    ''' </summary>
    ''' <param name="sender">Object for which the event was generated.</param>
    ''' <param name="e">Event's parameters.</param>
    Private Async Sub ConnectSocket_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If CoreApplication.Properties.ContainsKey("clientSocket") Then
            rootPage.NotifyUser("This step has already been executed. Please move to the next one.", NotifyType.ErrorMessage)
            Return
        End If

        If String.IsNullOrEmpty(ServiceNameForConnect.Text) Then
            rootPage.NotifyUser("Please provide a service name.", NotifyType.ErrorMessage)
            Return
        End If

        ' By default 'HostNameForConnect' is disabled and host name validation is not required. When enabling the
        ' text box validating the host name is required since it was received from an untrusted source 
        ' (user input). The host name is validated by catching ArgumentExceptions thrown by the HostName 
        ' constructor for invalid input.
        ' Note that when enabling the text box users may provide names for hosts on the Internet that require the
        ' "Internet (Client)" capability.
        Dim hostName As HostName
        Try
            hostName = New HostName(HostNameForConnect.Text)
        Catch e1 As ArgumentException
            rootPage.NotifyUser("Error: Invalid host name.", NotifyType.ErrorMessage)
            Return
        End Try

        Dim socket As New Sockets.StreamSocket()

        ' Save the socket, so subsequent steps can use it.
        CoreApplication.Properties.Add("clientSocket", socket)
        Try
            If adapter Is Nothing Then
                rootPage.NotifyUser("Connecting to: " & HostNameForConnect.Text, NotifyType.StatusMessage)

                ' Connect to the server (in our case the listener we created in previous step).
                Await socket.ConnectAsync(hostName, ServiceNameForConnect.Text)

                rootPage.NotifyUser("Connected", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Connecting to: " & HostNameForConnect.Text & " using network adapter " & adapter.NetworkAdapterId.ToString(), NotifyType.StatusMessage)

                ' Connect to the server (in our case the listener we created in previous step)
                ' limiting traffic to the same adapter that the user specified in the previous step.
                ' This option will be overriden by interfaces with weak-host or forwarding modes enabled.
                Await socket.ConnectAsync(hostName, ServiceNameForConnect.Text, SocketProtectionLevel.PlainSocket, adapter)

                rootPage.NotifyUser("Connected using network adapter " & adapter.NetworkAdapterId.ToString(), NotifyType.StatusMessage)
            End If

            ' Mark the socket as connected. Set the value to null, as we care only about the fact that the 
            ' property is set.
            CoreApplication.Properties.Add("connected", Nothing)
        Catch exception As Exception
            ' If this is an unknown status it means that the error is fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            rootPage.NotifyUser("Connect failed with error: " & exception.Message, NotifyType.ErrorMessage)
        End Try
    End Sub
End Class

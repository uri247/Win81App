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
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports Windows.Web

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private messageWebSocket As MessageWebSocket
    Private messageWriter As DataWriter

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Async Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If String.IsNullOrEmpty(InputField.Text) Then
            rootPage.NotifyUser("Please specify text to send", NotifyType.ErrorMessage)
            Return
        End If

        Dim connecting As Boolean = True
        Try
            ' Have we connected yet?
            If messageWebSocket Is Nothing Then
                ' By default 'ServerAddressField' is disabled and URI validation is not required. When enabling the
                ' text box validating the URI is required since it was received from an untrusted source (user
                ' input). 
                ' The URI is validated by calling TryGetUri() that will return 'false' for strings that are not
                ' valid WebSocket URIs.
                ' Note that when enabling the text box users may provide URIs to machines on the intrAnet 
                ' or intErnet. In these cases the app requires the "Home or Work Networking" or 
                ' "Internet (Client)" capability respectively.
                Dim server As Uri = Nothing
                If Not rootPage.TryGetUri(ServerAddressField.Text, server) Then
                    Return
                End If

                rootPage.NotifyUser("Connecting to: " & server.ToString(), NotifyType.StatusMessage)

                messageWebSocket = New MessageWebSocket()
                messageWebSocket.Control.MessageType = SocketMessageType.Utf8
                AddHandler messageWebSocket.MessageReceived, AddressOf MessageReceived

                ' Dispatch close event on UI thread. This allows us to avoid synchronizing access to messageWebSocket.
                AddHandler messageWebSocket.Closed, Async Sub(senderSocket, args) Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() Closed(senderSocket, args))

                Await messageWebSocket.ConnectAsync(server)
                messageWriter = New DataWriter(messageWebSocket.OutputStream)

                rootPage.NotifyUser("Connected", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Already connected", NotifyType.StatusMessage)
            End If

            connecting = False
            Dim message As String = InputField.Text
            OutputField.Text &= "Sending Message:" & vbCrLf & message & vbCrLf

            ' Buffer any data we want to send.
            messageWriter.WriteString(message)

            ' Send the data as one complete message.
            Await messageWriter.StoreAsync()

            rootPage.NotifyUser("Send Complete", NotifyType.StatusMessage)
        Catch ex As Exception ' For debugging
            ' Error happened during connect operation.
            If connecting AndAlso messageWebSocket IsNot Nothing Then
                messageWebSocket.Dispose()
                messageWebSocket = Nothing
            End If

            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            Select Case status
                Case WebErrorStatus.CannotConnect, WebErrorStatus.NotFound, WebErrorStatus.RequestTimeout
                    rootPage.NotifyUser("Cannot connect to the server. Please make sure " & "to run the server setup script before running the sample.", NotifyType.ErrorMessage)

                Case WebErrorStatus.Unknown
                    Throw

                Case Else
                    rootPage.NotifyUser("Error: " & status, NotifyType.ErrorMessage)
            End Select

            OutputField.Text += ex.Message & vbCrLf
        End Try
    End Sub

    Private Sub MessageReceived(ByVal sender As MessageWebSocket, ByVal args As MessageWebSocketMessageReceivedEventArgs)
        Try
            MarshalText(OutputField, "Message Received; Type: " & args.MessageType & vbCrLf)
            Using reader As DataReader = args.GetDataReader()
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8

                Dim read As String = reader.ReadString(reader.UnconsumedBufferLength)
                MarshalText(OutputField, read & vbCrLf)
            End Using
        Catch ex As Exception ' For debugging
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            If status = WebErrorStatus.Unknown Then
                Throw
            End If

            ' Normally we'd use the status to test for specific conditions we want to handle specially,
            ' and only use ex.Message for display purposes.  In this sample, we'll just output the
            ' status for debugging here, but still use ex.Message below.
            MarshalText(OutputField, "Error: " & status & vbCrLf)

            MarshalText(OutputField, ex.Message & vbCrLf)
        End Try
    End Sub

    Private Sub Close_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            If messageWebSocket IsNot Nothing Then
                rootPage.NotifyUser("Closing", NotifyType.StatusMessage)
                messageWebSocket.Close(1000, "Closed due to user request.")
                messageWebSocket = Nothing
            Else
                rootPage.NotifyUser("No active WebSocket, send something first", NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            If status = WebErrorStatus.Unknown Then
                Throw
            End If

            ' Normally we'd use the status to test for specific conditions we want to handle specially,
            ' and only use ex.Message for display purposes.  In this sample, we'll just output the
            ' status for debugging here, but still use ex.Message below.
            rootPage.NotifyUser("Error: " & status, NotifyType.ErrorMessage)

            OutputField.Text += ex.Message & vbCrLf
        End Try
    End Sub

    ' This may be triggered remotely by the server or locally by Close/Dispose()
    Private Sub Closed(ByVal sender As IWebSocket, ByVal args As WebSocketClosedEventArgs)
        MarshalText(OutputField, "Closed; Code: " & args.Code & ", Reason: " & args.Reason & vbCrLf)

        If messageWebSocket IsNot Nothing Then
            messageWebSocket.Dispose()
            messageWebSocket = Nothing
        End If
    End Sub

    Private Sub MarshalText(ByVal output As TextBox, ByVal value As String)
        MarshalText(output, value, True)
    End Sub

    ' When operations happen on a background thread we have to marshal UI updates back to the UI thread.
    Private Sub MarshalText(ByVal output As TextBox, ByVal value As String, ByVal append As Boolean)
        Dim ignore = output.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                                   If append Then
                                                                                                       output.Text += value
                                                                                                   Else
                                                                                                       output.Text = value
                                                                                                   End If
                                                                                               End Sub)
    End Sub
End Class

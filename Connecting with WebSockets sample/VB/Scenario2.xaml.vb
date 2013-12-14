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
Imports System.Runtime.InteropServices.WindowsRuntime
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports Windows.Web

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private streamWebSocket As StreamWebSocket
    Private readBuffer() As Byte

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
        ' Have we connected yet?
        If streamWebSocket IsNot Nothing Then
            rootPage.NotifyUser("Already connected", NotifyType.StatusMessage)
            Return
        End If

        ' By default 'ServerAddressField' is disabled and URI validation is not required. When enabling the
        ' text box validating the URI is required since it was received from an untrusted source (user input).
        ' The URI is validated by calling TryGetUri() that will return 'false' for strings that are not
        ' valid WebSocket URIs.
        ' Note that when enabling the text box users may provide URIs to machines on the intrAnet
        ' or intErnet. In these cases the app requires the "Home or Work Networking" or
        ' "Internet (Client)" capability respectively.
        Dim server As Uri = Nothing
        If Not rootPage.TryGetUri(ServerAddressField.Text, server) Then
            Return
        End If

        Try
            rootPage.NotifyUser("Connecting to: " & server.ToString(), NotifyType.StatusMessage)

            streamWebSocket = New StreamWebSocket()

            ' Dispatch close event on UI thread. This allows us to avoid synchronizing access to streamWebSocket.
            AddHandler streamWebSocket.Closed, Async Sub(senderSocket, args) Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() Closed(senderSocket, args))

            Await streamWebSocket.ConnectAsync(server)

            readBuffer = New Byte(999) {}

            ' Start a background task to continuously read for incoming data
            Dim receiving As Task = Task.Factory.StartNew(AddressOf Scenario2ReceiveData, streamWebSocket.InputStream.AsStreamForRead(), TaskCreationOptions.LongRunning)

            ' Start a background task to continuously write outgoing data
            Dim sending As Task = Task.Factory.StartNew(AddressOf Scenario2SendData, streamWebSocket.OutputStream, TaskCreationOptions.LongRunning)

            rootPage.NotifyUser("Connected", NotifyType.StatusMessage)
        Catch ex As Exception ' For debugging
            If streamWebSocket IsNot Nothing Then
                streamWebSocket.Dispose()
                streamWebSocket = Nothing
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

    ' Continuously write outgoing data. For writing data we'll show how to use data.AsBuffer() to get an
    ' IBuffer for use with webSocket.OutputStream.WriteAsync.  Alternatively you can call
    ' webSocket.OutputStream.AsStreamForWrite() to use .NET streams.
    Private Async Sub Scenario2SendData(ByVal state As Object)
        Dim dataSent As Integer = 0
        Dim data() As Byte = {&H0, &H1, &H2, &H3, &H4, &H5, &H6, &H7, &H8, &H9}

        MarshalText(OutputField, "Background sending data in " & data.Length & " byte chunks each second." & vbCrLf)

        Try
            Dim writeStream As IOutputStream = CType(state, IOutputStream)

            ' Send until the socket gets closed/stopped
            Do
                ' using System.Runtime.InteropServices.WindowsRuntime;
                Await writeStream.WriteAsync(data.AsBuffer())

                dataSent += data.Length
                MarshalText(DataSentField, dataSent.ToString(), False)

                ' Delay so the user can watch what's going on.
                Await Task.Delay(TimeSpan.FromSeconds(1))
            Loop
        Catch e1 As ObjectDisposedException
            MarshalText(OutputField, "Background write stopped." & vbCrLf)
        Catch ex As Exception
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            Select Case status
                Case WebErrorStatus.OperationCanceled
                    MarshalText(OutputField, "Background write canceled." & vbCrLf)

                Case WebErrorStatus.Unknown
                    Throw

                Case Else
                    MarshalText(OutputField, "Error: " & status & vbCrLf)
                    MarshalText(OutputField, ex.Message & vbCrLf)
            End Select
        End Try
    End Sub

    ' Continuously read incoming data. For reading data we'll show how to use webSocket.InputStream.AsStream()
    ' to get a .NET stream. Alternatively you could call readBuffer.AsBuffer() to use IBuffer with
    ' webSocket.InputStream.ReadAsync.
    Private Async Sub Scenario2ReceiveData(ByVal state As Object)
        Dim bytesReceived As Integer = 0
        Try
            Dim readStream As Stream = CType(state, Stream)
            MarshalText(OutputField, "Background read starting." & vbCrLf)

            Do ' Until closed and ReadAsync fails.
                Dim read As Integer = Await readStream.ReadAsync(readBuffer, 0, readBuffer.Length)
                bytesReceived += read
                MarshalText(DataReceivedField, bytesReceived.ToString(), False)

                ' Do something with the data.
            Loop
        Catch e1 As ObjectDisposedException
            MarshalText(OutputField, "Background read stopped." & vbCrLf)
        Catch ex As Exception
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            Select Case status
                Case WebErrorStatus.OperationCanceled
                    MarshalText(OutputField, "Background write canceled." & vbCrLf)

                Case WebErrorStatus.Unknown
                    Throw

                Case Else
                    MarshalText(OutputField, "Error: " & status & vbCrLf)
                    MarshalText(OutputField, ex.Message & vbCrLf)
            End Select
        End Try
    End Sub

    Private Sub Stop_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            If streamWebSocket IsNot Nothing Then
                rootPage.NotifyUser("Stopping", NotifyType.StatusMessage)
                streamWebSocket.Close(1000, "Closed due to user request.")
                streamWebSocket = Nothing
            Else
                rootPage.NotifyUser("There is no active socket to stop.", NotifyType.StatusMessage)
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

        If streamWebSocket IsNot Nothing Then
            streamWebSocket.Dispose()
            streamWebSocket = Nothing
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

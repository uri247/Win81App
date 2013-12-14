'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams

Public Class BufferConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal language As String) As Object Implements IValueConverter.Convert
        Dim metadata As String = String.Empty
        Dim buffer As IBuffer = TryCast(value, IBuffer)
        If buffer IsNot Nothing Then
            Using metadataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer)
                metadata = metadataReader.ReadString(buffer.Length)
            End Using
            metadata = String.Format("({0})", metadata)
        End If
        Return metadata
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal language As String) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

' This class encapsulates a Peer.
Public Class ConnectedPeer
    Public _socket As StreamSocket
    Public _socketClosed As Boolean
    Public _dataWriter As DataWriter

    Public Sub New(ByVal socket As StreamSocket, ByVal socketClosed As Boolean, ByVal dataWriter As DataWriter)
        _socket = socket
        _socketClosed = socketClosed
        _dataWriter = dataWriter
    End Sub
End Class

Public Class SocketEventArgs
    Inherits EventArgs

    Public Sub New(ByVal s As String)
        msg = s
    End Sub
    Private msg As String
    Public ReadOnly Property Message() As String
        Get
            Return msg
        End Get
    End Property
End Class

Public Class MessageEventArgs
    Inherits EventArgs

    Public Sub New(ByVal s As String)
        msg = s
    End Sub
    Private msg As String
    Public ReadOnly Property Message() As String
        Get
            Return msg
        End Get
    End Property
End Class

Public Enum ConnectState
    PeerFound
    Listening
    Connecting
    Completed
    Canceled
    Failed
End Enum

Friend Class SocketHelper
    Private _connectedPeers As New List(Of ConnectedPeer)()

    Public Event RaiseSocketErrorEvent As EventHandler(Of SocketEventArgs)
    Public Event RaiseMessageEvent As EventHandler(Of MessageEventArgs)

    Public ReadOnly Property ConnectedPeers() As ReadOnlyCollection(Of ConnectedPeer)
        Get
            Return New ReadOnlyCollection(Of ConnectedPeer)(_connectedPeers)
        End Get
    End Property

    Public Sub Add(ByVal p As ConnectedPeer)
        _connectedPeers.Add(p)
    End Sub

    ' Send a message through a specific dataWriter
    Public Async Sub SendMessageToPeer(ByVal message As String, ByVal connectedPeer As ConnectedPeer)
        Try
            If Not connectedPeer._socketClosed Then
                Dim dataWriter As DataWriter = connectedPeer._dataWriter

                Dim msgLength As UInteger = dataWriter.MeasureString(message)
                dataWriter.WriteInt32(CInt(msgLength))
                dataWriter.WriteString(message)

                Dim numBytesWritten As UInteger = Await dataWriter.StoreAsync()
                If numBytesWritten > 0 Then
                    OnRaiseMessageEvent(New MessageEventArgs("Sent message: " & message & ", number of bytes written: " & numBytesWritten))
                Else
                    OnRaiseSocketErrorEvent(New SocketEventArgs("The remote side closed the socket"))
                End If
            End If
        Catch err As Exception
            If Not connectedPeer._socketClosed Then
                OnRaiseSocketErrorEvent(New SocketEventArgs("Failed to send message with error: " & err.Message))
            End If
        End Try
    End Sub

    Public Async Sub StartReader(ByVal connectedPeer As ConnectedPeer)
        Try
            Using socketReader = New Windows.Storage.Streams.DataReader(connectedPeer._socket.InputStream)
                ' Read the message sent by the remote peer
                Dim bytesRead As UInteger = Await socketReader.LoadAsync(CUInt(Marshal.SizeOf(New UInteger)))
                If bytesRead > 0 Then
                    Dim strLength As UInteger = CUInt(socketReader.ReadUInt32())
                    bytesRead = Await socketReader.LoadAsync(strLength)
                    If bytesRead > 0 Then
                        Dim message As String = socketReader.ReadString(strLength)
                        OnRaiseMessageEvent(New MessageEventArgs("Got message: " & message))
                        StartReader(connectedPeer) ' Start another reader
                    Else
                        OnRaiseSocketErrorEvent(New SocketEventArgs("The remote side closed the socket"))
                    End If
                Else
                    OnRaiseSocketErrorEvent(New SocketEventArgs("The remote side closed the socket"))
                End If

                socketReader.DetachStream()
            End Using

        Catch e As Exception
            If Not connectedPeer._socketClosed Then
                OnRaiseSocketErrorEvent(New SocketEventArgs("Reading from socket failed: " & e.Message))
            End If
        End Try
    End Sub

    Public Sub CloseSocket()
        ' Close all the established sockets.
        For Each obj As ConnectedPeer In _connectedPeers
            If obj._socket IsNot Nothing Then
                obj._socketClosed = True
                obj._socket.Dispose()
                obj._socket = Nothing
            End If

            If obj._dataWriter IsNot Nothing Then
                obj._dataWriter.Dispose()
                obj._dataWriter = Nothing
            End If
        Next obj

        _connectedPeers.Clear()
    End Sub

    Protected Overridable Sub OnRaiseSocketErrorEvent(ByVal e As SocketEventArgs)
        ' Make a temporary copy of the event to avoid possibility of 
        ' a race condition if the last subscriber unsubscribes 
        ' immediately after the null check and before the event is raised.
        Dim handler As EventHandler(Of SocketEventArgs) = RaiseSocketErrorEventEvent

        ' Event will be null if there are no subscribers 
        If handler IsNot Nothing Then
            ' Use the () operator to raise the event.
            handler(Me, e)
        End If
    End Sub

    Protected Overridable Sub OnRaiseMessageEvent(ByVal e As MessageEventArgs)
        ' Make a temporary copy of the event to avoid possibility of 
        ' a race condition if the last subscriber unsubscribes 
        ' immediately after the null check and before the event is raised.
        Dim handler As EventHandler(Of MessageEventArgs) = RaiseMessageEventEvent

        ' Event will be null if there are no subscribers 
        If handler IsNot Nothing Then
            ' Use the () operator to raise the event.
            handler(Me, e)
        End If
    End Sub

End Class


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
Imports Windows.Data.Json
Imports Windows.Storage.Streams
Imports Windows.Web.Http
Imports Windows.Web.Http.Headers

Friend Class HttpJsonContent
    Implements IHttpContent

    Private jsonValue As IJsonValue
    Private _headers As HttpContentHeaderCollection

    Public ReadOnly Property Headers() As HttpContentHeaderCollection Implements IHttpContent.Headers
        Get
            Return _headers
        End Get
    End Property

    Public Sub New(ByVal jsonValue As IJsonValue)
        If jsonValue Is Nothing Then
            Throw New ArgumentException("jsonValue cannot be null.")
        End If
        Me.jsonValue = jsonValue
        _headers = New HttpContentHeaderCollection()
        _headers.ContentType = New HttpMediaTypeHeaderValue("application/json")
        _headers.ContentType.CharSet = "UTF-8"
    End Sub

    Public Function BufferAllAsync() As IAsyncOperationWithProgress(Of ULong, ULong) Implements IHttpContent.BufferAllAsync
        Return AsyncInfo.Run(Of ULong, ULong)(Function(cancellationToken, progress)
                                                  ' Report progress.
                                                  ' Just return the size in bytes.
                                                  Return Task(Of ULong).Run(Function()
                                                                                Dim length As ULong = GetLength()
                                                                                progress.Report(length)
                                                                                Return length
                                                                            End Function)
                                              End Function)
    End Function

    Public Function ReadAsBufferAsync() As IAsyncOperationWithProgress(Of IBuffer, ULong) Implements IHttpContent.ReadAsBufferAsync
        Return AsyncInfo.Run(Of IBuffer, ULong)(Function(cancellationToken, progress)
                                                    ' Make sure that the DataWriter destructor does not free the buffer.
                                                    ' Report progress.
                                                    Return Task(Of IBuffer).Run(Function()
                                                                                    Dim writer As New DataWriter()
                                                                                    writer.WriteString(jsonValue.Stringify())
                                                                                    Dim buffer As IBuffer = writer.DetachBuffer()
                                                                                    progress.Report(buffer.Length)
                                                                                    Return buffer
                                                                                End Function)
                                                End Function)
    End Function

    Public Function ReadAsInputStreamAsync() As IAsyncOperationWithProgress(Of IInputStream, ULong) Implements IHttpContent.ReadAsInputStreamAsync
        Return AsyncInfo.Run(Of IInputStream, ULong)(Async Function(cancellationToken, progress)
                                                         ' Make sure that the DataWriter destructor does not close the stream.
                                                         ' Report progress.
                                                         Dim randomAccessStream As New InMemoryRandomAccessStream()
                                                         Dim writer As New DataWriter(randomAccessStream)
                                                         writer.WriteString(jsonValue.Stringify())
                                                         Dim bytesStored As UInteger = Await writer.StoreAsync().AsTask(cancellationToken)
                                                         writer.DetachStream()
                                                         progress.Report(randomAccessStream.Size)
                                                         Return randomAccessStream.GetInputStreamAt(0)
                                                     End Function)
    End Function

    Public Function ReadAsStringAsync() As IAsyncOperationWithProgress(Of String, ULong) Implements IHttpContent.ReadAsStringAsync
        Return AsyncInfo.Run(Of String, ULong)(Function(cancellationToken, progress)
                                                   ' Report progress (length of string).
                                                   Return Task(Of String).Run(Function()
                                                                                  Dim jsonString As String = jsonValue.Stringify()
                                                                                  progress.Report(CULng(jsonString.Length))
                                                                                  Return jsonString
                                                                              End Function)
                                               End Function)
    End Function

    Public Function TryComputeLength(<System.Runtime.InteropServices.Out()> ByRef length As ULong) As Boolean Implements IHttpContent.TryComputeLength
        length = GetLength()
        Return True
    End Function

    Public Function WriteToStreamAsync(ByVal outputStream As IOutputStream) As IAsyncOperationWithProgress(Of ULong, ULong) Implements IHttpContent.WriteToStreamAsync
        Return AsyncInfo.Run(Of ULong, ULong)(Async Function(cancellationToken, progress)
                                                  ' Make sure that DataWriter destructor does not close the stream.
                                                  ' Report progress.
                                                  Dim writer As New DataWriter(outputStream)
                                                  writer.WriteString(jsonValue.Stringify())
                                                  Dim bytesWritten As UInteger = Await writer.StoreAsync().AsTask(cancellationToken)
                                                  writer.DetachStream()
                                                  progress.Report(bytesWritten)
                                                  Return bytesWritten
                                              End Function)
    End Function

    Public Sub Dispose() Implements IHttpContent.Dispose
    End Sub

    Private Function GetLength() As ULong
        Dim writer As New DataWriter()
        writer.WriteString(jsonValue.Stringify())

        Dim buffer As IBuffer = writer.DetachBuffer()
        Return buffer.Length
    End Function
End Class

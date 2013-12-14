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
Imports Windows.Storage.Streams

Friend Class SlowInputStream
    Implements IInputStream

    Private length As UInteger
    Private position As UInteger

    Public Sub New(ByVal length As UInteger)
        Me.length = length
        position = 0
    End Sub

    Public Function ReadAsync(ByVal buffer As IBuffer, ByVal count As UInteger, ByVal options As InputStreamOptions) As IAsyncOperationWithProgress(Of IBuffer, UInteger) Implements IInputStream.ReadAsync
        Return AsyncInfo.Run(Of IBuffer, UInteger)(Async Function(cancellationToken, progress)
                                                       ' Introduce a 1 second delay.
                                                       If length - position < count Then
                                                           count = length - position
                                                       End If
                                                       Dim data(CInt(count - 1)) As Byte
                                                       For i As Integer = 0 To CInt(count - 1)
                                                           data(i) = 64
                                                       Next i
                                                       Await Task.Delay(1000)
                                                       position += count
                                                       progress.Report(count)
                                                       Return data.AsBuffer()
                                                   End Function)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub
End Class

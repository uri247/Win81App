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
Imports Windows.Web.Http
Imports Windows.Web.Http.Filters

Public Class PlugInFilter
    Implements IHttpFilter

    Private innerFilter As IHttpFilter

    Public Sub New(ByVal innerFilter As IHttpFilter)
        If innerFilter Is Nothing Then
            Throw New ArgumentException("innerFilter cannot be null.")
        End If
        Me.innerFilter = innerFilter
    End Sub

    Public Function SendRequestAsync(ByVal request As HttpRequestMessage) As IAsyncOperationWithProgress(Of HttpResponseMessage, HttpProgress) Implements IHttpFilter.SendRequestAsync
        Return AsyncInfo.Run(Of HttpResponseMessage, HttpProgress)(Async Function(cancellationToken, progress)
                                                                       request.Headers.Add("Custom-Header", "CustomRequestValue")
                                                                       Dim response As HttpResponseMessage = Await innerFilter.SendRequestAsync(request).AsTask(cancellationToken, progress)
                                                                       cancellationToken.ThrowIfCancellationRequested()
                                                                       response.Headers.Add("Custom-Header", "CustomResponseValue")
                                                                       Return response
                                                                   End Function)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        innerFilter.Dispose()
        GC.SuppressFinalize(Me)
    End Sub
End Class

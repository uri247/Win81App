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
Imports System.Text
Imports Windows.Web.Http
Imports Windows.Web.Http.Filters
Imports Windows.Web.Http.Headers

Friend NotInheritable Class Helpers

    Private Sub New()
    End Sub

    Friend Shared Async Function DisplayTextResultAsync(ByVal response As HttpResponseMessage, ByVal output As TextBox, ByVal token As CancellationToken) As Task
        Dim responseBodyAsText As String
        output.Text += SerializeHeaders(response)
        responseBodyAsText = Await response.Content.ReadAsStringAsync().AsTask(token)

        token.ThrowIfCancellationRequested()

        ' Insert new lines.
        responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine)

        output.Text += responseBodyAsText
    End Function

    Friend Shared Function SerializeHeaders(ByVal response As HttpResponseMessage) As String
        Dim output As New StringBuilder()

        ' We cast the StatusCode to an int so we display the numeric value (e.g., "200") rather than the
        ' name of the enum (e.g., "OK") which would often be redundant with the ReasonPhrase.
        output.Append((CInt(response.StatusCode)) & " " & response.ReasonPhrase & vbCrLf)

        SerializeHeaderCollection(response.Headers, output)
        SerializeHeaderCollection(response.Content.Headers, output)
        output.Append(vbCrLf)
        Return output.ToString()
    End Function

    Friend Shared Sub SerializeHeaderCollection(ByVal headers As IEnumerable(Of KeyValuePair(Of String, String)), ByVal output As StringBuilder)
        For Each header In headers
            output.Append(header.Key & ": " & header.Value & vbCrLf)
        Next header
    End Sub

    Friend Shared Sub CreateHttpClient(ByRef httpClient As HttpClient)
        If httpClient IsNot Nothing Then
            httpClient.Dispose()
        End If

        ' HttpClient functionality can be extended by plugging multiple filters together and providing
        ' HttpClient with the configured filter pipeline.
        Dim filter As IHttpFilter = New HttpBaseProtocolFilter()
        filter = New PlugInFilter(filter) ' Adds a custom header to every request and response message.
        httpClient = New HttpClient(filter)

        ' The following line sets a "User-Agent" request header as a default header on the HttpClient instance.
        ' Default headers will be sent with every request sent from this HttpClient instance.
        httpClient.DefaultRequestHeaders.UserAgent.Add(New HttpProductInfoHeaderValue("Sample", "v8"))
    End Sub

    Friend Shared Sub ScenarioStarted(ByVal startButton As Button, ByVal cancelButton As Button, ByVal outputField As TextBox)
        startButton.IsEnabled = False
        cancelButton.IsEnabled = True
        If outputField IsNot Nothing Then
            outputField.Text = String.Empty
        End If
    End Sub

    Friend Shared Sub ScenarioCompleted(ByVal startButton As Button, ByVal cancelButton As Button)
        startButton.IsEnabled = True
        cancelButton.IsEnabled = False
    End Sub

    Friend Shared Sub ReplaceQueryString(ByVal addressField As TextBox, ByVal newQueryString As String)
        Dim resourceAddress As String = addressField.Text

        ' Remove previous query string.
        Dim questionMarkIndex As Integer = resourceAddress.IndexOf("?", StringComparison.Ordinal)
        If questionMarkIndex <> -1 Then
            resourceAddress = resourceAddress.Substring(0, questionMarkIndex)
        End If

        addressField.Text = resourceAddress & newQueryString
    End Sub
End Class

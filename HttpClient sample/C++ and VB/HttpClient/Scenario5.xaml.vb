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
Imports Windows.Storage.Streams
Imports Windows.Web.Http

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private httpClient As HttpClient
    Private cts As CancellationTokenSource

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        Helpers.CreateHttpClient(httpClient)
        cts = New CancellationTokenSource()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dispose()
    End Sub

    Private Async Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Helpers.ScenarioStarted(StartButton, CancelButton, OutputField)
        rootPage.NotifyUser("In progress", NotifyType.StatusMessage)

        Try
            Const contentLength As Integer = 1000
            Dim stream As Stream = GenerateSampleStream(contentLength)
            Dim streamContent As New HttpStreamContent(stream.AsInputStream())

            ' 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
            ' text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
            Dim resourceAddress As New Uri(AddressField.Text)

            Dim request As New HttpRequestMessage(HttpMethod.Post, resourceAddress)
            request.Content = streamContent

            ' Do an asynchronous POST.
            Dim response As HttpResponseMessage = Await httpClient.SendRequestAsync(request).AsTask(cts.Token)

            Await Helpers.DisplayTextResultAsync(response, OutputField, cts.Token)

            rootPage.NotifyUser("Completed", NotifyType.StatusMessage)
        Catch e1 As TaskCanceledException
            rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage)
        Catch ex As Exception
            rootPage.NotifyUser("Error: " & ex.Message, NotifyType.ErrorMessage)
        Finally
            Helpers.ScenarioCompleted(StartButton, CancelButton)
        End Try
    End Sub

    Private Sub Cancel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        cts.Cancel()
        cts.Dispose()

        ' Re-create the CancellationTokenSource.
        cts = New CancellationTokenSource()
    End Sub

    Private Shared Function GenerateSampleStream(ByVal size As Integer) As MemoryStream
        ' Generate sample data.
        Dim subData(size - 1) As Byte
        For i As Integer = 0 To subData.Length - 1
            subData(i) = 64 ' '@'
        Next i

        Return New MemoryStream(subData)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If httpClient IsNot Nothing Then
            httpClient.Dispose()
            httpClient = Nothing
        End If

        If cts IsNot Nothing Then
            cts.Dispose()
            cts = Nothing
        End If
    End Sub
End Class

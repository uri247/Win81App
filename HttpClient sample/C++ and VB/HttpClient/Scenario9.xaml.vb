'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Web.Http
Imports Windows.Web.Http.Filters

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario9
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

    Private Sub GetCookies_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            ' 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
            ' text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
            Dim uri As New Uri(AddressField.Text)

            Dim filter As New HttpBaseProtocolFilter()
            Dim cookieCollection As HttpCookieCollection = filter.CookieManager.GetCookies(uri)

            OutputField.Text = cookieCollection.Count & " cookies found." & vbCrLf
            For Each cookie As HttpCookie In cookieCollection
                OutputField.Text &= "--------------------" & vbCrLf
                OutputField.Text &= "Name: " & cookie.Name & vbCrLf
                OutputField.Text &= "Domain: " & cookie.Domain & vbCrLf
                OutputField.Text &= "Path: " & cookie.Path & vbCrLf
                OutputField.Text &= "Value: " & cookie.Value & vbCrLf
                OutputField.Text &= "Expires: " & cookie.Expires.ToString() & vbCrLf
                OutputField.Text &= "Secure: " & cookie.Secure & vbCrLf
                OutputField.Text &= "HttpOnly: " & cookie.HttpOnly & vbCrLf
            Next cookie
        Catch ex As Exception
            rootPage.NotifyUser("Error: " & ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Async Sub SendHttpGetButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Helpers.ScenarioStarted(SendHttpGetButton, CancelButton, OutputField)
        rootPage.NotifyUser("In progress", NotifyType.StatusMessage)

        Try
            ' 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
            ' text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
            Dim resourceAddress As New Uri(AddressField.Text)

            Dim response As HttpResponseMessage = Await httpClient.GetAsync(resourceAddress).AsTask(cts.Token)

            Await Helpers.DisplayTextResultAsync(response, OutputField, cts.Token)

            rootPage.NotifyUser("Completed", NotifyType.StatusMessage)
        Catch e1 As TaskCanceledException
            rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage)
        Catch ex As Exception
            rootPage.NotifyUser("Error: " & ex.Message, NotifyType.ErrorMessage)
        Finally
            Helpers.ScenarioCompleted(SendHttpGetButton, CancelButton)
        End Try
    End Sub

    Private Sub Cancel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        cts.Cancel()
        cts.Dispose()

        ' Re-create the CancellationTokenSource.
        cts = New CancellationTokenSource()
    End Sub

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

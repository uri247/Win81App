'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports HttpFilters
Imports Windows.Web.Http
Imports Windows.Web.Http.Filters

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario13
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
        Dim baseProtocolFilter As New HttpBaseProtocolFilter()
        Dim retryFilter As New HttpRetryFilter(baseProtocolFilter)
        httpClient = New HttpClient(retryFilter)
        cts = New CancellationTokenSource()
        UpdateAddressField()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dispose()
    End Sub

    Private Sub UpdateAddressField()
        ' Tell the server which type of Retry-After header we would like to receive.
        Dim queryString As String = ""
        If RetryAfterSwitch.IsOn Then
            queryString &= "?retryAfter=deltaSeconds"
        Else
            queryString &= "?retryAfter=httpDate"
        End If

        Helpers.ReplaceQueryString(AddressField, queryString)
    End Sub

    Private Sub RetryAfterSwitch_Toggled(ByVal sender As Object, ByVal e As RoutedEventArgs)
        UpdateAddressField()
    End Sub

    Private Async Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Helpers.ScenarioStarted(StartButton, CancelButton, OutputField)
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
            Helpers.ScenarioCompleted(StartButton, CancelButton)
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

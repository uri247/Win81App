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
Imports Windows.Web.Http
Imports Windows.Web.Http.Filters

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    ' A pointer back to the main page. This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private filter As HttpBaseProtocolFilter
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
        ' In this scenario we just create an HttpClient instance with default settings. I.e. no custom filters. 
        ' For examples on how to use custom filters see other scenarios.
        filter = New HttpBaseProtocolFilter()
        httpClient = New HttpClient(filter)
        cts = New CancellationTokenSource()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dispose()
    End Sub

    Private Async Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' The value of 'AddressField' is set by the user and is therefore untrusted input. If we can't create a
        ' valid, absolute URI, we'll notify the user about the incorrect input.
        ' Note that this app has both "Internet (Client)" and "Home and Work Networking" capabilities set,
        ' since the user may provide URIs for servers located on the intErnet or intrAnet. If apps only
        ' communicate with servers on the intErnet, only the "Internet (Client)" capability should be set.
        ' Similarly if an app is only intended to communicate on the intrAnet, only the "Home and Work
        ' Networking" capability should be set.
        Dim resourceUri As Uri = Nothing
        If Not Uri.TryCreate(AddressField.Text.Trim(), UriKind.Absolute, resourceUri) Then
            rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage)
            Return
        End If

        If resourceUri.Scheme <> "http" AndAlso resourceUri.Scheme <> "https" Then
            rootPage.NotifyUser("Only 'http' and 'https' schemes supported.", NotifyType.ErrorMessage)
            Return
        End If

        Helpers.ScenarioStarted(StartButton, CancelButton, OutputField)
        rootPage.NotifyUser("In progress", NotifyType.StatusMessage)

        Try
            If ReadDefaultRadio.IsChecked.Value Then
                filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default
            ElseIf ReadMostRecentRadio.IsChecked.Value Then
                filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent
            ElseIf ReadOnlyFromCacheRadio.IsChecked.Value Then
                filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.OnlyFromCache
            End If

            If WriteDefaultRadio.IsChecked.Value Then
                filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default
            ElseIf WriteNoCacheRadio.IsChecked.Value Then
                filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache
            End If

            Dim response As HttpResponseMessage = Await httpClient.GetAsync(resourceUri).AsTask(cts.Token)

            Await Helpers.DisplayTextResultAsync(response, OutputField, cts.Token)

            rootPage.NotifyUser("Completed. Response came from " & response.Source.ToString() & ".", NotifyType.StatusMessage)
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
        If filter IsNot Nothing Then
            filter.Dispose()
            filter = Nothing
        End If

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

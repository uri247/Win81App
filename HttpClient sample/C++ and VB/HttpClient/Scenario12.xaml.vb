'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports HttpFilters
Imports Windows.UI.ApplicationSettings
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.Web.Http
Imports Windows.Web.Http.Filters

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario12
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private settingsPopup As Popup

    Private meteredConnectionFilter As HttpMeteredConnectionFilter
    Private httpClient As HttpClient
    Private cts As CancellationTokenSource

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        AddHandler SettingsPane.GetForCurrentView().CommandsRequested, AddressOf OnCommandsRequested

        Dim baseProtocolFilter As New HttpBaseProtocolFilter()
        meteredConnectionFilter = New HttpMeteredConnectionFilter(baseProtocolFilter)
        httpClient = New HttpClient(meteredConnectionFilter)
        cts = New CancellationTokenSource()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
        RemoveHandler SettingsPane.GetForCurrentView().CommandsRequested, AddressOf OnCommandsRequested
        Dispose()
    End Sub

    Private Sub OnCommandsRequested(ByVal sender As SettingsPane, ByVal args As SettingsPaneCommandsRequestedEventArgs)
        Dim settingsCommand As New SettingsCommand("filter", "Metered Connection Filter", Sub(command)
                                                                                              Const settingsWidth As Double = 400
                                                                                              settingsPopup = New Popup()
                                                                                              AddHandler settingsPopup.Closed, AddressOf OnPopupClosed
                                                                                              settingsPopup.IsLightDismissEnabled = True
                                                                                              AddHandler Window.Current.Activated, AddressOf OnWindowActivated
                                                                                              Dim pane As New MeteredConnectionFilterSettings(meteredConnectionFilter)
                                                                                              pane.Width = settingsWidth
                                                                                              pane.Height = Window.Current.Bounds.Height
                                                                                              settingsPopup.Child = pane
                                                                                              settingsPopup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - settingsWidth)
                                                                                              settingsPopup.SetValue(Canvas.TopProperty, 0)
                                                                                              settingsPopup.IsOpen = True
                                                                                          End Sub)

        args.Request.ApplicationCommands.Add(settingsCommand)
    End Sub

    Private Sub OnWindowActivated(ByVal sender As Object, ByVal e As Windows.UI.Core.WindowActivatedEventArgs)
        If e.WindowActivationState = Windows.UI.Core.CoreWindowActivationState.Deactivated Then
            settingsPopup.IsOpen = False
        End If
    End Sub

    Private Sub OnPopupClosed(ByVal sender As Object, ByVal e As Object)
        RemoveHandler Window.Current.Activated, AddressOf OnWindowActivated
    End Sub

    Private Async Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Helpers.ScenarioStarted(StartButton, CancelButton, OutputField)
        rootPage.NotifyUser("In progress", NotifyType.StatusMessage)

        Try
            ' 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
            ' text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
            Dim resourceAddress As New Uri(AddressField.Text)

            Dim request As New HttpRequestMessage(HttpMethod.Get, resourceAddress)

            Dim priority As MeteredConnectionPriority = MeteredConnectionPriority.Low
            If MediumRadio.IsChecked.Value Then
                priority = MeteredConnectionPriority.Medium
            ElseIf HighRadio.IsChecked.Value Then
                priority = MeteredConnectionPriority.High
            End If
            request.Properties("meteredConnectionPriority") = priority

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

    Public Sub Dispose() Implements IDisposable.Dispose
        If meteredConnectionFilter IsNot Nothing Then
            meteredConnectionFilter.Dispose()
            meteredConnectionFilter = Nothing
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

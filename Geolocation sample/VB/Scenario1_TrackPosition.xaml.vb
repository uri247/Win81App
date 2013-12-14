'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Devices.Geolocation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private _geolocator As Geolocator = Nothing

    Public Sub New()
        Me.InitializeComponent()

        _geolocator = New Geolocator()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        StartTrackingButton.IsEnabled = True
        StopTrackingButton.IsEnabled = False
    End Sub

    ''' <summary>
    ''' Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
    ''' </summary>
    ''' <param name="e">
    ''' Event data that can be examined by overriding code. The event data is representative
    ''' of the navigation that will unload the current Page unless canceled. The
    ''' navigation can potentially be canceled by setting e.Cancel to true.
    ''' </param>
    Protected Overrides Sub OnNavigatingFrom(ByVal e As NavigatingCancelEventArgs)
        If StopTrackingButton.IsEnabled Then
            RemoveHandler _geolocator.PositionChanged, AddressOf OnPositionChanged
            RemoveHandler _geolocator.StatusChanged, AddressOf OnStatusChanged
        End If

        MyBase.OnNavigatingFrom(e)
    End Sub

    ''' <summary>
    ''' This is the event handler for PositionChanged events.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub OnPositionChanged(ByVal sender As Geolocator, ByVal e As PositionChangedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     Dim pos As Geoposition = e.Position
                                                                     rootPage.NotifyUser("Updated", NotifyType.StatusMessage)
                                                                     ScenarioOutput_Latitude.Text = pos.Coordinate.Point.Position.Latitude.ToString()
                                                                     ScenarioOutput_Longitude.Text = pos.Coordinate.Point.Position.Longitude.ToString()
                                                                     ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString()
                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' This is the event handler for StatusChanged events.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub OnStatusChanged(ByVal sender As Geolocator, ByVal e As StatusChangedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     ' Location platform is providing valid data.
                                                                     ' Location platform is acquiring a fix. It may or may not have data. Or the data may be less accurate.
                                                                     ' Location platform could not obtain location data.
                                                                     ' The permission to access location data is denied by the user or other policies.
                                                                     'Clear cached location data if any
                                                                     ' The location platform is not initialized. This indicates that the application has not made a request for location data.
                                                                     ' The location platform is not available on this version of the OS.
                                                                     Select Case e.Status
                                                                         Case PositionStatus.Ready
                                                                             ScenarioOutput_Status.Text = "Ready"
                                                                         Case PositionStatus.Initializing
                                                                             ScenarioOutput_Status.Text = "Initializing"
                                                                         Case PositionStatus.NoData
                                                                             ScenarioOutput_Status.Text = "No data"
                                                                         Case PositionStatus.Disabled
                                                                             ScenarioOutput_Status.Text = "Disabled"
                                                                             ScenarioOutput_Latitude.Text = "No data"
                                                                             ScenarioOutput_Longitude.Text = "No data"
                                                                             ScenarioOutput_Accuracy.Text = "No data"
                                                                         Case PositionStatus.NotInitialized
                                                                             ScenarioOutput_Status.Text = "Not initialized"
                                                                         Case PositionStatus.NotAvailable
                                                                             ScenarioOutput_Status.Text = "Not available"
                                                                         Case Else
                                                                             ScenarioOutput_Status.Text = "Unknown"
                                                                     End Select
                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'StartTracking' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StartTracking(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage)

        AddHandler _geolocator.PositionChanged, AddressOf OnPositionChanged
        AddHandler _geolocator.StatusChanged, AddressOf OnStatusChanged

        StartTrackingButton.IsEnabled = False
        StopTrackingButton.IsEnabled = True
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'StopTracking' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StopTracking(ByVal sender As Object, ByVal e As RoutedEventArgs)
        RemoveHandler _geolocator.PositionChanged, AddressOf OnPositionChanged
        RemoveHandler _geolocator.StatusChanged, AddressOf OnStatusChanged

        StartTrackingButton.IsEnabled = True
        StopTrackingButton.IsEnabled = False
    End Sub
End Class


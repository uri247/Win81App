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
Imports System.Threading

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private _geolocator As Geolocator = Nothing
    Private _cts As CancellationTokenSource = Nothing

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        _geolocator = New Geolocator()

        AddHandler DesiredAccuracyInMeters.TextChanged, AddressOf DesiredAccuracyInMeters_TextChanged
        AddHandler SetDesiredAccuracyInMeters.Click, AddressOf SetDesiredAccuracyInMeters_Click
    End Sub

    Private Sub SetDesiredAccuracyInMeters_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim desiredAccuracy As UInteger = UInteger.Parse(DesiredAccuracyInMeters.Text)

        _geolocator.DesiredAccuracyInMeters = desiredAccuracy

        ' update get field
        ScenarioOutput_DesiredAccuracyInMeters.Text = _geolocator.DesiredAccuracyInMeters.ToString()
    End Sub

    Private Sub DesiredAccuracyInMeters_TextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
        Try
            Dim value As UInteger = UInteger.Parse(DesiredAccuracyInMeters.Text)

            SetDesiredAccuracyInMeters.IsEnabled = True

            ' clear out status message
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch e1 As ArgumentNullException
            SetDesiredAccuracyInMeters.IsEnabled = False
        Catch e2 As FormatException
            rootPage.NotifyUser("Desired Accuracy must be a number", NotifyType.StatusMessage)
            SetDesiredAccuracyInMeters.IsEnabled = False
        Catch e3 As OverflowException
            rootPage.NotifyUser("Desired Accuracy is out of bounds", NotifyType.StatusMessage)
            SetDesiredAccuracyInMeters.IsEnabled = False
        End Try
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        GetGeolocationButton.IsEnabled = True
        CancelGetGeolocationButton.IsEnabled = False
        DesiredAccuracyInMeters.IsEnabled = False
        SetDesiredAccuracyInMeters.IsEnabled = False
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
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts = Nothing
        End If

        MyBase.OnNavigatingFrom(e)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'getGeolocation' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub GetGeolocation(ByVal sender As Object, ByVal e As RoutedEventArgs)
        GetGeolocationButton.IsEnabled = False
        CancelGetGeolocationButton.IsEnabled = True

        Try
            ' Get cancellation token
            _cts = New CancellationTokenSource()
            Dim token As CancellationToken = _cts.Token

            rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage)

            ' Carry out the operation
            Dim pos As Geoposition = Await _geolocator.GetGeopositionAsync().AsTask(token)

            rootPage.NotifyUser("Updated", NotifyType.StatusMessage)

            DesiredAccuracyInMeters.IsEnabled = True

            ScenarioOutput_Latitude.Text = pos.Coordinate.Point.Position.Latitude.ToString()
            ScenarioOutput_Longitude.Text = pos.Coordinate.Point.Position.Longitude.ToString()
            ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString()
            ScenarioOutput_Source.Text = pos.Coordinate.PositionSource.ToString()

            If pos.Coordinate.PositionSource = PositionSource.Satellite Then
                ' show labels and satellite data
                Label_PosPrecision.Opacity = 1
                ScenarioOutput_PosPrecision.Opacity = 1
                ScenarioOutput_PosPrecision.Text = pos.Coordinate.SatelliteData.PositionDilutionOfPrecision.ToString()
                Label_HorzPrecision.Opacity = 1
                ScenarioOutput_HorzPrecision.Opacity = 1
                ScenarioOutput_HorzPrecision.Text = pos.Coordinate.SatelliteData.HorizontalDilutionOfPrecision.ToString()
                Label_VertPrecision.Opacity = 1
                ScenarioOutput_VertPrecision.Opacity = 1
                ScenarioOutput_VertPrecision.Text = pos.Coordinate.SatelliteData.VerticalDilutionOfPrecision.ToString()
            Else
                ' hide labels and satellite data
                Label_PosPrecision.Opacity = 0
                ScenarioOutput_PosPrecision.Opacity = 0
                Label_HorzPrecision.Opacity = 0
                ScenarioOutput_HorzPrecision.Opacity = 0
                Label_VertPrecision.Opacity = 0
                ScenarioOutput_VertPrecision.Opacity = 0
            End If

            ScenarioOutput_DesiredAccuracyInMeters.Text = _geolocator.DesiredAccuracyInMeters.ToString()

        Catch e1 As System.UnauthorizedAccessException
            rootPage.NotifyUser("Disabled", NotifyType.StatusMessage)

            ScenarioOutput_Latitude.Text = "No data"
            ScenarioOutput_Longitude.Text = "No data"
            ScenarioOutput_Accuracy.Text = "No data"
        Catch e2 As TaskCanceledException
            rootPage.NotifyUser("Canceled", NotifyType.StatusMessage)
        Catch ex As Exception
            ' If there are no location sensors GetGeopositionAsync()
            ' will timeout -- that is acceptable.
            Const WaitTimeoutHResult As Integer = CInt(&H80070102)

            If ex.HResult = WaitTimeoutHResult Then ' WAIT_TIMEOUT
                rootPage.NotifyUser("Operation accessing location sensors timed out. Possibly there are no location sensors.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
            End If
        Finally
            _cts = Nothing
        End Try

        GetGeolocationButton.IsEnabled = True
        CancelGetGeolocationButton.IsEnabled = False
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'CancelGetGeolocation' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CancelGetGeolocation(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts = Nothing
        End If

        GetGeolocationButton.IsEnabled = True
        CancelGetGeolocationButton.IsEnabled = False
    End Sub
End Class


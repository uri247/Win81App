'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Threading
Imports Windows.Devices.Enumeration
Imports Windows.Devices.Geolocation
Imports Windows.Devices.Geolocation.Geofencing

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private nameSet As Boolean = False
    Private latitudeSet As Boolean = False
    Private longitudeSet As Boolean = False
    Private radiusSet As Boolean = False
    Private permissionsChecked As Boolean = False
    Private inGetPositionAsync As Boolean = False
    Private secondsPerMinute As Integer = 60
    Private secondsPerHour As Integer = 60 * 60
    Private secondsPerDay As Integer = 24 * 60 * 60
    Private oneHundredNanosecondsPerSecond As Integer = 10000000
    Private defaultDwellTimeSeconds As Integer = 10
    Private Const maxEventDescriptors As Integer = 42 ' Value determined by how many max length event descriptors (91 chars)
    ' stored as a JSON string can fit in 8K (max allowed for local settings)
    Private cts As CancellationTokenSource = Nothing
    Private geolocator As Geolocator = Nothing
    Private geofences As IList(Of Geofence)
    Private geofenceCollection As GeofenceItemCollection = Nothing
    Private eventCollection As EventDescriptorCollection = Nothing
    Private accessInfo As DeviceAccessInformation
    Private formatterShortDateLongTime As Windows.Globalization.DateTimeFormatting.DateTimeFormatter
    Private formatterLongTime As Windows.Globalization.DateTimeFormatting.DateTimeFormatter
    Private calendar As Windows.Globalization.Calendar
    Private decimalFormatter As Windows.Globalization.NumberFormatting.DecimalFormatter
    Private coreWindow As Windows.UI.Core.CoreWindow

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        Try
            formatterShortDateLongTime = New Windows.Globalization.DateTimeFormatting.DateTimeFormatter("{month.integer}/{day.integer}/{year.full} {hour.integer}:{minute.integer(2)}:{second.integer(2)}", {"en-US"}, "US", Windows.Globalization.CalendarIdentifiers.Gregorian, Windows.Globalization.ClockIdentifiers.TwentyFourHour)
            formatterLongTime = New Windows.Globalization.DateTimeFormatting.DateTimeFormatter("{hour.integer}:{minute.integer(2)}:{second.integer(2)}", {"en-US"}, "US", Windows.Globalization.CalendarIdentifiers.Gregorian, Windows.Globalization.ClockIdentifiers.TwentyFourHour)
            calendar = New Windows.Globalization.Calendar()
            decimalFormatter = New Windows.Globalization.NumberFormatting.DecimalFormatter()

            geofenceCollection = New GeofenceItemCollection()
            eventCollection = New EventDescriptorCollection()

            ' Get a geolocator object
            geolocator = New Geolocator()

            geofences = GeofenceMonitor.Current.Geofences

            ' using data binding to the root page collection of GeofenceItems
            RegisteredGeofenceListBox.DataContext = geofenceCollection

            ' using data binding to the root page collection of GeofenceItems associated with events
            GeofenceEventsListBox.DataContext = eventCollection

            FillRegisteredGeofenceListBoxWithExistingGeofences()
            FillEventListBoxWithExistingEvents()

            accessInfo = DeviceAccessInformation.CreateFromDeviceClass(DeviceClass.Location)
            AddHandler accessInfo.AccessChanged, AddressOf OnAccessChanged

            coreWindow = coreWindow.GetForCurrentThread() ' this needs to be set before InitializeComponent sets up event registration for app visibility
            AddHandler coreWindow.VisibilityChanged, AddressOf OnVisibilityChanged

            ' register for state change events
            AddHandler GeofenceMonitor.Current.GeofenceStateChanged, AddressOf OnGeofenceStateChanged
            AddHandler GeofenceMonitor.Current.StatusChanged, AddressOf OnGeofenceStatusChanged
        Catch e1 As UnauthorizedAccessException
            If DeviceAccessStatus.DeniedByUser = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.DeniedBySystem = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.Unspecified = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            ' GeofenceMonitor failed in adding a geofence
            ' exceptions could be from out of memory, lat/long out of range,
            ' too long a name, not a unique name, specifying an activation
            ' time + duration that is still in the past

            ' If ex.HResult is RPC_E_DISCONNECTED (0x80010108):
            ' The Location Framework service event state is out of synchronization
            ' with the Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.
            ' To recover remove all event handlers on the GeofenceMonitor or restart the application.
            ' Once all event handlers are removed you may add back any event handlers and retry the operation.

            rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
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
        If True = inGetPositionAsync Then
            If cts IsNot Nothing Then
                cts.Cancel()
                cts = Nothing
            End If
        End If

        RemoveHandler GeofenceMonitor.Current.GeofenceStateChanged, AddressOf OnGeofenceStateChanged
        RemoveHandler GeofenceMonitor.Current.StatusChanged, AddressOf OnGeofenceStatusChanged

        ' save off the contents of the event collection
        SaveExistingEvents()

        MyBase.OnNavigatingFrom(e)
    End Sub

    Private Sub OnVisibilityChanged(ByVal sender As CoreWindow, ByVal args As VisibilityChangedEventArgs)
        ' NOTE: After the app is no longer visible on the screen and before the app is suspended
        ' you might want your app to use toast notification for any geofence activity.
        ' By registering for VisibiltyChanged the app is notified when the app is no longer visible in the foreground.

        If args.Visible Then
            ' register for foreground events
            AddHandler GeofenceMonitor.Current.GeofenceStateChanged, AddressOf OnGeofenceStateChanged
            AddHandler GeofenceMonitor.Current.StatusChanged, AddressOf OnGeofenceStatusChanged
        Else
            ' unregister foreground events (let background capture events)
            RemoveHandler GeofenceMonitor.Current.GeofenceStateChanged, AddressOf OnGeofenceStateChanged
            RemoveHandler GeofenceMonitor.Current.StatusChanged, AddressOf OnGeofenceStatusChanged
        End If
    End Sub

    Public Async Sub OnAccessChanged(ByVal sender As DeviceAccessInformation, ByVal args As DeviceAccessChangedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     ' clear status
                                                                     Dim status = args.Status
                                                                     Dim eventDescription As String = GetTimeStampedMessage("Device Access Status")
                                                                     eventDescription &= " (" & status.ToString() & ")"
                                                                     AddEventDescription(eventDescription)
                                                                     If DeviceAccessStatus.DeniedByUser = args.Status Then
                                                                         rootPage.NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType.StatusMessage)
                                                                     ElseIf DeviceAccessStatus.DeniedBySystem = args.Status Then
                                                                         rootPage.NotifyUser("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", NotifyType.StatusMessage)
                                                                     ElseIf DeviceAccessStatus.Unspecified = args.Status Then
                                                                         rootPage.NotifyUser("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", NotifyType.StatusMessage)
                                                                     ElseIf DeviceAccessStatus.Allowed = args.Status Then
                                                                         rootPage.NotifyUser("", NotifyType.StatusMessage)
                                                                     Else
                                                                         rootPage.NotifyUser("Unknown device access information status", NotifyType.StatusMessage)
                                                                     End If
                                                                 End Sub)
    End Sub

    Public Async Sub OnGeofenceStatusChanged(ByVal sender As GeofenceMonitor, ByVal e As Object)
        Dim status = sender.Status

        Dim eventDescription As String = GetTimeStampedMessage("Geofence Status Changed")

        eventDescription &= " (" & status.ToString() & ")"

        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() AddEventDescription(eventDescription))
    End Sub

    Public Async Sub OnGeofenceStateChanged(ByVal sender As GeofenceMonitor, ByVal e As Object)
        Dim reports = sender.ReadReports()

        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     ' remove the geofence from the client side geofences collection
                                                                     ' empty the registered geofence listbox and repopulate
                                                                     ' NOTE: You might want to write your app to take particular
                                                                     ' action based on whether the app has internet connectivity.
                                                                     For Each report As GeofenceStateChangeReport In reports
                                                                         Dim state As GeofenceState = report.NewState
                                                                         Dim geofence As Geofence = report.Geofence
                                                                         Dim eventDescription As String = GetTimeStampedMessage(geofence.Id)
                                                                         eventDescription &= " (" & state.ToString()
                                                                         If state = GeofenceState.Removed Then
                                                                             eventDescription &= "/" & report.RemovalReason.ToString() & ")"
                                                                             AddEventDescription(eventDescription)
                                                                             Remove(geofence)
                                                                             geofenceCollection.Clear()
                                                                             FillRegisteredGeofenceListBoxWithExistingGeofences()
                                                                         ElseIf state = GeofenceState.Entered OrElse state = GeofenceState.Exited Then
                                                                             eventDescription &= ")"
                                                                             AddEventDescription(eventDescription)
                                                                         End If
                                                                     Next report
                                                                 End Sub)
    End Sub

    ''' <summary>
    ''' This method removes the geofence from the client side geofences collection
    ''' </summary>
    ''' <param name="geofence"></param>
    Private Sub Remove(ByVal geofence As Geofence)
        Try
            If Not geofences.Remove(geofence) Then
                Dim strMsg = "Could not find Geofence " & geofence.Id & " in the geofences collection"

                rootPage.NotifyUser(strMsg, NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Remove Geofence Item' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OnRemoveGeofenceItem(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Nothing IsNot RegisteredGeofenceListBox.SelectedItem Then
            ' get selected item
            Dim itemToRemove As GeofenceItem = TryCast(RegisteredGeofenceListBox.SelectedItem, GeofenceItem)

            Dim geofence = itemToRemove.Geofence

            ' remove the geofence from the client side geofences collection
            Remove(geofence)

            ' empty the registered geofence listbox and repopulate
            geofenceCollection.Clear()

            FillRegisteredGeofenceListBoxWithExistingGeofences()
        End If
    End Sub

    Private Function GenerateGeofence() As Geofence
        Dim geofence As Geofence = Nothing

        Dim fenceKey As New String(Id.Text.ToCharArray())

        Dim position As BasicGeoposition
        position.Latitude = Double.Parse(Latitude.Text)
        position.Longitude = Double.Parse(Longitude.Text)
        position.Altitude = 0.0
        Dim geoRadius As Double = Double.Parse(Radius.Text)

        ' the geofence is a circular region
        Dim geocircle As New Geocircle(position, geoRadius)

        Dim usedOnce As Boolean = CBool(SingleUse.IsChecked)

        ' want to listen for enter geofence, exit geofence and remove geofence events
        ' you can select a subset of these event states
        Dim mask As MonitoredGeofenceStates = 0

        mask = mask Or MonitoredGeofenceStates.Entered
        mask = mask Or MonitoredGeofenceStates.Exited
        mask = mask Or MonitoredGeofenceStates.Removed

        ' setting up how long you need to be in geofence for enter event to fire
        Dim lingerTime As TimeSpan

        If "" <> dwellTime.Text Then
            lingerTime = New TimeSpan(ParseTimeSpan(DwellTime.Text, defaultDwellTimeSeconds))
        Else
            lingerTime = New TimeSpan(ParseTimeSpan("0", defaultDwellTimeSeconds))
        End If

        ' setting up how long the geofence should be active
        Dim extent As TimeSpan

        If "" <> Duration.Text Then
            extent = New TimeSpan(ParseTimeSpan(Duration.Text, 0))
        Else
            extent = New TimeSpan(ParseTimeSpan("0", 0))
        End If

        ' setting up the start time of the geofence
        Dim start As DateTimeOffset

        Try
            If "" <> startTime.Text Then
                start = DateTimeOffset.Parse(StartTime.Text)
            Else
                ' if you don't set start time in VB the start time defaults to 1/1/1601
                calendar.SetToNow()

                start = calendar.GetDateTime()
            End If
        Catch e1 As ArgumentNullException
        Catch e2 As FormatException
            rootPage.NotifyUser("Start Time is not a valid string representation of a date and time", NotifyType.ErrorMessage)
        Catch e3 As ArgumentException
            rootPage.NotifyUser("The offset is greater than 14 hours or less than -14 hours.", NotifyType.ErrorMessage)
        End Try

        geofence = New Geofence(fenceKey, geocircle, mask, usedOnce, lingerTime, start, extent)

        Return geofence
    End Function
End Class


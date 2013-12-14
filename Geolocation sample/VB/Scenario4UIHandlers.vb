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
Imports Windows.Data.Json

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private Sub OnRadiusTextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
        radiusSet = TextChangedHandlerDouble(False, "Radius", Radius)

        DetermineCreateGeofenceButtonEnableState()
    End Sub

    Private Sub OnLongitudeTextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
        longitudeSet = TextChangedHandlerDouble(False, "Longitude", Longitude)

        DetermineCreateGeofenceButtonEnableState()
    End Sub

    Private Sub OnLatitudeTextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
        latitudeSet = TextChangedHandlerDouble(False, "Latitude", Latitude)

        DetermineCreateGeofenceButtonEnableState()
    End Sub

    Private Sub OnIdTextChanged(ByVal sender As Object, ByVal e As TextChangedEventArgs)
        ' get number of characters
        Dim characterCount As Integer = Id.Text.Length

        nameSet = (0 <> characterCount)

        CharCount.Text = characterCount.ToString() & " characters"

        DetermineCreateGeofenceButtonEnableState()
    End Sub

    Private Sub OnRegisteredGeofenceListBoxSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim list As IList(Of Object) = e.AddedItems

        If 0 = list.Count Then
            ' disable the remove button
            RemoveGeofenceItem.IsEnabled = False
        Else
            ' enable the remove button
            RemoveGeofenceItem.IsEnabled = True

            ' update controls with the values from this geofence item
            ' get selected item
            Dim item As GeofenceItem = TryCast(RegisteredGeofenceListBox.SelectedItem, GeofenceItem)

            RefreshControlsFromGeofenceItem(item)

            DetermineCreateGeofenceButtonEnableState()

        End If
    End Sub

    Private Sub RefreshControlsFromGeofenceItem(ByVal item As GeofenceItem)
        If Nothing IsNot item Then
            Id.Text = item.Id
            Latitude.Text = item.Latitude.ToString()
            Longitude.Text = item.Longitude.ToString()
            Radius.Text = item.Radius.ToString()
            SingleUse.IsChecked = item.SingleUse

            If 0 <> item.DwellTime.Ticks Then
                DwellTime.Text = item.DwellTime.ToString()
            Else
                DwellTime.Text = ""
            End If

            If 0 <> item.Duration.Ticks Then
                Duration.Text = item.Duration.ToString()
            Else
                Duration.Text = ""
            End If

            If 0 <> item.StartTime.Ticks Then
                Dim dt As DateTimeOffset = item.StartTime

                StartTime.Text = formatterShortDateLongTime.Format(dt)
            Else
                StartTime.Text = ""
            End If

            ' Update flags used to enable Create Geofence button
            OnIdTextChanged(Nothing, Nothing)
            OnLongitudeTextChanged(Nothing, Nothing)
            OnLatitudeTextChanged(Nothing, Nothing)
            OnRadiusTextChanged(Nothing, Nothing)
        End If
    End Sub

    Private Function TextChangedHandlerDouble(ByVal nullAllowed As Boolean, ByVal name As String, ByVal e As TextBox) As Boolean
        Dim valueSet As Boolean = False

        Try
            Dim value As Double = Double.Parse(e.Text)

            valueSet = True

            ' clear out status message
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch e1 As ArgumentNullException
            If False = nullAllowed Then
                If Nothing IsNot name Then
                    rootPage.NotifyUser(name & " needs a value", NotifyType.StatusMessage)
                End If
            End If
        Catch e2 As FormatException
            If Nothing IsNot name Then
                rootPage.NotifyUser(name & " must be a number", NotifyType.StatusMessage)
            End If
        Catch e3 As OverflowException
            If Nothing IsNot name Then
                rootPage.NotifyUser(name & " is out of bounds", NotifyType.StatusMessage)
            End If
        End Try

        Return valueSet
    End Function

    Private Function TextChangedHandlerInt(ByVal nullAllowed As Boolean, ByVal name As String, ByVal e As TextBox) As Boolean
        Dim valueSet As Boolean = False

        Try
            Dim value As Integer = Integer.Parse(e.Text)

            valueSet = True

            ' clear out status message
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch e1 As ArgumentNullException
            If False = nullAllowed Then
                If Nothing IsNot name Then
                    rootPage.NotifyUser(name & " needs a value", NotifyType.StatusMessage)
                End If
            End If
        Catch e2 As FormatException
            If Nothing IsNot name Then
                rootPage.NotifyUser(name & " must be a number", NotifyType.StatusMessage)
            End If
        Catch e3 As OverflowException
            If Nothing IsNot name Then
                rootPage.NotifyUser(name & " is out of bounds", NotifyType.StatusMessage)
            End If
        End Try

        Return valueSet
    End Function

    Private Sub SetStartTimeToNowFunction()
        Try
            calendar.SetToNow()

            Dim dt As DateTimeOffset = calendar.GetDateTime()

            StartTime.Text = formatterShortDateLongTime.Format(dt)
        Catch ex As Exception
            rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Function GetTimeStampedMessage(ByVal eventCalled As String) As String
        Dim message As String

        calendar.SetToNow()

        message = eventCalled & " " & formatterLongTime.Format(calendar.GetDateTime())

        Return message
    End Function

    ' are settings available so a geofence can be created?
    Private Function SettingsAvailable() As Boolean
        Dim fSettingsAvailable As Boolean = False

        If True = nameSet AndAlso True = latitudeSet AndAlso True = longitudeSet AndAlso True = radiusSet Then
            ' also need to test if data is good
            fSettingsAvailable = True
        End If

        Return fSettingsAvailable
    End Function

    Private Sub DetermineCreateGeofenceButtonEnableState()
        CreateGeofenceButton.IsEnabled = SettingsAvailable()
    End Sub

    ' add geofence to listbox
    Private Sub AddGeofenceToRegisteredGeofenceListBox(ByVal geofence As Geofence)
        Dim item As New GeofenceItem(geofence)

        ' the registered geofence listbox is data bound
        ' to the collection stored in the root page
        geofenceCollection.Insert(0, item)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Set Start Time to Now' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OnSetStartTimeToNow(ByVal sender As Object, ByVal e As RoutedEventArgs)
        SetStartTimeToNowFunction()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Set Lat/Long to current position' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub OnSetPositionToHere(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' save off current cursor and set cursor to a wait cursor
        Dim oldCursor As Windows.UI.Core.CoreCursor = Window.Current.CoreWindow.PointerCursor
        Window.Current.CoreWindow.PointerCursor = New Windows.UI.Core.CoreCursor(CoreCursorType.Wait, 0)
        SetPositionToHereButton.IsEnabled = False
        Latitude.IsEnabled = False
        Longitude.IsEnabled = False

        Try
            ' Get cancellation token
            cts = New CancellationTokenSource()
            Dim token As CancellationToken = cts.Token

            ' Carry out the operation
            Dim pos As Geoposition = Await geolocator.GetGeopositionAsync().AsTask(token)

            Latitude.Text = pos.Coordinate.Point.Position.Latitude.ToString()
            Longitude.Text = pos.Coordinate.Point.Position.Longitude.ToString()

            ' clear status
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch e1 As UnauthorizedAccessException
            If DeviceAccessStatus.DeniedByUser = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.DeniedBySystem = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.Unspecified = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", NotifyType.StatusMessage)
            End If
        Catch e2 As TaskCanceledException
            rootPage.NotifyUser("Task canceled", NotifyType.StatusMessage)
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
            cts = Nothing
        End Try

        ' restore cursor and re-enable controls
        Window.Current.CoreWindow.PointerCursor = oldCursor
        SetPositionToHereButton.IsEnabled = True
        Latitude.IsEnabled = True
        Longitude.IsEnabled = True
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Create Geofence' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OnCreateGeofence(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            ' This must be done here because there is no guarantee of 
            ' getting the location consent from a geofence call.
            If False = permissionsChecked Then
                GetGeopositionAsync()
                permissionsChecked = True
            End If

            ' get lat/long/radius, the fence name (fenceKey), 
            ' and other properties from controls,
            ' depending on data in controls for activation time
            ' and duration the appropriate
            ' constructor will be used.
            Dim geofence As Geofence = GenerateGeofence()

            ' Add the geofence to the GeofenceMonitor's
            ' collection of fences
            geofences.Add(geofence)

            ' add geofence to listbox
            AddGeofenceToRegisteredGeofenceListBox(geofence)
        Catch e1 As System.UnauthorizedAccessException
            If DeviceAccessStatus.DeniedByUser = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.DeniedBySystem = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.Unspecified = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", NotifyType.StatusMessage)
            End If
        Catch e2 As TaskCanceledException
            rootPage.NotifyUser("Canceled", NotifyType.StatusMessage)
        Catch ex As Exception
            ' GeofenceMonitor failed in adding a geofence
            ' exceptions could be from out of memory, lat/long out of range,
            ' too long a name, not a unique name, specifying an activation
            ' time + duration that is still in the past
            rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
        Finally
        End Try
    End Sub

    ''' <summary>
    ''' Helper method to invoke Geolocator.GetGeopositionAsync.
    ''' </summary>
    Private Async Sub GetGeopositionAsync()
        rootPage.NotifyUser("Checking permissions...", NotifyType.StatusMessage)

        inGetPositionAsync = True

        Try
            ' Get cancellation token
            cts = New CancellationTokenSource()
            Dim token As CancellationToken = cts.Token

            ' Carry out the operation
            Await geolocator.GetGeopositionAsync().AsTask(token)

            ' clear status
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch e1 As UnauthorizedAccessException
            If DeviceAccessStatus.DeniedByUser = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.DeniedBySystem = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", NotifyType.StatusMessage)
            ElseIf DeviceAccessStatus.Unspecified = accessInfo.CurrentStatus Then
                rootPage.NotifyUser("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", NotifyType.StatusMessage)
            End If
        Catch e2 As TaskCanceledException
            rootPage.NotifyUser("Task canceled", NotifyType.StatusMessage)
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
            cts = Nothing
        End Try

        inGetPositionAsync = False
    End Sub

    Private Sub FillRegisteredGeofenceListBoxWithExistingGeofences()
        For Each geofence As Geofence In geofences
            AddGeofenceToRegisteredGeofenceListBox(geofence)
        Next geofence
    End Sub

    Private Sub FillEventListBoxWithExistingEvents()
        Dim settings = ApplicationData.Current.LocalSettings
        If settings.Values.ContainsKey("ForegroundGeofenceEventCollection") Then
            Dim geofenceEvent As String = settings.Values("ForegroundGeofenceEventCollection").ToString()

            If 0 <> geofenceEvent.Length Then
                Dim events = JsonValue.Parse(geofenceEvent).GetArray()

                ' NOTE: the events are accessed in reverse order
                ' because the events were added to JSON from
                ' newer to older.  AddEventDescription() adds
                ' each new entry to the beginning of the collection.
                For pos As Integer = events.Count - 1 To 0 Step -1
                    Dim element = events.GetStringAt(CUInt(pos))
                    AddEventDescription(element)
                Next pos
            End If

            settings.Values("ForegroundGeofenceEventCollection") = Nothing
        End If
    End Sub

    Private Sub SaveExistingEvents()
        Dim jsonArray As New JsonArray()

        For Each eventDescriptor In eventCollection
            jsonArray.Add(JsonValue.CreateStringValue(eventDescriptor.ToString()))
        Next eventDescriptor

        Dim jsonString As String = jsonArray.Stringify()

        Dim settings = ApplicationData.Current.LocalSettings
        settings.Values("ForegroundGeofenceEventCollection") = jsonString
    End Sub

    Private Sub AddEventDescription(ByVal eventDescription As String)
        If eventCollection.Count = maxEventDescriptors Then
            eventCollection.RemoveAt(maxEventDescriptors - 1)
        End If

        eventCollection.Insert(0, eventDescription)
    End Sub

    Private Enum TimeComponent
        day
        hour
        minute
        second
    End Enum

    Private Function ParseTimeSpan(ByVal field As String, ByVal defaultValue As Integer) As Long
        Dim timeSpanValue As Long = 0
        Dim delimiterChars() As Char = {":"c}
        Dim timeComponents() As String = field.Split(delimiterChars)
        Dim start As Integer = 4 - timeComponents.Length

        If start >= 0 Then
            Dim [loop] As Integer = 0
            Dim index As Integer = start
            Do While [loop] < timeComponents.Length
                Dim tc As TimeComponent = CType(index, TimeComponent)

                Select Case tc
                    Case TimeComponent.day
                        timeSpanValue += CLng(decimalFormatter.ParseInt(timeComponents([loop]))) * secondsPerDay

                    Case TimeComponent.hour
                        timeSpanValue += CLng(decimalFormatter.ParseInt(timeComponents([loop]))) * secondsPerHour

                    Case TimeComponent.minute
                        timeSpanValue += CLng(decimalFormatter.ParseInt(timeComponents([loop]))) * secondsPerMinute

                    Case TimeComponent.second
                        timeSpanValue += CLng(decimalFormatter.ParseInt(timeComponents([loop])))

                    Case Else
                End Select
                [loop] += 1
                index += 1
            Loop
        End If

        If 0 = timeSpanValue Then
            timeSpanValue = defaultValue
        End If

        timeSpanValue *= oneHundredNanosecondsPerSecond

        Return timeSpanValue
    End Function

    Private Class GeofenceItemCollection
        Inherits System.Collections.ObjectModel.ObservableCollection(Of GeofenceItem)

    End Class

    Private Class EventDescriptorCollection
        Inherits System.Collections.ObjectModel.ObservableCollection(Of String)

    End Class
End Class

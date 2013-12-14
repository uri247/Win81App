Imports System
Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Data.Json
Imports Windows.Storage
Imports Windows.Devices.Geolocation
Imports Windows.Devices.Geolocation.Geofencing
Imports Windows.UI.Notifications

Namespace BackgroundTask
    Public NotInheritable Class LocationBackgroundTask
        Implements IBackgroundTask

        Private cts As CancellationTokenSource = Nothing

        Private Async Sub IBackgroundTask_Run(ByVal taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
            Dim deferral As BackgroundTaskDeferral = taskInstance.GetDeferral()

            Try
                ' Associate a cancellation handler with the background task.
                AddHandler taskInstance.Canceled, AddressOf OnCanceled

                ' Get cancellation token
                If cts Is Nothing Then
                    cts = New CancellationTokenSource()
                End If
                Dim token As CancellationToken = cts.Token

                ' Create geolocator object
                Dim geolocator As New Geolocator()

                ' Make the request for the current position
                Dim pos As Geoposition = Await geolocator.GetGeopositionAsync().AsTask(token)

                Dim currentTime As Date = Date.Now

                WriteStatusToAppdata("Time: " & currentTime.ToString())
                WriteGeolocToAppdata(pos)
            Catch e1 As UnauthorizedAccessException
                WriteStatusToAppdata("Disabled")
                WipeGeolocDataFromAppdata()
            Catch ex As Exception
                ' If there are no location sensors GetGeopositionAsync()
                ' will timeout -- that is acceptable.
                Const WaitTimeoutHResult As Integer = CInt(&H80070102)

                If ex.HResult = WaitTimeoutHResult Then ' WAIT_TIMEOUT
                    WriteStatusToAppdata("An operation requiring location sensors timed out. Possibly there are no location sensors.")
                Else
                    WriteStatusToAppdata(ex.ToString())
                End If

                WipeGeolocDataFromAppdata()
            Finally
                cts = Nothing

                deferral.Complete()
            End Try
        End Sub

        Private Sub WriteGeolocToAppdata(ByVal pos As Geoposition)
            Dim settings = ApplicationData.Current.LocalSettings
            settings.Values("Latitude") = pos.Coordinate.Point.Position.Latitude.ToString()
            settings.Values("Longitude") = pos.Coordinate.Point.Position.Longitude.ToString()
            settings.Values("Accuracy") = pos.Coordinate.Accuracy.ToString()
        End Sub

        Private Sub WipeGeolocDataFromAppdata()
            Dim settings = ApplicationData.Current.LocalSettings
            settings.Values("Latitude") = ""
            settings.Values("Longitude") = ""
            settings.Values("Accuracy") = ""
        End Sub

        Private Sub WriteStatusToAppdata(ByVal status As String)
            Dim settings = ApplicationData.Current.LocalSettings
            settings.Values("Status") = status
        End Sub

        Private Sub OnCanceled(ByVal sender As IBackgroundTaskInstance, ByVal reason As BackgroundTaskCancellationReason)
            If cts IsNot Nothing Then
                cts.Cancel()
                cts = Nothing
            End If
        End Sub
    End Class

    Public NotInheritable Class GeofenceBackgroundTask
        Implements IBackgroundTask

        Private cts As CancellationTokenSource = Nothing
        Private geofenceBackgroundEvents As ItemCollection = Nothing
        Private Const oneHundredNanosecondsPerSecond As Long = 10000000 ' conversion from 100 nano-second resolution to seconds
        Private Const maxEventDescriptors As Integer = 42 ' Value determined by how many max length event descriptors (91 chars)
                                                    ' stored as a JSON string can fit in 8K (max allowed for local settings)

        Private Class ItemCollection
            Inherits System.Collections.ObjectModel.ObservableCollection(Of String)

        End Class

        Public Sub New()
            geofenceBackgroundEvents = New ItemCollection()
        End Sub

        Private Async Sub IBackgroundTask_Run(ByVal taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
            Dim deferral As BackgroundTaskDeferral = taskInstance.GetDeferral()

            Try
                ' Associate a cancellation handler with the background task.
                AddHandler taskInstance.Canceled, AddressOf OnCanceled

                ' Get cancellation token
                If cts Is Nothing Then
                    cts = New CancellationTokenSource()
                End If
                Dim token As CancellationToken = cts.Token

                ' Create geolocator object
                Dim geolocator As New Geolocator()

                ' Make the request for the current position
                Dim pos As Geoposition = Await geolocator.GetGeopositionAsync().AsTask(token)

                GetGeofenceStateChangedReports(pos)
            Catch e1 As UnauthorizedAccessException
                WriteStatusToAppdata("Location Permissions disabled by user. Enable access through the settings charm to enable the background task.")
                WipeGeofenceDataFromAppdata()
            Finally
                cts = Nothing

                deferral.Complete()
            End Try
        End Sub

        Private Sub WipeGeofenceDataFromAppdata()
            Dim settings = ApplicationData.Current.LocalSettings
            settings.Values("GeofenceEvent") = ""
        End Sub

        Private Sub WriteStatusToAppdata(ByVal status As String)
            Dim settings = ApplicationData.Current.LocalSettings
            settings.Values("Status") = status
        End Sub

        Private Sub OnCanceled(ByVal sender As IBackgroundTaskInstance, ByVal reason As BackgroundTaskCancellationReason)
            If cts IsNot Nothing Then
                cts.Cancel()
                cts = Nothing
            End If
        End Sub

        Private Sub GetGeofenceStateChangedReports(ByVal pos As Geoposition)
            geofenceBackgroundEvents.Clear()

            FillEventCollectionWithExistingEvents()

            Dim monitor As GeofenceMonitor = GeofenceMonitor.Current

            Dim posLastKnown As Geoposition = monitor.LastKnownGeoposition

            Dim calendar As New Windows.Globalization.Calendar()
            Dim formatterLongTime As Windows.Globalization.DateTimeFormatting.DateTimeFormatter
            formatterLongTime = New Windows.Globalization.DateTimeFormatting.DateTimeFormatter("{hour.integer}:{minute.integer(2)}:{second.integer(2)}", { "en-US" }, "US", Windows.Globalization.CalendarIdentifiers.Gregorian, Windows.Globalization.ClockIdentifiers.TwentyFourHour)

            Dim eventOfInterest As Boolean = True

            ' NOTE TO DEVELOPER:
            ' These events can be filtered out if the
            ' geofence event time is stale.
            Dim eventDateTime As DateTimeOffset = pos.Coordinate.Timestamp

            calendar.SetToNow()
            Dim nowDateTime As DateTimeOffset = calendar.GetDateTime()

            Dim diffTimeSpan As TimeSpan = nowDateTime - eventDateTime

            Dim deltaInSeconds As Long = diffTimeSpan.Ticks \ oneHundredNanosecondsPerSecond

            ' NOTE TO DEVELOPER:
            ' If the time difference between the geofence event and now is too large
            ' the eventOfInterest should be set to false.

            If eventOfInterest Then
                ' NOTE TO DEVELOPER:
                ' This event can be filtered out if the
                ' geofence event location is too far away.
                If (posLastKnown.Coordinate.Point.Position.Latitude <> pos.Coordinate.Point.Position.Latitude) OrElse (posLastKnown.Coordinate.Point.Position.Longitude <> pos.Coordinate.Point.Position.Longitude) Then
                    ' NOTE TO DEVELOPER:
                    ' Use an algorithm like the Haversine formula or Vincenty's formulae to determine
                    ' the distance between the current location (pos.Coordinate)
                    ' and the location of the geofence event (latitudeEvent/longitudeEvent).
                    ' If too far apart set eventOfInterest to false to
                    ' filter the event out.
                End If

                If eventOfInterest Then
                    Dim geofenceItemEvent As String = Nothing

                    Dim numEventsOfInterest As Integer = 0

                    ' Retrieve a vector of state change reports
                    Dim reports = GeofenceMonitor.Current.ReadReports()

                    For Each report As GeofenceStateChangeReport In reports
                        Dim state As GeofenceState = report.NewState

                        geofenceItemEvent = report.Geofence.Id & " " & formatterLongTime.Format(eventDateTime)

                        If state = GeofenceState.Removed Then
                            Dim reason As GeofenceRemovalReason = report.RemovalReason

                            If reason = GeofenceRemovalReason.Expired Then
                                geofenceItemEvent &= " (Removed/Expired)"
                            ElseIf reason = GeofenceRemovalReason.Used Then
                                geofenceItemEvent &= " (Removed/Used)"
                            End If
                        ElseIf state = GeofenceState.Entered Then
                            geofenceItemEvent &= " (Entered)"
                        ElseIf state = GeofenceState.Exited Then
                            geofenceItemEvent &= " (Exited)"
                        End If

                        AddGeofenceEvent(geofenceItemEvent)

                        numEventsOfInterest += 1
                    Next report

                    If eventOfInterest AndAlso (0 <> numEventsOfInterest) Then
                        SaveExistingEvents()

                        ' NOTE: Other notification mechanisms can be used here, such as Badge and/or Tile updates.
                        DoToast(numEventsOfInterest, geofenceItemEvent)
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Helper method to pop a toast
        ''' </summary>
        Private Sub DoToast(ByVal numEventsOfInterest As Integer, ByVal eventName As String)
            ' pop a toast for each geofence event
            Dim ToastNotifier As ToastNotifier = ToastNotificationManager.CreateToastNotifier()

            ' Create a two line toast and add audio reminder

            ' Here the xml that will be passed to the 
            ' ToastNotification for the toast is retrieved
            Dim toastXml As Windows.Data.Xml.Dom.XmlDocument = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)

            ' Set both lines of text
            Dim toastNodeList As Windows.Data.Xml.Dom.XmlNodeList = toastXml.GetElementsByTagName("text")
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode("Geolocation Sample"))

            If 1 = numEventsOfInterest Then
                toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(eventName))
            Else
                Dim secondLine As String = "There are " & numEventsOfInterest & " new geofence events"
                toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(secondLine))
            End If

            ' now create a xml node for the audio source
            Dim toastNode As Windows.Data.Xml.Dom.IXmlNode = toastXml.SelectSingleNode("/toast")
            Dim audio As Windows.Data.Xml.Dom.XmlElement = toastXml.CreateElement("audio")
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS")

            Dim toast As New ToastNotification(toastXml)
            ToastNotifier.Show(toast)
        End Sub

        Private Sub FillEventCollectionWithExistingEvents()
            Dim settings = ApplicationData.Current.LocalSettings
            If settings.Values.ContainsKey("BackgroundGeofenceEventCollection") Then
                Dim geofenceEvent As String = settings.Values("BackgroundGeofenceEventCollection").ToString()

                If 0 <> geofenceEvent.Length Then
                    Dim events = JsonValue.Parse(geofenceEvent).GetArray()

                    ' NOTE: the events are accessed in reverse order
                    ' because the events were added to JSON from
                    ' newer to older.  AddGeofenceEvent() adds
                    ' each new entry to the beginning of the collection.
                    For pos As Integer = events.Count - 1 To 0 Step -1
                        Dim element = events.GetStringAt(CUInt(pos))
                        AddGeofenceEvent(element)
                    Next pos
                End If
            End If
        End Sub

        Private Sub SaveExistingEvents()
            Dim jsonArray = New JsonArray()

            For Each eventDescriptor In geofenceBackgroundEvents
                jsonArray.Add(JsonValue.CreateStringValue(eventDescriptor.ToString()))
            Next eventDescriptor

            Dim jsonString As String = jsonArray.Stringify()

            Dim settings = ApplicationData.Current.LocalSettings
            settings.Values("BackgroundGeofenceEventCollection") = jsonString
        End Sub

        Private Sub AddGeofenceEvent(ByVal eventDescription As String)
            If geofenceBackgroundEvents.Count = maxEventDescriptors Then
                geofenceBackgroundEvents.RemoveAt(maxEventDescriptors - 1)
            End If

            geofenceBackgroundEvents.Insert(0, eventDescription)
        End Sub
    End Class
End Namespace

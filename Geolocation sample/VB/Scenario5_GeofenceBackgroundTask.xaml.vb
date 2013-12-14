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
Imports Windows.ApplicationModel.Background
Imports Windows.Data.Json
Imports Windows.Devices.Geolocation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private geofenceTask As IBackgroundTaskRegistration = Nothing
    Private cts As CancellationTokenSource = Nothing
    Private geofenceBackgroundEvents As ItemCollection = Nothing
    Private geolocator As Geolocator = Nothing
    Private Const oneHundredNanosecondsPerSecond As Long = 10000000 ' conversion from 100 nano-second resolution to seconds

    Private Class ItemCollection
        Inherits System.Collections.ObjectModel.ObservableCollection(Of String)

    End Class

    Private Const SampleBackgroundTaskName As String = "SampleGeofenceBackgroundTask"
    Private Const SampleBackgroundTaskEntryPoint As String = "BackgroundTask.GeofenceBackgroundTask"

    Public Sub New()
        Me.InitializeComponent()

        Dim settings = ApplicationData.Current.LocalSettings

        ' Get a geolocator object
        geolocator = New Geolocator()

        geofenceBackgroundEvents = New ItemCollection()

        ' using data binding to the root page collection of GeofenceItems associated with events
        GeofenceBackgroundEventsListBox.DataContext = geofenceBackgroundEvents
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Loop through all background tasks to see if SampleGeofenceBackgroundTask is already registered
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = SampleBackgroundTaskName Then
                geofenceTask = cur.Value
                Exit For
            End If
        Next cur

        If geofenceTask IsNot Nothing Then
            FillEventListBoxWithExistingEvents()

            ' Associate an event handler with the existing background task
            AddHandler geofenceTask.Completed, AddressOf OnCompleted

            Try
                Dim backgroundAccessStatus As BackgroundAccessStatus = BackgroundExecutionManager.GetAccessStatus()

                Select Case backgroundAccessStatus
                    Case backgroundAccessStatus.Unspecified, backgroundAccessStatus.Denied
                        rootPage.NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType.ErrorMessage)

                    Case Else
                        rootPage.NotifyUser("Background task is already registered. Waiting for next update...", NotifyType.ErrorMessage)
                End Select
            Catch ex As Exception
                ' HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED) == 0x80070032
                Const RequestNotSupportedHResult As Integer = CInt(&H80070032)

                If ex.HResult = RequestNotSupportedHResult Then
                    rootPage.NotifyUser("Location Simulator not supported.  Could not determine lock screen status, be sure that the application is added to the lock screen.", NotifyType.StatusMessage)
                Else
                    rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
                End If
            End Try

            UpdateButtonStates(True) 'registered:
        Else
            UpdateButtonStates(False) 'registered:
        End If
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
        ' Just in case the original GetGeopositionAsync call is still active
        CancelGetGeoposition()

        MyBase.OnNavigatingFrom(e)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Register' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub RegisterBackgroundTask(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            ' Get permission for a background task from the user. If the user has already answered once,
            ' this does nothing and the user must manually update their preference via PC Settings.
            Dim backgroundAccessStatus As BackgroundAccessStatus = Await BackgroundExecutionManager.RequestAccessAsync()

            ' Regardless of the answer, register the background task. If the user later adds this application
            ' to the lock screen, the background task will be ready to run.
            ' Create a new background task builder
            Dim geofenceTaskBuilder As New BackgroundTaskBuilder()

            geofenceTaskBuilder.Name = SampleBackgroundTaskName
            geofenceTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint

            ' Create a new location trigger
            Dim trigger = New LocationTrigger(LocationTriggerType.Geofence)

            ' Associate the locationi trigger with the background task builder
            geofenceTaskBuilder.SetTrigger(trigger)

            ' If it is important that there is user presence and/or
            ' internet connection when OnCompleted is called
            ' the following could be called before calling Register()
            ' SystemCondition condition = new SystemCondition(SystemConditionType.UserPresent | SystemConditionType.InternetAvailable);
            ' geofenceTaskBuilder.AddCondition(condition);

            ' Register the background task
            geofenceTask = geofenceTaskBuilder.Register()

            ' Associate an event handler with the new background task
            AddHandler geofenceTask.Completed, AddressOf OnCompleted

            UpdateButtonStates(True) 'registered:

            Select Case backgroundAccessStatus
                Case backgroundAccessStatus.Unspecified, backgroundAccessStatus.Denied
                    rootPage.NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType.ErrorMessage)

                Case Else
                    ' Ensure we have presented the location consent prompt (by asynchronously getting the current
                    ' position). This must be done here because the background task cannot display UI.
                    GetGeopositionAsync()
            End Select
        Catch ex As Exception
            ' HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED) == 0x80070032
            Const RequestNotSupportedHResult As Integer = CInt(&H80070032)

            If ex.HResult = RequestNotSupportedHResult Then
                rootPage.NotifyUser("Location Simulator not supported.  Could not get permission to add application to the lock screen, this application must be added to the lock screen before the background task will run.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
            End If

            UpdateButtonStates(False) 'registered:
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Unregister' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub UnregisterBackgroundTask(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Unregister the background task
        If Nothing IsNot geofenceTask Then
            geofenceTask.Unregister(True)
            geofenceTask = Nothing
        End If

        rootPage.NotifyUser("Background task unregistered", NotifyType.StatusMessage)

        UpdateButtonStates(False) 'registered:
    End Sub

    ''' <summary>
    ''' Helper method to invoke Geolocator.GetGeopositionAsync.
    ''' </summary>
    Private Async Sub GetGeopositionAsync()
        rootPage.NotifyUser("Checking permissions...", NotifyType.StatusMessage)

        Try
            ' Get cancellation token
            cts = New CancellationTokenSource()
            Dim token As CancellationToken = cts.Token

            ' Carry out the operation
            Dim pos As Geoposition = Await geolocator.GetGeopositionAsync().AsTask(token)

            ' got permissions so clear the status string
            rootPage.NotifyUser("", NotifyType.StatusMessage)
        Catch e1 As UnauthorizedAccessException
            rootPage.NotifyUser("Location Permissions disabled by user. Enable access through the settings charm to enable the background task.", NotifyType.StatusMessage)
        Catch e2 As TaskCanceledException
            rootPage.NotifyUser("Permission check operation canceled.", NotifyType.StatusMessage)
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
    End Sub

    ''' <summary>
    ''' Helper method to cancel the GetGeopositionAsync request (if any).
    ''' </summary>
    Private Sub CancelGetGeoposition()
        If cts IsNot Nothing Then
            cts.Cancel()
            cts = Nothing
        End If
    End Sub

    ''' <summary>
    ''' This is the callback when background event has been handled
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub OnCompleted(ByVal sender As IBackgroundTaskRegistration, ByVal e As BackgroundTaskCompletedEventArgs)
        If sender IsNot Nothing Then
            ' Update the UI with progress reported by the background task
            Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                         ' If the background task threw an exception, display the exception in
                                                                         ' the error text box.
                                                                         ' Update the UI with the completion status of the background task
                                                                         ' The Run method of the background task sets the LocalSettings. 
                                                                         ' get status
                                                                         ' The background task had an error
                                                                         Try
                                                                             e.CheckResult()
                                                                             Dim settings = ApplicationData.Current.LocalSettings
                                                                             If settings.Values.ContainsKey("Status") Then
                                                                                 rootPage.NotifyUser(settings.Values("Status").ToString(), NotifyType.StatusMessage)
                                                                             End If
                                                                             FillEventListBoxWithExistingEvents()
                                                                         Catch ex As Exception
                                                                             rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage)
                                                                         End Try
                                                                     End Sub)
        End If
    End Sub

    ''' <summary>
    ''' Update the enable state of the register/unregister buttons.
    ''' 
    Private Async Sub UpdateButtonStates(ByVal registered As Boolean)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     RegisterBackgroundTaskButton.IsEnabled = Not registered
                                                                     UnregisterBackgroundTaskButton.IsEnabled = registered
                                                                 End Sub)
    End Sub

    Private Sub FillEventListBoxWithExistingEvents()
        Dim settings = ApplicationData.Current.LocalSettings
        If settings.Values.ContainsKey("BackgroundGeofenceEventCollection") Then
            Dim geofenceEvent As String = settings.Values("BackgroundGeofenceEventCollection").ToString()

            If 0 <> geofenceEvent.Length Then
                ' remove all entries and repopulate
                geofenceBackgroundEvents.Clear()

                Dim events = JsonValue.Parse(geofenceEvent).GetArray()

                ' NOTE: the events are accessed in reverse order
                ' because the events were added to JSON from
                ' newer to older.  geofenceBackgroundEvents.Insert() adds
                ' each new entry to the beginning of the collection.
                For pos As Integer = events.Count - 1 To 0 Step -1
                    Dim element = events.GetStringAt(CUInt(pos))
                    geofenceBackgroundEvents.Insert(0, element)
                Next pos
            End If
        End If
    End Sub
End Class



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
Imports Windows.Devices.Geolocation
Imports Windows.ApplicationModel.Background


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private _geolocTask As IBackgroundTaskRegistration = Nothing
    Private _cts As CancellationTokenSource = Nothing

    Private Const SampleBackgroundTaskName As String = "SampleLocationBackgroundTask"
    Private Const SampleBackgroundTaskEntryPoint As String = "BackgroundTask.LocationBackgroundTask"

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Loop through all background tasks to see if SampleBackgroundTaskName is already registered
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = SampleBackgroundTaskName Then
                _geolocTask = cur.Value
                Exit For
            End If
        Next cur

        If _geolocTask IsNot Nothing Then
            ' Associate an event handler with the existing background task
            AddHandler _geolocTask.Completed, AddressOf OnCompleted

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

        If _geolocTask IsNot Nothing Then
            ' Remove the event handler
            RemoveHandler _geolocTask.Completed, AddressOf OnCompleted
        End If

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
            Dim geolocTaskBuilder As New BackgroundTaskBuilder()

            geolocTaskBuilder.Name = SampleBackgroundTaskName
            geolocTaskBuilder.TaskEntryPoint = SampleBackgroundTaskEntryPoint

            ' Create a new timer triggering at a 15 minute interval
            Dim trigger = New TimeTrigger(15, False)

            ' Associate the timer trigger with the background task builder
            geolocTaskBuilder.SetTrigger(trigger)

            ' Register the background task
            _geolocTask = geolocTaskBuilder.Register()

            ' Associate an event handler with the new background task
            AddHandler _geolocTask.Completed, AddressOf OnCompleted

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
        If Nothing IsNot _geolocTask Then
            _geolocTask.Unregister(True)
            _geolocTask = Nothing
        End If

        rootPage.NotifyUser("Background task unregistered", NotifyType.StatusMessage)

        ScenarioOutput_Latitude.Text = "No data"
        ScenarioOutput_Longitude.Text = "No data"
        ScenarioOutput_Accuracy.Text = "No data"

        UpdateButtonStates(False) 'registered:
    End Sub

    ''' <summary>
    ''' Helper method to invoke Geolocator.GetGeopositionAsync.
    ''' </summary>
    Private Async Sub GetGeopositionAsync()
        Try
            ' Get cancellation token
            _cts = New CancellationTokenSource()
            Dim token As CancellationToken = _cts.Token

            ' Get a geolocator object
            Dim geolocator As New Geolocator()

            rootPage.NotifyUser("Getting initial position...", NotifyType.StatusMessage)

            ' Carry out the operation
            Dim pos As Geoposition = Await geolocator.GetGeopositionAsync().AsTask(token)

            rootPage.NotifyUser("Initial position. Waiting for update...", NotifyType.StatusMessage)

            ScenarioOutput_Latitude.Text = pos.Coordinate.Point.Position.Latitude.ToString()
            ScenarioOutput_Longitude.Text = pos.Coordinate.Point.Position.Longitude.ToString()
            ScenarioOutput_Accuracy.Text = pos.Coordinate.Accuracy.ToString()
        Catch e1 As UnauthorizedAccessException
            rootPage.NotifyUser("Disabled by user. Enable access through the settings charm to enable the background task.", NotifyType.StatusMessage)

            ScenarioOutput_Latitude.Text = "No data"
            ScenarioOutput_Longitude.Text = "No data"
            ScenarioOutput_Accuracy.Text = "No data"
        Catch e2 As TaskCanceledException
            rootPage.NotifyUser("Initial position operation canceled. Waiting for update...", NotifyType.StatusMessage)
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
    End Sub

    ''' <summary>
    ''' Helper method to cancel the GetGeopositionAsync request (if any).
    ''' </summary>
    Private Sub CancelGetGeoposition()
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts = Nothing
        End If
    End Sub

    Private Async Sub OnCompleted(ByVal sender As IBackgroundTaskRegistration, ByVal e As BackgroundTaskCompletedEventArgs)
        If sender IsNot Nothing Then
            ' Update the UI with progress reported by the background task
            Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                         ' If the background task threw an exception, display the exception in
                                                                         ' the error text box.
                                                                         ' Update the UI with the completion status of the background task
                                                                         ' The Run method of the background task sets this status. 
                                                                         ' Extract and display Latitude
                                                                         ' Extract and display Longitude
                                                                         ' Extract and display Accuracy
                                                                         ' The background task had an error
                                                                         Try
                                                                             e.CheckResult()
                                                                             Dim settings = ApplicationData.Current.LocalSettings
                                                                             If settings.Values("Status") IsNot Nothing Then
                                                                                 rootPage.NotifyUser(settings.Values("Status").ToString(), NotifyType.StatusMessage)
                                                                             End If
                                                                             If settings.Values("Latitude") IsNot Nothing Then
                                                                                 ScenarioOutput_Latitude.Text = settings.Values("Latitude").ToString()
                                                                             Else
                                                                                 ScenarioOutput_Latitude.Text = "No data"
                                                                             End If
                                                                             If settings.Values("Longitude") IsNot Nothing Then
                                                                                 ScenarioOutput_Longitude.Text = settings.Values("Longitude").ToString()
                                                                             Else
                                                                                 ScenarioOutput_Longitude.Text = "No data"
                                                                             End If
                                                                             If settings.Values("Accuracy") IsNot Nothing Then
                                                                                 ScenarioOutput_Accuracy.Text = settings.Values("Accuracy").ToString()
                                                                             Else
                                                                                 ScenarioOutput_Accuracy.Text = "No data"
                                                                             End If
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
End Class



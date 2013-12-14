' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.ApplicationModel.Background
Imports Windows.Storage
Imports System.Threading
Imports Windows.System.Threading

'
' A background task always implements the IBackgroundTask interface.
'
Public NotInheritable Class SampleBackgroundTask
    Implements IBackgroundTaskInstance


    Private _cancelReason As BackgroundTaskCancellationReason = BackgroundTaskCancellationReason.Abort
    Private _cancelRequested As Boolean = False
    Private _deferral As BackgroundTaskDeferral = Nothing
    Private _periodicTimer As ThreadPoolTimer = Nothing
    Private progressPrivate As UInteger = 0
    Private _taskInstance As IBackgroundTaskInstance = Nothing

    '
    ' The Run method is the entry point of a background task.
    '
    Public Sub Run(ByVal taskInstance As IBackgroundTaskInstance)
        Debug.WriteLine("Background " & taskInstance.Task.Name & " Starting...")

        '
        ' Query BackgroundWorkCost
        ' Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
        ' of work in the background task and return immediately.
        '
        Dim cost = BackgroundWorkCost.CurrentBackgroundWorkCost
        Dim settings = ApplicationData.Current.LocalSettings
        settings.Values("BackgroundWorkCost") = cost.ToString()

        '
        ' Associate a cancellation handler with the background task.
        '
        AddHandler taskInstance.Canceled, AddressOf OnCanceled

        '
        ' Get the deferral object from the task instance, and take a reference to the taskInstance;
        '
        _deferral = taskInstance.GetDeferral()
        _taskInstance = taskInstance

        _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(New TimerElapsedHandler(AddressOf PeriodicTimerCallback), TimeSpan.FromSeconds(1))
    End Sub

    '
    ' Handles background task cancellation.
    '
    Private Sub OnCanceled(ByVal sender As IBackgroundTaskInstance, ByVal reason As BackgroundTaskCancellationReason)
        '
        ' Indicate that the background task is canceled.
        '
        Volatile.Write(_cancelRequested, True)
        _cancelReason = reason

        Debug.WriteLine("Background " & sender.Task.Name & " Cancel Requested...")
    End Sub

    '
    ' Simulate the background task activity.
    '
    Private Sub PeriodicTimerCallback(ByVal timer As ThreadPoolTimer)
        If (Volatile.Read(_cancelRequested) = False) AndAlso (_Progress < 100) Then
            _Progress += CUInt(10)
            _taskInstance.Progress = _Progress
        Else
            _periodicTimer.Cancel()

            Dim settings = ApplicationData.Current.LocalSettings
            Dim key = _taskInstance.Task.Name

            '
            ' Write to LocalSettings to indicate that this background task ran.
            '
            settings.Values(key) = If(_Progress < 100, "Canceled with reason: " & _cancelReason.ToString(), "Completed")
            Debug.WriteLine("Background " & _taskInstance.Task.Name + CStr(settings.Values(key)))

            '
            ' Indicate that the background task has completed.
            '
            _deferral.Complete()
        End If
    End Sub

    Public Event Canceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason) Implements IBackgroundTaskInstance.Canceled

    Public Function GetDeferral() As BackgroundTaskDeferral Implements IBackgroundTaskInstance.GetDeferral
        Return GetDeferral()
    End Function

    Public ReadOnly Property InstanceId As Guid Implements IBackgroundTaskInstance.InstanceId
        Get

        End Get
    End Property

    Public Property Progress As UInteger Implements IBackgroundTaskInstance.Progress

    Public ReadOnly Property SuspendedCount As UInteger Implements IBackgroundTaskInstance.SuspendedCount
        Get
            Return SuspendedCount
        End Get
    End Property

    Public ReadOnly Property Task As BackgroundTaskRegistration Implements IBackgroundTaskInstance.Task
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property TriggerDetails As Object Implements IBackgroundTaskInstance.TriggerDetails
        Get
            Return Nothing
        End Get
    End Property
End Class
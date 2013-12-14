' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Networking.PushNotifications
Imports Windows.ApplicationModel.Background

Partial Public Class App
    Public CurrentChannel As PushNotificationChannel = Nothing

    Private Const PUSH_NOTIFICATIONS_TASK_NAME As String = "UpdateChannels"
    Private Const PUSH_NOTIFICATIONS_TASK_ENTRY_POINT As String = "PushNotificationsHelper.MaintenanceTask"
    Private Const MAINTENANCE_INTERVAL As Integer = 10 * 24 * 60 ' Check for channels that need to be updated every 10 days

    Public Sub New()
        InitializeComponent()
        AddHandler Suspending, AddressOf OnSuspending

        If Not IsTaskRegistered() Then
            Dim taskBuilder As New BackgroundTaskBuilder()
            Dim trigger As New MaintenanceTrigger(MAINTENANCE_INTERVAL, False)
            taskBuilder.SetTrigger(trigger)
            taskBuilder.TaskEntryPoint = PUSH_NOTIFICATIONS_TASK_ENTRY_POINT
            taskBuilder.Name = PUSH_NOTIFICATIONS_TASK_NAME

            Dim internetCondition As New SystemCondition(SystemConditionType.InternetAvailable)
            taskBuilder.AddCondition(internetCondition)

            taskBuilder.Register()
        End If
    End Sub

    Private Function IsTaskRegistered() As Boolean
        For Each task In BackgroundTaskRegistration.AllTasks.Values
            If task.Name = PUSH_NOTIFICATIONS_TASK_NAME Then
                Return True
            End If
        Next task
        Return False
    End Function

    Protected Async Sub OnSuspending(ByVal sender As Object, ByVal args As SuspendingEventArgs)
        Dim deferral As SuspendingDeferral = args.SuspendingOperation.GetDeferral()
        Await SuspensionManager.SaveAsync()
        deferral.Complete()
    End Sub

    Protected Overrides Async Sub OnLaunched(ByVal args As LaunchActivatedEventArgs)
        If args.PreviousExecutionState = ApplicationExecutionState.Terminated Then
            '     Do an asynchronous restore
            Await SuspensionManager.RestoreAsync()
        End If
        Dim rootFrame = New Frame()
        rootFrame.Navigate(GetType(MainPage))
        Window.Current.Content = rootFrame
        Dim p As MainPage = TryCast(rootFrame.Content, MainPage)
        p.RootNamespace = Me.GetType().Namespace
        Window.Current.Activate()
    End Sub
End Class


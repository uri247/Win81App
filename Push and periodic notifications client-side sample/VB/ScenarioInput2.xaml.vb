' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports PushNotificationsHelper
Imports Windows.Networking.PushNotifications
Imports Windows.ApplicationModel.Background

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private Const PUSH_NOTIFICATIONS_TASK_NAME As String = "UpdateChannels"
    Private Const PUSH_NOTIFICATIONS_TASK_ENTRY_POINT As String = "PushNotificationsHelper.MaintenanceTask"
    Private Const MAINTENANCE_INTERVAL As Integer = 10 * 24 * 60 ' Check for channels that need to be updated every 10 days*/

    Public Sub New()
        InitializeComponent()


    End Sub


    Private Async Sub RenewChannelsButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            ' The Notifier object allows us to use the same code in the maintenance task and this foreground application
            Await rootPage.Notifier.RenewAllAsync(True)
            rootPage.NotifyUser("Channels renewed successfully", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser("Channels renewal failed: " & ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub RegisterTaskButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If GetRegisteredTask() Is Nothing Then
            Dim taskBuilder As New BackgroundTaskBuilder()
            Dim trigger As New MaintenanceTrigger(MAINTENANCE_INTERVAL, False)
            taskBuilder.SetTrigger(trigger)
            taskBuilder.TaskEntryPoint = PUSH_NOTIFICATIONS_TASK_ENTRY_POINT
            taskBuilder.Name = PUSH_NOTIFICATIONS_TASK_NAME

            Dim internetCondition As New SystemCondition(SystemConditionType.InternetAvailable)
            taskBuilder.AddCondition(internetCondition)

            Try
                taskBuilder.Register()
                rootPage.NotifyUser("Task registered", NotifyType.StatusMessage)
            Catch ex As Exception
                rootPage.NotifyUser("Error registering task: " & ex.Message, NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("Task already registered", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Sub UnregisterTaskButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim task As IBackgroundTaskRegistration = GetRegisteredTask()
        If task IsNot Nothing Then
            task.Unregister(True)
            rootPage.NotifyUser("Task unregistered", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Task not registered", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Function GetRegisteredTask() As IBackgroundTaskRegistration
        For Each task In BackgroundTaskRegistration.AllTasks.Values
            If task.Name = PUSH_NOTIFICATIONS_TASK_NAME Then
                Return task
            End If
        Next task
        Return Nothing
    End Function


    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded

        If rootPage.Notifier Is Nothing Then
            rootPage.Notifier = New Notifier()
        End If
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(ByVal sender As Object, ByVal e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = CType(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;
    End Sub

#End Region
End Class


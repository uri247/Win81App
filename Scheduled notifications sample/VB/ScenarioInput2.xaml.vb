' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports Windows.UI.Notifications

Friend Class NotificationData
    Public Property ItemType() As String
    Public Property ItemId() As String
    Public Property DueTime() As String
    Public Property InputString() As String
    Public Property IsTile() As Boolean
End Class

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler RefreshListButton.Click, AddressOf RefreshList_Click
        AddHandler RemoveButton.Click, AddressOf Remove_Click
    End Sub

    ' Remove the notification by checking the list of scheduled notifications for a notification with matching ID.
    ' While it would be possible to manage the notifications by storing a reference to each notification, such practice
    ' causes memory leaks by not allowing the notifications to be collected once they have shown.
    ' It's important to create unique IDs for each notification if they are to be managed later.
    Private Sub Remove_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim items As IList(Of Object) = ItemGridView.SelectedItems
        For i As Integer = 0 To items.Count - 1
            Dim item As NotificationData = CType(items(i), NotificationData)
            Dim itemId As String = item.ItemId
            If item.IsTile Then
                Dim updater As TileUpdater = TileUpdateManager.CreateTileUpdaterForApplication()
                Dim scheduled As IReadOnlyList(Of ScheduledTileNotification) = updater.GetScheduledTileNotifications()
                For j As Integer = 0 To scheduled.Count - 1
                    If scheduled(j).Id = itemId Then
                        updater.RemoveFromSchedule(scheduled(j))
                    End If
                Next j
            Else
                Dim notifier As ToastNotifier = ToastNotificationManager.CreateToastNotifier()
                Dim scheduled As IReadOnlyList(Of ScheduledToastNotification) = notifier.GetScheduledToastNotifications()
                For j As Integer = 0 To scheduled.Count - 1
                    If scheduled(j).Id = itemId Then
                        notifier.RemoveFromSchedule(scheduled(j))
                    End If
                Next j
            End If
        Next i
        rootPage.NotifyUser("Removed selected scheduled notifications", NotifyType.StatusMessage)
        RefreshListView()
    End Sub

    Private Sub RefreshList_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        RefreshListView()
    End Sub


    Private Sub RefreshListView()
        Dim scheduledToasts As IReadOnlyList(Of ScheduledToastNotification) = ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications()
        Dim scheduledTiles As IReadOnlyList(Of ScheduledTileNotification) = TileUpdateManager.CreateTileUpdaterForApplication().GetScheduledTileNotifications()

        Dim toastLength As Integer = scheduledToasts.Count
        Dim tileLength As Integer = scheduledTiles.Count

        Dim bindingList As New List(Of NotificationData)(toastLength + tileLength)
        For i As Integer = 0 To toastLength - 1
            Dim toast As ScheduledToastNotification = scheduledToasts(i)

            bindingList.Add(New NotificationData() With {.ItemType = "Toast", .ItemId = toast.Id, .DueTime = toast.DeliveryTime.ToLocalTime().ToString(), .InputString = toast.Content.GetElementsByTagName("text")(0).InnerText, .IsTile = False})
        Next i

        For i As Integer = 0 To tileLength - 1
            Dim tile As ScheduledTileNotification = scheduledTiles(i)

            bindingList.Add(New NotificationData() With {.ItemType = "Tile", .ItemId = tile.Id, .DueTime = tile.DeliveryTime.ToLocalTime().ToString(), .InputString = tile.Content.GetElementsByTagName("text")(0).InnerText, .IsTile = False})
        Next i

        ItemGridView.ItemsSource = bindingList
    End Sub


    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
        RefreshListView()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
        ItemGridView.ItemsSource = Nothing
    End Sub
End Class

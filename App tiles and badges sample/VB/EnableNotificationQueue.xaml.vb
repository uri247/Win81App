'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports NotificationsExtensions.TileContent
Imports Windows.UI.Notifications

Partial Public NotInheritable Class EnableNotificationQueue
    Inherits SDKTemplate.Common.LayoutAwarePage

#Region "TemplateCode"
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub
#End Region ' TemplateCode

    Private Sub ClearTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Tile cleared"
    End Sub

    Private Sub UpdateTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create a notification for the Square310x310 tile using one of the available templates for the size.
        Dim square310x310TileContent As ITileSquare310x310Text09 = TileContentFactory.CreateTileSquare310x310Text09()
        square310x310TileContent.TextHeadingWrap.Text = TextContent.Text

        ' Create a notification for the Wide310x150 tile using one of the available templates for the size.
        Dim wide310x150TileContent As ITileWide310x150Text03 = TileContentFactory.CreateTileWide310x150Text03()
        wide310x150TileContent.TextHeadingWrap.Text = TextContent.Text

        ' Create a notification for the Square150x150 tile using one of the available templates for the size.
        Dim square150x150TileContent As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        square150x150TileContent.TextBodyWrap.Text = TextContent.Text

        ' Attach the Square150x150 template to the Wide310x150 template.
        wide310x150TileContent.Square150x150Content = square150x150TileContent

        ' Attach the Wide310x150 template to the Square310x310 template.
        square310x310TileContent.Wide310x150Content = wide310x150TileContent

        Dim tileNotification As TileNotification = square310x310TileContent.CreateNotification()

        Dim tag As String = "TestTag01"
        If Not Id.Text.Equals(String.Empty) Then
            tag = Id.Text
        End If

        ' Set the tag on the notification.
        tileNotification.Tag = tag
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        OutputTextBlock.Text = "Tile notification sent. It is tagged with '" & tag & "'." & vbLf & MainPage.PrettyPrint(square310x310TileContent.GetContent())
    End Sub

    Private Sub EnableNotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Enable the notification queue - this only needs to be called once in the lifetime of your app.
        ' Note that the default is false.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(True)
        OutputTextBlock.Text = "Notification cycling enabled for all tile sizes."
    End Sub

    Private Sub DisableNotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Disable the notification queue.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(False)
        OutputTextBlock.Text = "Notification cycling disabled for all tile sizes."
    End Sub

    Private Sub EnableSquare150x150NotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Enable the notification queue for the medium (Square150x150) tile size, without affecting the setting for the other tile sizes.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare150x150(True)
        OutputTextBlock.Text = "Notification cycling enabled for medium (Square150x150) tiles."
    End Sub

    Private Sub DisableSquare150x150NotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Disable the notification queue for the medium (Square150x150) tile size, without affecting the setting for the other tile sizes.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare150x150(False)
        OutputTextBlock.Text = "Notification cycling disabled for medium (Square150x150) tiles."
    End Sub

    Private Sub EnableWide310x150NotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Enable the notification queue for the wide (Wide310x150) tile size, without affecting the setting for the other tile sizes.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForWide310x150(True)
        OutputTextBlock.Text = "Notification cycling enabled for wide (Wide310x150) tiles."
    End Sub

    Private Sub DisableWide310x150NotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Disable the notification queue for the wide (Wide310x150) tile size, without affecting the setting for the other tile sizes.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForWide310x150(False)
        OutputTextBlock.Text = "Notification cycling disabled for wide (Wide310x150) tiles."
    End Sub

    Private Sub EnableSquare310x310NotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Enable the notification queue for the large (Square310x310)tile size, without affecting the setting for the other tile sizes.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(True)
        OutputTextBlock.Text = "Notification cycling enabled for large (Square310x310) tiles."
    End Sub

    Private Sub DisableSquare310x310NotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Disable the notification queue for the large (Square310x310) tile size, without affecting the setting for the other tile sizes.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(False)
        OutputTextBlock.Text = "Notification cycling disabled for large (Square310x310) tiles."
    End Sub
End Class

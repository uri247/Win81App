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

Partial Public NotInheritable Class ContentDeduplication
    Inherits SDKTemplate.Common.LayoutAwarePage

#Region "TemplateCode"
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub
#End Region ' TemplateCode

    Private Sub EnableNotificationQueue_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Enable the notification queue - this only needs to be called once in the lifetime of your app.
        ' Note that the default is false.
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(True)
        OutputTextBlock.Text = "Notification cycling enabled for all tile sizes."
    End Sub

    Private Sub ClearTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Tile cleared"
    End Sub

    Private Sub SendNotifications_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create a notification for the first set of stories with bindings for all 3 tile sizes
        Dim square310x310TileContent1 As ITileSquare310x310Text09 = TileContentFactory.CreateTileSquare310x310Text09()
        square310x310TileContent1.TextHeadingWrap.Text = "Main Story"

        Dim wide310x150TileContent1 As ITileWide310x150Text03 = TileContentFactory.CreateTileWide310x150Text03()
        wide310x150TileContent1.TextHeadingWrap.Text = "Main Story"

        Dim square150x150TileContent1 As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        square150x150TileContent1.TextBodyWrap.Text = "Main Story"

        wide310x150TileContent1.Square150x150Content = square150x150TileContent1
        square310x310TileContent1.Wide310x150Content = wide310x150TileContent1

        ' Set the contentId on the Square310x310 tile
        square310x310TileContent1.ContentId = "Main_1"

        ' Tag the notification and send it to the tile
        Dim tileNotification As TileNotification = square310x310TileContent1.CreateNotification()
        tileNotification.Tag = "1"
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        ' Create the first notification for the second set of stories with binding for all 3 tiles sizes
        Dim square310x310TileContent2 As ITileSquare310x310TextList03 = TileContentFactory.CreateTileSquare310x310TextList03()
        square310x310TileContent2.TextHeading1.Text = "Additional Story 1"
        square310x310TileContent2.TextHeading2.Text = "Additional Story 2"
        square310x310TileContent2.TextHeading3.Text = "Additional Story 3"

        Dim wide310x150TileContent2 As ITileWide310x150Text03 = TileContentFactory.CreateTileWide310x150Text03()
        wide310x150TileContent2.TextHeadingWrap.Text = "Additional Story 1"

        Dim square150x150TileContent2 As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        square150x150TileContent2.TextBodyWrap.Text = "Additional Story 1"

        wide310x150TileContent2.Square150x150Content = square150x150TileContent2
        square310x310TileContent2.Wide310x150Content = wide310x150TileContent2

        ' Set the contentId on the Square310x310 tile
        square310x310TileContent2.ContentId = "Additional_1"

        ' Tag the notification and send it to the tile
        tileNotification = square310x310TileContent2.CreateNotification()
        tileNotification.Tag = "2"
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        ' Create the second notification for the second set of stories with binding for all 3 tiles sizes
        ' Notice that we only replace the Wide310x150 and Square150x150 binding elements,
        ' and keep the Square310x310 content the same - this will cause the Square310x310 to be ignored for this notification,
        ' since the contentId for this size is the same as in the first notification of the second set of stories.
        Dim wide310x150TileContent3 As ITileWide310x150Text03 = TileContentFactory.CreateTileWide310x150Text03()
        wide310x150TileContent3.TextHeadingWrap.Text = "Additional Story 2"

        Dim square150x150TileContent3 As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        square150x150TileContent3.TextBodyWrap.Text = "Additional Story 2"

        wide310x150TileContent3.Square150x150Content = square150x150TileContent3
        square310x310TileContent2.Wide310x150Content = wide310x150TileContent3

        ' Tag the notification and send it to the tile
        tileNotification = square310x310TileContent2.CreateNotification()
        tileNotification.Tag = "3"
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        ' Create the third notification for the second set of stories with binding for all 3 tiles sizes
        ' Notice that we only replace the Wide310x150 and Square150x150 binding elements,
        ' and keep the Square310x310 content the same again - this will cause the Square310x310 to be ignored for this notification,
        ' since the contentId for this size is the same as in the first notification of the second set of stories.
        Dim wide310x150TileContent4 As ITileWide310x150Text03 = TileContentFactory.CreateTileWide310x150Text03()
        wide310x150TileContent4.TextHeadingWrap.Text = "Additional Story 3"

        Dim square150x150TileContent4 As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        square150x150TileContent4.TextBodyWrap.Text = "Additional Story 3"

        wide310x150TileContent4.Square150x150Content = square150x150TileContent4
        square310x310TileContent2.Wide310x150Content = wide310x150TileContent4

        ' Tag the notification and send it to the tile
        tileNotification = square310x310TileContent2.CreateNotification()
        tileNotification.Tag = "4"
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        OutputTextBlock.Text = "Four notifications sent"
    End Sub
End Class

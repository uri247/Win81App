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

Partial Public NotInheritable Class NotificationExpiration
    Inherits SDKTemplate.Common.LayoutAwarePage

#Region "TemplateCode"
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

#End Region ' TemplateCode

    Private Sub UpdateTileExpiring_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim seconds As Integer
        If Not Int32.TryParse(Time.Text, seconds) Then
            seconds = 10
        End If

        Dim cal As New Windows.Globalization.Calendar()
        cal.SetToNow()
        cal.AddSeconds(seconds)

        Dim longTime = New Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime")
        Dim expiryTime As DateTimeOffset = cal.GetDateTime()
        Dim expiryTimeString As String = longTime.Format(expiryTime)

        ' Create a notification for the Square310x310 tile using one of the available templates for the size.
        Dim tileSquare310x310Content As ITileSquare310x310Text09 = TileContentFactory.CreateTileSquare310x310Text09()
        tileSquare310x310Content.TextHeadingWrap.Text = "This notification will expire at " & expiryTimeString

        ' Create a notification for the Wide310x150 tile using one of the available templates for the size.
        Dim wide310x150TileContent As ITileWide310x150Text04 = TileContentFactory.CreateTileWide310x150Text04()
        wide310x150TileContent.TextBodyWrap.Text = "This notification will expire at " & expiryTimeString

        ' Create a notification for the Square150x150 tile using one of the available templates for the size.
        Dim square150x150TileContent As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        square150x150TileContent.TextBodyWrap.Text = "This notification will expire at " & expiryTimeString

        ' Attach the Square150x150 template to the Wide310x150 template.
        wide310x150TileContent.Square150x150Content = square150x150TileContent

        ' Attach the Wide310x150 template to the Square310x310 template.
        tileSquare310x310Content.Wide310x150Content = wide310x150TileContent

        Dim tileNotification As TileNotification = tileSquare310x310Content.CreateNotification()

        ' Set the expiration time and update the tile.
        tileNotification.ExpirationTime = expiryTime
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        OutputTextBlock.Text = MainPage.PrettyPrint(tileSquare310x310Content.GetContent())
    End Sub
End Class

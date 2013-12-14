'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
Imports Windows.UI.StartScreen

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SecondaryTileNotification
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private appBar As AppBar

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Preserve the app bar
        appBar = rootPage.BottomAppBar
        ' this ensures the app bar is not shown in this scenario
        rootPage.BottomAppBar = Nothing
        ToggleButtons(SecondaryTile.Exists(MainPage.dynamicTileId))
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be navigated out in a Frame
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatingFrom(ByVal e As NavigatingCancelEventArgs)
        ' Restore the app bar
        rootPage.BottomAppBar = appBar
    End Sub


    ''' <summary>
    ''' Enables or disables the notification buttons
    ''' </summary>
    ''' <param name="isEnabled"> enables if true</param>
    Private Sub ToggleButtons(ByVal isEnabled As Boolean)
        SendBadgeNotification.IsEnabled = isEnabled
        SendTileNotification.IsEnabled = isEnabled
        SendBadgeNotificationString.IsEnabled = isEnabled
        SendTileNotificationString.IsEnabled = isEnabled
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Pin Tile' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub PinLiveTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            ' Prepare the images for our tile to be pinned.
            Dim square150x150Logo As New Uri("ms-appx:///Assets/square150x150Tile-sdk.png")
            Dim wide310x150Logo As New Uri("ms-appx:///Assets/wide310x150Tile-sdk.png")

            ' During creation of the secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
            ' These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
            Dim tileActivationArguments As String = MainPage.dynamicTileId & " WasPinnedAt=" & Date.Now.ToLocalTime().ToString()

            ' Create a Secondary tile with all the required properties and sets perfered size to Wide310x150.
            Dim secondaryTile As New SecondaryTile(MainPage.dynamicTileId, "A Live Secondary Tile", tileActivationArguments, square150x150Logo, TileSize.Wide310x150)

            ' Adding the wide tile logo.
            secondaryTile.VisualElements.Wide310x150Logo = wide310x150Logo

            ' The display of the app name can be controlled for each tile size.
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = True
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = True

            ' Specify a foreground text value.
            ' The tile background color is inherited from the parent unless a separate value is specified.
            secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark

            ' OK, the tile is created and we can now attempt to pin the tile.
            ' Note that the status message is updated when the async operation to pin the tile completes.
            Dim isPinned As Boolean = Await secondaryTile.RequestCreateForSelectionAsync(MainPage.GetElementRect(CType(sender, FrameworkElement)), Windows.UI.Popups.Placement.Below)

            If isPinned Then
                rootPage.NotifyUser("Secondary tile successfully pinned.", NotifyType.StatusMessage)
                ToggleButtons(True)
            Else
                rootPage.NotifyUser("Secondary tile not pinned.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Sending tile notification' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SendTileNotification_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            If SecondaryTile.Exists(MainPage.dynamicTileId) Then
                ' Note: This sample contains an additional reference, NotificationsExtensions, which you can use in your apps
                Dim tileContent As ITileWide310x150Text04 = TileContentFactory.CreateTileWide310x150Text04()
                tileContent.TextBodyWrap.Text = "Sent to a secondary tile from NotificationsExtensions!"

                Dim squareContent As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
                squareContent.TextBodyWrap.Text = "Sent to a secondary tile from NotificationExtensions!"
                tileContent.Square150x150Content = squareContent

                ' Send the notification to the secondary tile by creating a secondary tile updater
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(tileContent.CreateNotification())

                rootPage.NotifyUser("Tile notification sent to " & MainPage.dynamicTileId, NotifyType.StatusMessage)
            Else
                ToggleButtons(False)
                rootPage.NotifyUser(MainPage.dynamicTileId & " not pinned.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Other' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SendBadgeNotification_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            If SecondaryTile.Exists(MainPage.dynamicTileId) Then
                Dim badgeContent As New BadgeNumericNotificationContent(6)

                ' Send the notification to the secondary tile
                BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(badgeContent.CreateNotification())

                rootPage.NotifyUser("Badge notification sent to " & MainPage.dynamicTileId, NotifyType.StatusMessage)
            Else
                ToggleButtons(False)
                rootPage.NotifyUser(MainPage.dynamicTileId & " not pinned.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub

    Private Sub SendTileNotificationWithStringManipulation_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            Dim tileXmlString As String = "<tile>" & "<visual version='2'>" & "<binding template='TileWide310x150Text04' fallback='TileWideText04'>" & "<text id='1'>Send to a secondary tile from strings</text>" & "</binding>" & "<binding template='TileSquare150x150Text04' fallback='TileSquareText04'>" & "<text id='1'>Send to a secondary tile from strings</text>" & "</binding>" & "</visual>" & "</tile>"

            Dim tileDOM As New Windows.Data.Xml.Dom.XmlDocument()
            tileDOM.LoadXml(tileXmlString)
            Dim tile As New TileNotification(tileDOM)

            ' Send the notification to the secondary tile by creating a secondary tile updater
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(tile)

            rootPage.NotifyUser("Tile notification sent to " & MainPage.dynamicTileId, NotifyType.StatusMessage)
        End If
    End Sub

    Private Sub SendBadgeNotificationWithStringManipulation_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            Dim badgeXmlString As String = "<badge value='9'/>"
            Dim badgeDOM As New Windows.Data.Xml.Dom.XmlDocument()
            badgeDOM.LoadXml(badgeXmlString)
            Dim badge As New BadgeNotification(badgeDOM)

            ' Send the notification to the secondary tile
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(badge)

            rootPage.NotifyUser("Badge notification sent to " & MainPage.dynamicTileId, NotifyType.StatusMessage)
        End If
    End Sub
End Class

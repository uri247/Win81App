'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.StartScreen

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PinTile
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
    ''' This is the click handler for the 'PinButton' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub PinButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            ' Prepare package images for all four tile sizes in our tile to be pinned as well as for the square30x30 logo used in the Apps view.  
            Dim square70x70Logo As New Uri("ms-appx:///Assets/square70x70Tile-sdk.png")
            Dim square150x150Logo As New Uri("ms-appx:///Assets/square150x150Tile-sdk.png")
            Dim wide310x150Logo As New Uri("ms-appx:///Assets/wide310x150Tile-sdk.png")
            Dim square310x310Logo As New Uri("ms-appx:///Assets/square310x310Tile-sdk.png")
            Dim square30x30Logo As New Uri("ms-appx:///Assets/square30x30Tile-sdk.png")

            ' During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
            ' These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
            Dim tileActivationArguments As String = MainPage.logoSecondaryTileId & " WasPinnedAt=" & Date.Now.ToLocalTime().ToString()

            ' Create a Secondary tile with all the required arguments.
            ' Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
            ' It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
            ' If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.
            ' TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise. 
            Dim secondaryTile As New SecondaryTile(MainPage.logoSecondaryTileId, "Title text shown on the tile", tileActivationArguments, square150x150Logo, TileSize.Square150x150)

            ' Only support of the small and medium tile sizes is mandatory. 
            ' To have the larger tile sizes available the assets must be provided.
            secondaryTile.VisualElements.Wide310x150Logo = wide310x150Logo
            secondaryTile.VisualElements.Square310x310Logo = square310x310Logo

            ' If the asset for the small tile size is not provided, it will be created by scaling down the medium tile size asset.
            ' Thus, providing the asset for the small tile size is not mandatory, though is recommended to avoid scaling artifacts and can be overridden as shown below. 
            secondaryTile.VisualElements.Square70x70Logo = square70x70Logo

            ' Like the background color, the square30x30 logo is inherited from the parent application tile by default. 
            ' Let's override it, just to see how that's done.
            secondaryTile.VisualElements.Square30x30Logo = square30x30Logo

            ' The display of the secondary tile name can be controlled for each tile size.
            ' The default is false.
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = False
            secondaryTile.VisualElements.ShowNameOnWide310x150Logo = True
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = True

            ' Specify a foreground text value.
            ' The tile background color is inherited from the parent unless a separate value is specified.
            secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark

            ' Set this to false if roaming doesn't make sense for the secondary tile.
            ' The default is true;
            secondaryTile.RoamingEnabled = False

            ' OK, the tile is created and we can now attempt to pin the tile.
            ' Note that the status message is updated when the async operation to pin the tile completes.
            Dim isPinned As Boolean = Await secondaryTile.RequestCreateForSelectionAsync(MainPage.GetElementRect(CType(sender, FrameworkElement)), Windows.UI.Popups.Placement.Below)

            If isPinned Then
                rootPage.NotifyUser("Secondary tile successfully pinned.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Secondary tile not pinned.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub
End Class


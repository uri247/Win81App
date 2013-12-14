'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class TilePinned
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
    ''' This is the click handler for the 'TileExists' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TileExists_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            If Windows.UI.StartScreen.SecondaryTile.Exists(MainPage.logoSecondaryTileId) Then
                rootPage.NotifyUser(MainPage.logoSecondaryTileId & " is pinned.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser(MainPage.logoSecondaryTileId & " not currently pinned.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub
End Class


'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Text
Imports Windows.UI.StartScreen

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class EnumerateTiles
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
    ''' This is the click handler for the 'Enumerate tile' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub EnumerateSecondaryTiles_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            ' Get secondary tile ids for this package
            Dim tilelist As IReadOnlyList(Of SecondaryTile) = Await Windows.UI.StartScreen.SecondaryTile.FindAllAsync()
            If tilelist.Count > 0 Then
                Dim count As Integer = 0
                Dim outputText As New StringBuilder()
                For Each tile In tilelist
                    outputText.AppendFormat("Tile Id[{0}] = {1}, Tile short display name = {2}  {3}", count, tile.TileId, tile.DisplayName, System.Environment.NewLine)
                    count += 1
                Next tile
                rootPage.NotifyUser(outputText.ToString(), NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("No secondary tiles are available for this appId.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub
End Class


'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Secondary Tile VB"

        #Region "Secondary Tile scenario specific variables"

        Public Const logoSecondaryTileId As String = "SecondaryTile.Logo"
        Public Const dynamicTileId As String = "SecondaryTile.LiveTile"
        Public Const appbarTileId As String = "SecondaryTile.AppBar"

        #End Region


        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Pin Tile", .ClassType = GetType(SecondaryTiles.PinTile)}, _
            New Scenario() With {.Title = "Unpin Tile", .ClassType = GetType(SecondaryTiles.UnpinTile)}, _
            New Scenario() With {.Title = "Enumerate Tiles", .ClassType = GetType(SecondaryTiles.EnumerateTiles)}, _
            New Scenario() With {.Title = "Is Tile Pinned?", .ClassType = GetType(SecondaryTiles.TilePinned)}, _
            New Scenario() With {.Title = "Show Activation Arguments", .ClassType = GetType(SecondaryTiles.LaunchedFromSecondaryTile)}, _
            New Scenario() With {.Title = "Secondary Tile Notifications", .ClassType = GetType(SecondaryTiles.SecondaryTileNotification)}, _
            New Scenario() With {.Title = "Pin/Unpin Through Appbar", .ClassType = GetType(SecondaryTiles.PinFromAppbar)}, _
            New Scenario() With {.Title = "Update Secondary Tile Default Logo", .ClassType = GetType(SecondaryTiles.UpdateAsync)}, _
            New Scenario() With {.Title = "Pin Tile Alternate Visual Elements", .ClassType = GetType(SecondaryTiles.PinTileAlternateVisualElements)}, _
            New Scenario() With {.Title = "Pin Tile Alternate Visual Elements Async", .ClassType = GetType(SecondaryTiles.PinTileAlternateVisualElementsAsync)} _
        }


#Region "Secondary Tile specific methods"

        ' Gets the rectangle of the element
        Public Shared Function GetElementRect(ByVal element As FrameworkElement) As Rect
            Dim buttonTransform As GeneralTransform = element.TransformToVisual(Nothing)
            Dim point As Point = buttonTransform.TransformPoint(New Point())
            Return New Rect(point, New Size(element.ActualWidth, element.ActualHeight))
        End Function

        ' Navigates to the Scenario "Show Activation Arguments"
        Public Sub NavigateToLaunchedFromSecondaryTile()
            Dim index As Integer = -1
            ' Populate the ListBox with the list of scenarios as defined in Constants.vb.
            For Each s As Scenario In scenariosList
                index += 1
                If s.ClassType Is GetType(SecondaryTiles.LaunchedFromSecondaryTile) Then
                    Exit For
                End If
            Next s

            SuspensionManager.SessionState("SelectedScenario") = index
            Scenarios.SelectedIndex = index
            LoadScenario(scenariosList(index).ClassType)
            InvalidateSize()
        End Sub

#End Region

    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

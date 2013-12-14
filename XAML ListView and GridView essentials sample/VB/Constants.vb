'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports ListViewSimple

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML ListView and GridView Essentials Sample (VB)"

        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Instantiating a GridView", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Responding to click events", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "Instantiating a ListView", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Retemplating GridViewItems", .ClassType = GetType(Scenario4)}, _
            New Scenario() With {.Title = "Retemplating ListViewItems", .ClassType = GetType(Scenario5)}, _
            New Scenario() With {.Title = "Custom item container template structure", .ClassType = GetType(Scenario6)} _
        }
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

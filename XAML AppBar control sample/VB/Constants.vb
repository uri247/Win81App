'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports AppBarControl

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Windows.UI.Xaml.Controls.Page

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML AppBar control sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenarios As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Create an AppBar", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Customize AppBar color", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "Customize icons", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Using CommandBar", .ClassType = GetType(Scenario4)}, _
            New Scenario() With {.Title = "Custom content", .ClassType = GetType(Scenario5)}, _
            New Scenario() With {.Title = "Control the AppBar and commands", .ClassType = GetType(Scenario6)}, _
            New Scenario() With {.Title = "Show contextual commands for a GridView", .ClassType = GetType(Scenario7)}, _
            New Scenario() With {.Title = "Localizing AppBar commands", .ClassType = GetType(Scenario8)} _
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

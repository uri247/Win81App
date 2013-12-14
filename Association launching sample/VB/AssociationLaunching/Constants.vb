'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports AssociationLaunching

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Association Launching Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Launching a file", .ClassType = GetType(LaunchFile)}, _
            New Scenario() With {.Title = "Launching a URI", .ClassType = GetType(LaunchUri)}, _
            New Scenario() With {.Title = "Receiving a file", .ClassType = GetType(ReceiveFile)}, _
            New Scenario() With {.Title = "Receiving a URI", .ClassType = GetType(ReceiveUri)} _
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

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports WebAuthentication

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Web Authentication Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Connect to Facebook Services", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Connect to Twitter Services", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "Connect to Flickr Services", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Connect to Google Services", .ClassType = GetType(Scenario4)}, _
            New Scenario() With {.Title = "Account Management", .ClassType = GetType(Scenario5)}, _
            New Scenario() With {.Title = "OAuth2 using Http Filters", .ClassType = GetType(Scenario6)} _
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

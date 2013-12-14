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

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML WebView control sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Navigate to a URL", .ClassType = GetType(Controls_WebView.Scenario1)}, _
            New Scenario() With {.Title = "Navigate to a String", .ClassType = GetType(Controls_WebView.Scenario2)}, _
            New Scenario() With {.Title = "Navigate to package and local state", .ClassType = GetType(Controls_WebView.Scenario3)}, _
            New Scenario() With {.Title = "Navigate with custom stream", .ClassType = GetType(Controls_WebView.Scenario4)}, _
            New Scenario() With {.Title = "Invoke script", .ClassType = GetType(Controls_WebView.Scenario5)}, _
            New Scenario() With {.Title = "Using ScriptNotify", .ClassType = GetType(Controls_WebView.Scenario6)}, _
            New Scenario() With {.Title = "Supporting the Share contract", .ClassType = GetType(Controls_WebView.Scenario7)}, _
            New Scenario() With {.Title = "Using CaptureBitmap", .ClassType = GetType(Controls_WebView.Scenario8)} _
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

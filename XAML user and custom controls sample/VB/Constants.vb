'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports UserAndCustomControls

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "XAML User and Custom Controls Sample (VB)"
        Public Const EXTENSION_SDK_LINK As String = "http://go.microsoft.com/fwlink/?LinkID=235409"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Using a custom UserControl", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Creating a custom control", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "DependencyProperty", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "Distributing your control", .ClassType = GetType(Scenario4)} _
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

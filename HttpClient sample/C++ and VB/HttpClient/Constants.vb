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
Imports Microsoft.Samples.Networking.HttpClientSample

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Http Client Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "GET Text With Cache Control", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "GET Stream", .ClassType = GetType(Scenario2)}, _
            New Scenario() With {.Title = "GET List", .ClassType = GetType(Scenario3)}, _
            New Scenario() With {.Title = "POST Text", .ClassType = GetType(Scenario4)}, _
            New Scenario() With {.Title = "POST Stream", .ClassType = GetType(Scenario5)}, _
            New Scenario() With {.Title = "POST Multipart", .ClassType = GetType(Scenario6)}, _
            New Scenario() With {.Title = "POST Stream With Progress", .ClassType = GetType(Scenario7)}, _
            New Scenario() With {.Title = "POST Custom Content", .ClassType = GetType(Scenario8)}, _
            New Scenario() With {.Title = "Get Cookies", .ClassType = GetType(Scenario9)}, _
            New Scenario() With {.Title = "Set Cookie", .ClassType = GetType(Scenario10)}, _
            New Scenario() With {.Title = "Delete Cookie", .ClassType = GetType(Scenario11)}, _
            New Scenario() With {.Title = "Metered Connection Filter", .ClassType = GetType(Scenario12)}, _
            New Scenario() With {.Title = "Retry Filter", .ClassType = GetType(Scenario13)} _
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

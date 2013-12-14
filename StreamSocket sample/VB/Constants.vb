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

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Stream Socket Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Start StreamSocketListener", .ClassType = GetType(StreamSocket.Scenario1)}, _
            New Scenario() With {.Title = "Connect to Listener", .ClassType = GetType(StreamSocket.Scenario2)}, _
            New Scenario() With {.Title = "Send Data", .ClassType = GetType(StreamSocket.Scenario3)}, _
            New Scenario() With {.Title = "Close Socket", .ClassType = GetType(StreamSocket.Scenario4)}, _
            New Scenario() With {.Title = "Handle Server Certificates", .ClassType = GetType(StreamSocket.Scenario5)} _
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

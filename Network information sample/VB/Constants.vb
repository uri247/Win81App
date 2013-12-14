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
        Public Const FEATURE_NAME As String = "Network Information"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Get Internet Connection Profile", .ClassType = GetType(NetworkInformationApi.InternetConnectionProfile)}, _
            New Scenario() With {.Title = "Get Data Usage over Connectivity Intervals", .ClassType = GetType(NetworkInformationApi.ProfileLocalUsageData)}, _
            New Scenario() With {.Title = "Get Connection Profile List", .ClassType = GetType(NetworkInformationApi.ConnectionProfileList)}, _
            New Scenario() With {.Title = "Get Lan Identifiers", .ClassType = GetType(NetworkInformationApi.LanId)}, _
            New Scenario() With {.Title = "Register for Network Status Change Notifications", .ClassType = GetType(NetworkInformationApi.NetworkStatusChange)}, _
            New Scenario() With {.Title = "Find Connection Profiles", .ClassType = GetType(NetworkInformationApi.FindConnectionProfiles)} _
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

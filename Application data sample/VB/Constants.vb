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
        Public Const FEATURE_NAME As String = "ApplicationData"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Files", .ClassType = GetType(ApplicationDataSample.Files)}, _
            New Scenario() With {.Title = "Settings", .ClassType = GetType(ApplicationDataSample.Settings)}, _
            New Scenario() With {.Title = "Setting Containers", .ClassType = GetType(ApplicationDataSample.SettingContainer)}, _
            New Scenario() With {.Title = "Composite Settings", .ClassType = GetType(ApplicationDataSample.CompositeSettings)}, _
            New Scenario() With {.Title = "DataChanged Event", .ClassType = GetType(ApplicationDataSample.DataChangedEvent)}, _
            New Scenario() With {.Title = "Roaming: HighPriority", .ClassType = GetType(ApplicationDataSample.HighPriority)}, _
            New Scenario() With {.Title = "ms-appdata:// Protocol", .ClassType = GetType(ApplicationDataSample.Msappdata)}, _
            New Scenario() With {.Title = "Clear", .ClassType = GetType(ApplicationDataSample.ClearScenario)}, _
            New Scenario() With {.Title = "SetVersion", .ClassType = GetType(ApplicationDataSample.SetVersion)} _
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

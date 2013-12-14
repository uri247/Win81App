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
        Public Const FEATURE_NAME As String = "Date and Time Formatting Sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Format date and time using long and short", .ClassType = GetType(DateTimeFormatting.LongAndShortFormats)}, _
            New Scenario() With {.Title = "Format using a string template", .ClassType = GetType(DateTimeFormatting.StringTemplate)}, _
            New Scenario() With {.Title = "Format using a parametrized template", .ClassType = GetType(DateTimeFormatting.ParametrizedTemplate)}, _
            New Scenario() With {.Title = "Override the current user's settings", .ClassType = GetType(DateTimeFormatting.OverrideSettings)}, _
            New Scenario() With {.Title = "Format using Unicode extensions", .ClassType = GetType(DateTimeFormatting.UsingUnicodeExtensions)}, _
            New Scenario() With {.Title = "Format using different timezones", .ClassType = GetType(DateTimeFormatting.TimeZoneSupport)} _
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

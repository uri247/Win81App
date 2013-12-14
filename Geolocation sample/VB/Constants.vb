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

        Public Const FEATURE_NAME As String = "Geolocation"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Track position", .ClassType = GetType(Microsoft.Samples.Devices.Geolocation.Scenario1)}, _
            New Scenario() With {.Title = "Get position", .ClassType = GetType(Microsoft.Samples.Devices.Geolocation.Scenario2)}, _
            New Scenario() With {.Title = "Get position in background task", .ClassType = GetType(Microsoft.Samples.Devices.Geolocation.Scenario3)}, _
            New Scenario() With {.Title = "Foreground Geofencing", .ClassType = GetType(Microsoft.Samples.Devices.Geolocation.Scenario4)}, _
            New Scenario() With {.Title = "Background Geofencing", .ClassType = GetType(Microsoft.Samples.Devices.Geolocation.Scenario5)} _
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

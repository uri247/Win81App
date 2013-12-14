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

        Public Const FEATURE_NAME As String = "Accelerometer Sensor Raw Data"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Accelerometer data events", .ClassType = GetType(Microsoft.Samples.Devices.Sensors.AccelerometerSample.Scenario1)}, _
            New Scenario() With {.Title = "Accelerometer shake events", .ClassType = GetType(Microsoft.Samples.Devices.Sensors.AccelerometerSample.Scenario2)}, _
            New Scenario() With {.Title = "Poll accelerometer readings", .ClassType = GetType(Microsoft.Samples.Devices.Sensors.AccelerometerSample.Scenario3)} _
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

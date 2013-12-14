'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' This code is licensed under the Microsoft Public License.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************


Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Windows Store app display orientation sample"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Adjust for Rotation", .ClassType = GetType(DisplayOrientation.Scenario1)}, _
            New Scenario() With {.Title = "Set a Rotation Preference", .ClassType = GetType(DisplayOrientation.Scenario2)}, _
            New Scenario() With {.Title = "Screen Orientation", .ClassType = GetType(DisplayOrientation.Scenario3)} _
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

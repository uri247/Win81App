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

        Public Const FEATURE_NAME As String = "Splash Screen Sample (VB)"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Registering for dismissal notifications", .ClassType = GetType(SplashScreen.Scenario1)}, _
            New Scenario() With {.Title = "Extending the splash screen", .ClassType = GetType(SplashScreen.Scenario2)} _
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

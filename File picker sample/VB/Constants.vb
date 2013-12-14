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

        Public Const FEATURE_NAME As String = "File Picker Sample (VB)"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Pick a single photo", .ClassType = GetType(FilePicker.Scenario1)}, _
            New Scenario() With {.Title = "Pick multiple files", .ClassType = GetType(FilePicker.Scenario2)}, _
            New Scenario() With {.Title = "Pick a folder", .ClassType = GetType(FilePicker.Scenario3)}, _
            New Scenario() With {.Title = "Save a file", .ClassType = GetType(FilePicker.Scenario4)}, _
            New Scenario() With {.Title = "Open a cached file", .ClassType = GetType(FilePicker.Scenario5)}, _
            New Scenario() With {.Title = "Update a cached file", .ClassType = GetType(FilePicker.Scenario6)} _
        }

        Friend Sub ResetScenarioOutput(ByVal output As TextBlock)
            ' clear Error/Status
            NotifyUser("", NotifyType.ErrorMessage)
            NotifyUser("", NotifyType.StatusMessage)
            ' clear scenario output
            output.Text = ""
        End Sub
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

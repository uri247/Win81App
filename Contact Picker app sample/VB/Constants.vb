'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel.Contacts.Provider

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Contact Picker Sample"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Pick a single contact", .ClassType = GetType(ContactPicker.ScenarioSingle)}, _
            New Scenario() With {.Title = "Pick multiple contacts", .ClassType = GetType(ContactPicker.ScenarioMultiple)} _
        }
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

    Partial Public Class App
        Inherits Application

        Protected Overrides Sub OnActivated(ByVal args As Windows.ApplicationModel.Activation.IActivatedEventArgs)
            If args.Kind = ActivationKind.ContactPicker Then
                Dim page = New MainPagePicker()
                page.Activate(CType(args, ContactPickerActivatedEventArgs))
            Else
                MyBase.OnActivated(args)
            End If
        End Sub
    End Class

    Partial Public Class MainPagePicker
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Contact Picker Sample"

        Private scenariosList As New List(Of Scenario)() From {New Scenario() With {.Title = "Select contact(s)", .ClassType = GetType(ContactPicker.ContactPickerPage)}}

        Friend contactPickerUI As ContactPickerUI = Nothing

        Public Sub Activate(ByVal args As ContactPickerActivatedEventArgs)
            ' cache ContactPickerUI
            contactPickerUI = args.ContactPickerUI
            Window.Current.Content = Me
            Me.OnNavigatedTo(Nothing)
            Window.Current.Activate()
        End Sub
    End Class
End Namespace

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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        If MainPage.Current.Dismissed Then
            OutputTextBlock.Text = "Received the splash screen dismissal event."
        Else
            OutputTextBlock.Text = "The application didn't receive the splash screen dismissal event."
        End If
    End Sub
End Class

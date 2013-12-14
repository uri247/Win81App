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
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        Dim rect As Rect = MainPage.Current.SplashImageRect
        XValue.Text = " " & rect.X.ToString() & ", "
        YValue.Text = " " & rect.Y.ToString() & " with "
        WidthValue.Text = " " & rect.Width.ToString() & ", "
        HeightValue.Text = " " & rect.Height.ToString() & "."
    End Sub
End Class

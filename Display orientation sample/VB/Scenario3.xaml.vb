'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Graphics.Display

Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        screenOrientation.Text = DisplayInformation.GetForCurrentView().CurrentOrientation.ToString()
        AddHandler DisplayInformation.GetForCurrentView().OrientationChanged, AddressOf OnOrientationChanged
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler DisplayInformation.GetForCurrentView().OrientationChanged, AddressOf OnOrientationChanged
    End Sub

    Private Sub OnOrientationChanged(ByVal sender As DisplayInformation, ByVal args As Object)
        screenOrientation.Text = sender.CurrentOrientation.ToString()
    End Sub
End Class

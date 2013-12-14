'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports DataBinding

Partial Public NotInheritable Class Scenario5
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Scenario5Reset(Nothing, Nothing)
    End Sub

    Private Sub Scenario5Reset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        lvDataTemplates.ItemsSource = New Teams()
    End Sub

End Class


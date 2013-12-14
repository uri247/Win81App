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

Partial Public NotInheritable Class Scenario6
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Scenario6Reset(Nothing, Nothing)
    End Sub

    Private Sub Scenario6Reset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim teams As New Teams()
        Dim result = From t In teams _
                     Group t By t.City Into g = Group _
                     Order By City _
                     Select New With {Key .Key = City, Key .Items = g}


        groupInfoCVS.Source = result
    End Sub
End Class

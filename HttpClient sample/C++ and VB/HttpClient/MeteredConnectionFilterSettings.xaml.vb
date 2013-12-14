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
Imports HttpFilters

' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Partial Public NotInheritable Class MeteredConnectionFilterSettings
    Inherits UserControl

    Private meteredConnectionFilter As HttpMeteredConnectionFilter

    Public Sub New(ByVal meteredConnectionFilter As HttpMeteredConnectionFilter)
        If meteredConnectionFilter Is Nothing Then
            Throw New ArgumentNullException("meteredConnectionFilter")
        End If

        Me.InitializeComponent()

        Me.meteredConnectionFilter = meteredConnectionFilter
        OptInSwitch.IsOn = meteredConnectionFilter.OptIn
    End Sub

    Private Sub OptInSwitch_Toggled(ByVal sender As Object, ByVal e As RoutedEventArgs)
        meteredConnectionFilter.OptIn = OptInSwitch.IsOn
    End Sub
End Class

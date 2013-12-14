'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Partial Public NotInheritable Class Scenario8
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private employees As GeneratorIncrementalLoadingClass(Of Employee)

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub Scenario8Reset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If employees IsNot Nothing Then
            RemoveHandler employees.CollectionChanged, AddressOf _employees_CollectionChanged
        End If

        employees = New GeneratorIncrementalLoadingClass(Of Employee)(1000, Function(count) New Employee() With {.Name = "Name" & count, .Organization = "Organization" & count})
        AddHandler employees.CollectionChanged, AddressOf _employees_CollectionChanged

        employeesCVS.Source = employees

        tbCollectionChangeStatus.Text = String.Empty
    End Sub

    Private Sub _employees_CollectionChanged(ByVal sender As Object, ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
        tbCollectionChangeStatus.Text = String.Format("Collection was changed. Count = {0}", employees.Count)
    End Sub
End Class

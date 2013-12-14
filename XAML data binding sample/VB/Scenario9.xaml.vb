'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Partial Public NotInheritable Class Scenario9

    Private _employee As Employee

    Public Sub New()
        Me.InitializeComponent()
        _employee = New Employee()
        AddHandler _employee.PropertyChanged, AddressOf employeeChanged
        Output.DataContext = _employee
        ScenarioReset(Nothing, Nothing)
    End Sub

    Private Sub ScenarioReset(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _employee.Name = "Jane Doe"
        _employee.Organization = "Contoso"
        BoundDataModelStatus.Text = ""
    End Sub

    Private Sub employeeChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        If e.PropertyName.Equals("Name") Then
            BoundDataModelStatus.Text = "The property '" & e.PropertyName & "' was changed." & vbLf & vbLf & "New value: " & _employee.Name
        End If
    End Sub

    Private Sub UpdateDataBtnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim expression = NameTxtBox.GetBindingExpression(TextBox.TextProperty)
        expression.UpdateSource()
    End Sub
End Class


'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Partial Public NotInheritable Class Scenario10
    Inherits Page

    Private _employee As Employee

    Public Sub New()
        Me.InitializeComponent()

        _employee = New Employee()
        Output.DataContext = _employee

        ScenarioReset(Nothing, Nothing)
        AddHandler _employee.PropertyChanged, AddressOf employeeChanged
        ShowEmployeeInfo(EmployeeDataModel)
    End Sub

    Private Sub ScenarioReset(ByVal sender As Object, ByVal e As RoutedEventArgs)

        _employee.Name = "Jane Doe"
        _employee.Organization = "Contoso"
        _employee.Age = Nothing

        'To reset Bindings with NullTargetValue and FallbackValue is necessary to reassign the Bindings
        Dim ageBindingExp As BindingExpression = AgeTextBox.GetBindingExpression(TextBox.TextProperty)
        Dim ageBinding As Binding = ageBindingExp.ParentBinding
        AgeTextBox.SetBinding(TextBox.TextProperty, ageBinding)

        Dim salaryBindingExp As BindingExpression = SalaryTextBox.GetBindingExpression(TextBox.TextProperty)
        Dim salaryBinding As Binding = salaryBindingExp.ParentBinding
        SalaryTextBox.SetBinding(TextBox.TextProperty, salaryBinding)

        tbBoundDataModelStatus.Text = ""
    End Sub

    Private Sub ShowEmployeeInfo(ByVal textBlock As TextBlock)
        textBlock.Text += vbLf & "Name: " & _employee.Name
        textBlock.Text += vbLf & "Organization: " & _employee.Organization
        If _employee.Age Is Nothing Then
            textBlock.Text += vbLf & "Age: Null"
        Else
            textBlock.Text += vbLf & "Age: " & _employee.Age
        End If
    End Sub

    Private Sub employeeChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        tbBoundDataModelStatus.Text = "The property '" & e.PropertyName & "' changed."
        tbBoundDataModelStatus.Text += vbLf & vbLf & "New values are:" & vbLf
        ShowEmployeeInfo(tbBoundDataModelStatus)
    End Sub

    Private Sub AgeTextBoxLostFocus(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If _employee.Age Is Nothing Then
            Dim age As Integer = 0
            If Integer.TryParse(AgeTextBox.Text, age) Then
                _employee.Age = age
            End If

        End If
    End Sub
End Class

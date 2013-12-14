'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' This value converter is used in Scenario 2. For more information on Value Converters, see http://go.microsoft.com/fwlink/?LinkId=254639#data_conversions
Public Class S2Formatter
    Implements IValueConverter

    'Convert the slider value into Grades
    Public Function Convert(ByVal value As Object, ByVal type As System.Type, ByVal parameter As Object, ByVal language As String) As Object Implements IValueConverter.Convert
        Dim _value As Integer
        Dim _grade As String = String.Empty
        'try parsing the value to int
        If Int32.TryParse(value.ToString(), _value) Then
            If _value < 50 Then
                _grade = "F"
            ElseIf _value < 60 Then
                _grade = "D"
            ElseIf _value < 70 Then
                _grade = "C"
            ElseIf _value < 80 Then
                _grade = "B"
            ElseIf _value < 90 Then
                _grade = "A"
            ElseIf _value < 100 Then
                _grade = "A+"
            ElseIf _value = 100 Then
                _grade = "SUPER STAR!"
            End If
        End If

        Return _grade
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal type As System.Type, ByVal parameter As Object, ByVal language As String) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException() 'doing one-way binding so this is not required.
    End Function
End Class


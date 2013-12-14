'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        rootPage.ValidateFile()
        AddHandler WriteTextButton.Click, AddressOf WriteTextButton_Click
        AddHandler ReadTextButton.Click, AddressOf ReadTextButton_Click
    End Sub

    Private Async Sub WriteTextButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                Dim userContent As String = InputTextBox.Text
                If Not String.IsNullOrEmpty(userContent) Then
                    Await FileIO.WriteTextAsync(file, userContent)
                    OutputTextBlock.Text = "The following text was written to '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & userContent
                Else
                    OutputTextBlock.Text = "The text box is empty, please write something and then click 'Write' again."
                End If
            Catch e1 As FileNotFoundException
                rootPage.NotifyUserFileNotExist()
            End Try
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub

    Private Async Sub ReadTextButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                Dim fileContent As String = Await FileIO.ReadTextAsync(file)
                OutputTextBlock.Text = "The following text was read from '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & fileContent
            Catch e1 As FileNotFoundException
                rootPage.NotifyUserFileNotExist()
            End Try
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub
End Class


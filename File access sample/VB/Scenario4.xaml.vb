'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage.Streams

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        rootPage.ValidateFile()
        AddHandler WriteBytesButton.Click, AddressOf WriteBytesButton_Click
        AddHandler ReadBytesButton.Click, AddressOf ReadBytesButton_Click
    End Sub

    Private Function GetBufferFromString(ByVal str As String) As IBuffer
        Using memoryStream As New InMemoryRandomAccessStream()
            Using dataWriter As New DataWriter(memoryStream)
                dataWriter.WriteString(str)
                Return dataWriter.DetachBuffer()
            End Using
        End Using
    End Function

    Private Async Sub WriteBytesButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                Dim userContent As String = InputTextBox.Text
                If Not String.IsNullOrEmpty(userContent) Then
                    Dim buffer As IBuffer = GetBufferFromString(userContent)
                    Await FileIO.WriteBufferAsync(file, buffer)
                    OutputTextBlock.Text = "The following " & buffer.Length & " bytes of text were written to '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & userContent
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

    Private Async Sub ReadBytesButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                Dim buffer As IBuffer = Await FileIO.ReadBufferAsync(file)
                Using dataReader As DataReader = dataReader.FromBuffer(buffer)
                    Dim fileContent As String = dataReader.ReadString(buffer.Length)
                    OutputTextBlock.Text = "The following " & buffer.Length & " bytes of text were read from '" & file.Name & "':" & Environment.NewLine & Environment.NewLine & fileContent
                End Using
            Catch e1 As FileNotFoundException
                rootPage.NotifyUserFileNotExist()
            End Try
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub
End Class


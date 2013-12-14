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
Partial Public NotInheritable Class Scenario5
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        rootPage.ValidateFile()
        AddHandler WriteToStreamButton.Click, AddressOf WriteToStreamButton_Click
        AddHandler ReadFromStreamButton.Click, AddressOf ReadFromStreamButton_Click
    End Sub

    Private Async Sub WriteToStreamButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                Dim userContent As String = InputTextBox.Text
                If Not String.IsNullOrEmpty(userContent) Then
                    Using transaction As StorageStreamTransaction = Await file.OpenTransactedWriteAsync()
                        Using dataWriter As New DataWriter(transaction.Stream)
                            dataWriter.WriteString(userContent)
                            transaction.Stream.Size = Await dataWriter.StoreAsync() ' reset stream size to override the file
                            Await transaction.CommitAsync()
                            OutputTextBlock.Text = "The following text was written to '" & file.Name & "' using a stream:" & Environment.NewLine & Environment.NewLine & userContent
                        End Using
                    End Using
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

    Private Async Sub ReadFromStreamButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                Using readStream As IRandomAccessStream = Await file.OpenAsync(FileAccessMode.Read)
                    Using dataReader As New DataReader(readStream)
                        Dim size As UInt64 = readStream.Size
                        If size <= UInt32.MaxValue Then
                            Dim numBytesLoaded As UInt32 = Await dataReader.LoadAsync(CUInt(size))
                            Dim fileContent As String = dataReader.ReadString(numBytesLoaded)
                            OutputTextBlock.Text = "The following text was read from '" & file.Name & "' using a stream:" & Environment.NewLine & Environment.NewLine & fileContent
                        Else
                            OutputTextBlock.Text = "File " & file.Name & " is too big for LoadAsync to load in a single chunk. Files larger than 4GB need to be broken into multiple chunks to be loaded by LoadAsync."
                        End If
                    End Using
                End Using
            Catch e1 As FileNotFoundException
                rootPage.NotifyUserFileNotExist()
            End Try
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub
End Class


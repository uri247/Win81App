'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage.Pickers

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario9
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        rootPage.ValidateFile()
        AddHandler CompareFilesButton.Click, AddressOf CompareFilesButton_Click
    End Sub

    ''' <summary>
    ''' Compares a picked file with sample.dat
    ''' </summary>
    Private Async Sub CompareFilesButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Dim picker As New FileOpenPicker()
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
            picker.FileTypeFilter.Add("*")
            Dim comparand As StorageFile = Await picker.PickSingleFileAsync()
            If comparand IsNot Nothing Then
                If file.IsEqual(comparand) Then
                    OutputTextBlock.Text = "Files are equal"
                Else
                    OutputTextBlock.Text = "Files are not equal"
                End If
            Else
                OutputTextBlock.Text = "Operation cancelled"
            End If
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub
End Class


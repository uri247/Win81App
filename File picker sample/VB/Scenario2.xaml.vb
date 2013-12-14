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
Imports System.Text
Imports Windows.Storage
Imports Windows.Storage.Pickers

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickFilesButton.Click, AddressOf PickFilesButton_Click
    End Sub

    Private Async Sub PickFilesButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Clear any previously returned files between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputTextBlock)

        Dim openPicker As New FileOpenPicker()
        openPicker.ViewMode = PickerViewMode.List
        openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        openPicker.FileTypeFilter.Add("*")
        Dim files As IReadOnlyList(Of StorageFile) = Await openPicker.PickMultipleFilesAsync()
        If files.Count > 0 Then
            Dim output As New StringBuilder("Picked files:" & vbLf)
            ' Application now has read/write access to the picked file(s)
            For Each file As StorageFile In files
                output.Append(file.Name & vbLf)
            Next file
            OutputTextBlock.Text = output.ToString()
        Else
            OutputTextBlock.Text = "Operation cancelled."
        End If
    End Sub
End Class

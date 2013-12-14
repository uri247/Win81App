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
Imports Windows.Storage
Imports Windows.Storage.Pickers

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickAFileButton.Click, AddressOf PickAFileButton_Click
    End Sub

    Private Async Sub PickAFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Clear previous returned file name, if it exists, between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputTextBlock)

        Dim openPicker As New FileOpenPicker()
        openPicker.ViewMode = PickerViewMode.Thumbnail
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        openPicker.FileTypeFilter.Add(".jpg")
        openPicker.FileTypeFilter.Add(".jpeg")
        openPicker.FileTypeFilter.Add(".png")
        Dim file As StorageFile = Await openPicker.PickSingleFileAsync()
        If file IsNot Nothing Then
            ' Application now has read/write access to the picked file
            OutputTextBlock.Text = "Picked photo: " & file.Name
        Else
            OutputTextBlock.Text = "Operation cancelled."
        End If
    End Sub
End Class

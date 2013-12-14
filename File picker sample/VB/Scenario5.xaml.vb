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
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current
    Private fileToken As String = String.Empty

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickFileButton.Click, AddressOf PickFileButton_Click
        AddHandler OutputFileButton.Click, AddressOf OutputFileButton_Click
    End Sub

    Private Async Sub PickFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Clear previous returned file content, if it exists, between iterations of this scenario
        rootPage.ResetScenarioOutput(OutputFileName)
        rootPage.ResetScenarioOutput(OutputFileContent)
        rootPage.NotifyUser("", NotifyType.StatusMessage)
        Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Clear()
        fileToken = String.Empty

        Dim openPicker As New FileOpenPicker()
        openPicker.FileTypeFilter.Add(".txt")
        Dim file As StorageFile = Await openPicker.PickSingleFileAsync()
        If file IsNot Nothing Then
            fileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file)
            OutputFileButton.IsEnabled = True
            OutputFileAsync(file)
        Else
            rootPage.NotifyUser("Operation cancelled.", NotifyType.StatusMessage)
        End If
    End Sub

    Private Async Sub OutputFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Not String.IsNullOrEmpty(fileToken) Then
            rootPage.NotifyUser("", NotifyType.StatusMessage)

            ' Windows will call the server app to update the local version of the file
            Try
                Dim file As StorageFile = Await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken)
                OutputFileAsync(file)
            Catch e1 As UnauthorizedAccessException
                rootPage.NotifyUser("Access is denied.", NotifyType.ErrorMessage)
            End Try
        End If
    End Sub

    Private Async Sub OutputFileAsync(ByVal file As StorageFile)
        Dim fileContent As String = Await FileIO.ReadTextAsync(file)
        OutputFileName.Text = String.Format("Received file: {0}", file.Name)
        OutputFileContent.Text = String.Format("File content:{0}{1}", System.Environment.NewLine, fileContent)
    End Sub
End Class

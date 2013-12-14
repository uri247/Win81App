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
Partial Public NotInheritable Class Scenario8
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        rootPage.ValidateFile()
        AddHandler CopyFileButton.Click, AddressOf CopyFileButton_Click
    End Sub

    Private Async Sub CopyFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                Dim fileCopy As StorageFile = Await file.CopyAsync(KnownFolders.PicturesLibrary, "sample - Copy.dat", NameCollisionOption.ReplaceExisting)
                OutputTextBlock.Text = "The file '" & file.Name & "' was copied and the new file was named '" & fileCopy.Name & "'."
            Catch e1 As FileNotFoundException
                rootPage.NotifyUserFileNotExist()
            End Try
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub
End Class


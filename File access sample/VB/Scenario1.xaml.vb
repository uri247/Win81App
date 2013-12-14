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
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler CreateFileButton.Click, AddressOf CreateFileButton_Click
    End Sub

    ''' <summary>
    ''' Creates a new file
    ''' </summary>
    Private Async Sub CreateFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim storageFolder As StorageFolder = KnownFolders.PicturesLibrary
        rootPage.sampleFile = Await storageFolder.CreateFileAsync(MainPage.filename, CreationCollisionOption.ReplaceExisting)
        OutputTextBlock.Text = "The file '" & rootPage.sampleFile.Name & "' was created."
    End Sub
End Class

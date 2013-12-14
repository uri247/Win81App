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
Partial Public NotInheritable Class Scenario11
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler GetFileButton.Click, AddressOf GetFileButton_Click
    End Sub

    ''' <summary>
    ''' Gets a file without throwing an exception
    ''' </summary>
    Private Async Sub GetFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim storageFolder As StorageFolder = KnownFolders.PicturesLibrary
        Dim file As StorageFile = TryCast(Await storageFolder.TryGetItemAsync("sample.dat"), StorageFile)
        If file IsNot Nothing Then
            OutputTextBlock.Text = "Operation result: " & file.Name
        Else
            OutputTextBlock.Text = "Operation result: null"
        End If
    End Sub
End Class


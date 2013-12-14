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
Partial Public NotInheritable Class Scenario2
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        rootPage.ValidateFile()
        AddHandler GetParentButton.Click, AddressOf GetParent_Click
    End Sub

    ''' <summary>
    ''' Gets the file's parent folder
    ''' </summary>
    Private Async Sub GetParent_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Dim parentFolder As StorageFolder = Await file.GetParentAsync()
            If parentFolder IsNot Nothing Then
                OutputTextBlock.Text &= "Item: " & file.Name & " (" & file.Path & ")" & vbLf
                OutputTextBlock.Text &= "Parent: " & parentFolder.Name & " (" & parentFolder.Path & ")" & vbLf
            End If
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub
End Class


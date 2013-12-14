'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage.FileProperties

Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Private Shared ReadOnly dateAccessedProperty As String = "System.DateAccessed"
    Private Shared ReadOnly fileOwnerProperty As String = "System.FileOwner"

    Public Sub New()
        Me.InitializeComponent()
        rootPage.ValidateFile()
        AddHandler ShowPropertiesButton.Click, AddressOf ShowPropertiesButton_Click
    End Sub

    Private Async Sub ShowPropertiesButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.ResetScenarioOutput(OutputTextBlock)
        Dim file As StorageFile = rootPage.sampleFile
        If file IsNot Nothing Then
            Try
                ' Get top level file properties
                Dim outputText As New StringBuilder()
                outputText.AppendLine("File name: " & file.Name)
                outputText.AppendLine("File type: " & file.FileType)

                ' Get basic properties
                Dim basicProperties As BasicProperties = Await file.GetBasicPropertiesAsync()
                outputText.AppendLine("File size: " & basicProperties.Size & " bytes")
                outputText.AppendLine("Date modified: " & basicProperties.DateModified.ToString())

                ' Get extra properties
                Dim propertiesName As New List(Of String)()
                propertiesName.Add(dateAccessedProperty)
                propertiesName.Add(fileOwnerProperty)
                Dim extraProperties As IDictionary(Of String, Object) = Await file.Properties.RetrievePropertiesAsync(propertiesName)
                Dim propValue = extraProperties(dateAccessedProperty)
                If propValue IsNot Nothing Then
                    outputText.AppendLine("Date accessed: " & propValue.ToString())
                End If
                propValue = extraProperties(fileOwnerProperty)
                If propValue IsNot Nothing Then
                    outputText.AppendLine("File owner: " & propValue.ToString())
                End If

                OutputTextBlock.Text = outputText.ToString()
            Catch e1 As FileNotFoundException
                rootPage.NotifyUserFileNotExist()
            End Try
        Else
            rootPage.NotifyUserFileNotExist()
        End If
    End Sub
End Class


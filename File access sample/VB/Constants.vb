'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "File access VB sample"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Creating a file", .ClassType = GetType(FileAccess.Scenario1)}, _
            New Scenario() With {.Title = "Getting a file's parent folder", .ClassType = GetType(FileAccess.Scenario2)}, _
            New Scenario() With {.Title = "Writing and reading text in a file", .ClassType = GetType(FileAccess.Scenario3)}, _
            New Scenario() With {.Title = "Writing and reading bytes in a file", .ClassType = GetType(FileAccess.Scenario4)}, _
            New Scenario() With {.Title = "Writing and reading using a stream", .ClassType = GetType(FileAccess.Scenario5)}, _
            New Scenario() With {.Title = "Displaying file properties", .ClassType = GetType(FileAccess.Scenario6)}, _
            New Scenario() With {.Title = "Persisting access to a storage item for future use", .ClassType = GetType(FileAccess.Scenario7)}, _
            New Scenario() With {.Title = "Copying a file", .ClassType = GetType(FileAccess.Scenario8)}, _
            New Scenario() With {.Title = "Comparing two files to see if they are the same file", .ClassType = GetType(FileAccess.Scenario9)}, _
            New Scenario() With {.Title = "Deleting a file", .ClassType = GetType(FileAccess.Scenario10)}, _
            New Scenario() With {.Title = "Attempting to get a file with no error on failure", .ClassType = GetType(FileAccess.Scenario11)} _
        }

        Public Const filename As String = "sample.dat"
        Public sampleFile As StorageFile = Nothing
        Public mruToken As String = Nothing
        Public falToken As String = Nothing

        ''' <summary>
        ''' Checks if sample file already exists, if it does assign it to sampleFile
        ''' </summary>
        Friend Async Sub ValidateFile()
            Try
                sampleFile = Await Windows.Storage.KnownFolders.PicturesLibrary.GetFileAsync(filename)
            Catch e1 As FileNotFoundException
                ' If file doesn't exist, indicate users to use scenario 1
                NotifyUserFileNotExist()
            End Try
        End Sub

        Friend Sub ResetScenarioOutput(ByVal output As TextBlock)
            ' clear Error/Status
            NotifyUser("", NotifyType.ErrorMessage)
            NotifyUser("", NotifyType.StatusMessage)
            ' clear scenario output
            output.Text = ""
        End Sub

        Friend Sub NotifyUserFileNotExist()
            NotifyUser(String.Format("The file '{0}' does not exist. Use scenario one to create this file.", filename), NotifyType.ErrorMessage)
        End Sub
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ReceiveFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Display the result of the file activation if we got here as a result of being activated for a file.
        If rootPage.FileEvent IsNot Nothing Then
            Dim output As String = "File activation received. The number of files received is " & rootPage.FileEvent.Files.Count & ". The received files are:" & vbLf
            For Each file As StorageFile In rootPage.FileEvent.Files
                output = output & file.Name & vbLf
            Next file

            rootPage.NotifyUser(output, NotifyType.StatusMessage)
        End If
    End Sub
End Class

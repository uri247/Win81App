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
Imports NotificationsExtensions.TileContent
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ImageProtocols
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current
    Private imageRelativePath As String = String.Empty 'used for copying an image to localstorage

    Public Sub New()
        Me.InitializeComponent()
        ProtocolList.SelectedIndex = 0
    End Sub
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Async Sub PickImage_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Await CopyImageToLocalFolderAsync()
    End Sub

    Private Async Function CopyImageToLocalFolderAsync() As Task
        Dim picker As FileOpenPicker = New Windows.Storage.Pickers.FileOpenPicker()
        picker.ViewMode = PickerViewMode.Thumbnail
        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        picker.FileTypeFilter.Add(".jpg")
        picker.FileTypeFilter.Add(".jpeg")
        picker.FileTypeFilter.Add(".png")
        picker.FileTypeFilter.Add(".gif")
        picker.CommitButtonText = "Copy"
        Dim file As StorageFile = Await picker.PickSingleFileAsync()
        If file IsNot Nothing Then
            Dim newFile As StorageFile = Await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(file.Name, Windows.Storage.CreationCollisionOption.GenerateUniqueName)
            Await file.CopyAndReplaceAsync(newFile)
            Me.imageRelativePath = newFile.Path.Substring(newFile.Path.LastIndexOf("\") + 1)
            OutputTextBlock.Text = "Image copied to application data local storage: " & newFile.Path
        Else
            OutputTextBlock.Text = "File was not copied due to error or cancelled by user."
        End If
    End Function

    Private Sub ProtocolList_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        LocalFolder.Visibility = Visibility.Collapsed
        HTTP.Visibility = Visibility.Collapsed

        If ProtocolList.SelectedItem Is appdataProtocol Then
            LocalFolder.Visibility = Visibility.Visible
        ElseIf ProtocolList.SelectedItem Is httpProtocol Then
            HTTP.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub SendTileNotification_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim wide310x150TileContent As IWide310x150TileNotificationContent = Nothing
        If ProtocolList.SelectedItem Is packageProtocol Then 'using the ms-appx:/// protocol
            Dim wide310x150ImageAndTextContent As ITileWide310x150ImageAndText01 = TileContentFactory.CreateTileWide310x150ImageAndText01()

            wide310x150ImageAndTextContent.RequireSquare150x150Content = False
            wide310x150ImageAndTextContent.TextCaptionWrap.Text = "The image is in the appx package"
            wide310x150ImageAndTextContent.Image.Src = "ms-appx:///images/redWide310x150.png"
            wide310x150ImageAndTextContent.Image.Alt = "Red image"

            wide310x150TileContent = wide310x150ImageAndTextContent
        ElseIf ProtocolList.SelectedItem Is appdataProtocol Then 'using the appdata:///local/ protocol
            Dim wide310x150ImageContent As ITileWide310x150Image = TileContentFactory.CreateTileWide310x150Image()

            wide310x150ImageContent.RequireSquare150x150Content = False
            wide310x150ImageContent.Image.Src = "ms-appdata:///local/" & Me.imageRelativePath
            wide310x150ImageContent.Image.Alt = "App data"

            wide310x150TileContent = wide310x150ImageContent
        ElseIf ProtocolList.SelectedItem Is httpProtocol Then 'using http:// protocol
            ' Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
            Dim wide310x150PeekImageCollectionContent As ITileWide310x150PeekImageCollection04 = TileContentFactory.CreateTileWide310x150PeekImageCollection04()

            wide310x150PeekImageCollectionContent.RequireSquare150x150Content = False
            Try
                wide310x150PeekImageCollectionContent.BaseUri = HTTPBaseURI.Text
            Catch exception As ArgumentException
                OutputTextBlock.Text = exception.Message
                Return
            End Try
            wide310x150PeekImageCollectionContent.TextBodyWrap.Text = "The base URI is " & HTTPBaseURI.Text
            wide310x150PeekImageCollectionContent.ImageMain.Src = HTTPImage1.Text
            wide310x150PeekImageCollectionContent.ImageSmallColumn1Row1.Src = HTTPImage2.Text
            wide310x150PeekImageCollectionContent.ImageSmallColumn1Row2.Src = HTTPImage3.Text
            wide310x150PeekImageCollectionContent.ImageSmallColumn2Row1.Src = HTTPImage4.Text
            wide310x150PeekImageCollectionContent.ImageSmallColumn2Row2.Src = HTTPImage5.Text

            wide310x150TileContent = wide310x150PeekImageCollectionContent
        End If

        wide310x150TileContent.RequireSquare150x150Content = False
        TileUpdateManager.CreateTileUpdaterForApplication().Update(wide310x150TileContent.CreateNotification())

        OutputTextBlock.Text = MainPage.PrettyPrint(wide310x150TileContent.GetContent())
    End Sub
End Class

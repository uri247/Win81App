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
Imports Windows.ApplicationModel.Resources
Imports Windows.Data.Xml.Dom
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.UI.Notifications
Imports Windows.UI.Xaml.Media.Imaging

Partial Public NotInheritable Class PreviewAllTemplates
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current
    Private previewTileImageDescriptions As ResourceLoader = ResourceLoader.GetForCurrentView()

    Public Sub New()
        Me.InitializeComponent()
        BrandingList.SelectedIndex = 0
        TemplateList.SelectedIndex = 10
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Async Sub ViewImages_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Not AvailableImages.Text.Equals(String.Empty) Then
            AvailableImages.Text = String.Empty
            ViewImages.Content = "View local images"
        Else
            Dim output As String = "ms-appx:///images/purpleSquare310x310.png " & vbLf & "ms-appx:///images/blueWide310x150.png " & vbLf & "ms-appx:///images/redWide310x150.png " & vbLf & "ms-appx:///images/graySquare150x150.png " & vbLf
            Dim files As IReadOnlyList(Of StorageFile) = Await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync()
            For Each file As StorageFile In files
                If file.FileType.Equals(".png") OrElse file.FileType.Equals(".jpg") OrElse file.FileType.Equals(".jpeg") OrElse file.FileType.Equals(".gif") Then
                    output &= "ms-appdata:///local/" & file.Name & " " & vbLf
                End If
            Next file
            ViewImages.Content = "Hide local images"
            AvailableImages.Text = output
        End If
    End Sub

    Private Async Sub CopyImages_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim picker As FileOpenPicker = New Windows.Storage.Pickers.FileOpenPicker()
        picker.ViewMode = PickerViewMode.Thumbnail
        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        picker.FileTypeFilter.Add(".jpg")
        picker.FileTypeFilter.Add(".jpeg")
        picker.FileTypeFilter.Add(".png")
        picker.FileTypeFilter.Add(".gif")
        picker.CommitButtonText = "Copy"
        Dim files As IReadOnlyList(Of StorageFile) = Await picker.PickMultipleFilesAsync()
        OutputTextBlock.Text = "Image(s) copied to application data local storage: " & vbLf
        For Each file As StorageFile In files
            Dim copyFile As StorageFile = Await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, file.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName)
            OutputTextBlock.Text += copyFile.Path & vbLf & " "
        Next file
    End Sub

    Private Sub TemplateList_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim item As ComboBoxItem = TryCast(TemplateList.SelectedItem, ComboBoxItem)
        Dim templateName As String = If(item IsNot Nothing, item.Name, "TileSquare150x150Image")
        Dim tileXml As XmlDocument = TileUpdateManager.GetTemplateContent(CType(TemplateList.SelectedIndex, TileTemplateType))

        OutputTextBlock.Text = MainPage.PrettyPrint(tileXml.GetXml().ToString())

        Dim tileTextAttributes As XmlNodeList = tileXml.GetElementsByTagName("text")
        For i As Integer = 0 To TextInputs.Children.Count - 1
            If i < tileTextAttributes.Length Then
                TextInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Visible
            Else
                TextInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed
            End If
        Next i

        Dim tileImageAttributes As XmlNodeList = tileXml.GetElementsByTagName("image")
        For i As Integer = 0 To ImageInputs.Children.Count - 1
            If i < tileImageAttributes.Length Then
                ImageInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Visible
            Else
                ImageInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed
            End If
        Next i

        Preview.Source = New BitmapImage(New Uri("ms-appx:///images/tiles/" & templateName & ".png"))

        ' Show any available description against the preview tile.
        TilePreviewDescription.Text = previewTileImageDescriptions.GetString(templateName)
        If Not String.IsNullOrEmpty(TilePreviewDescription.Text) Then
            TilePreviewDescription.Visibility = Windows.UI.Xaml.Visibility.Visible
        Else
            TilePreviewDescription.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        End If
    End Sub

    Private Sub ClearTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Tile cleared"
    End Sub

    Private Sub UpdateTileNotification_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item As ComboBoxItem = CType(BrandingList.SelectedItem, ComboBoxItem)
        Dim branding As String = If(item.Name.Equals("BrandName"), "Name", item.Name)

        UpdateTile(CType(TemplateList.SelectedIndex, TileTemplateType), branding)
    End Sub

    Private Sub UpdateTile(ByVal templateType As TileTemplateType, ByVal branding As String)
        ' This example uses the GetTemplateContent method to get the notification as xml instead of using NotificationExtensions.

        Dim tileXml As XmlDocument = TileUpdateManager.GetTemplateContent(templateType)

        Dim textElements As XmlNodeList = tileXml.GetElementsByTagName("text")
        For i As Integer = 0 To CInt(textElements.Length - 1)
            Dim tileText As String = String.Empty
            Dim panel As StackPanel = TryCast(TextInputs.Children(i), StackPanel)
            If panel IsNot Nothing Then
                Dim box As TextBox = TryCast(panel.Children(1), TextBox)
                If box IsNot Nothing Then
                    tileText = box.Text
                    If String.IsNullOrEmpty(tileText) Then
                        tileText = "Text field " & i
                    End If
                End If
            End If
            textElements.Item(CUInt(i)).AppendChild(tileXml.CreateTextNode(tileText))
        Next i

        Dim imageElements As XmlNodeList = tileXml.GetElementsByTagName("image")
        For i As Integer = 0 To CInt(imageElements.Length - 1)
            Dim imageElement As XmlElement = CType(imageElements.Item(CUInt(i)), XmlElement)
            Dim imageSource As String = String.Empty
            Dim panel As StackPanel = TryCast(ImageInputs.Children(i), StackPanel)
            If panel IsNot Nothing Then
                Dim box As TextBox = TryCast(panel.Children(1), TextBox)
                If box IsNot Nothing Then
                    imageSource = box.Text
                    If String.IsNullOrEmpty(imageSource) Then
                        imageSource = "ms-appx:///images/redWide310x150.png"
                    End If
                End If
            End If
            imageElement.SetAttribute("src", imageSource)
        Next i

        ' Set the branding on the notification as specified in the input.
        ' The logo and display name are declared in the manifest.
        ' Branding defaults to logo if omitted.
        Dim bindingElement As XmlElement = CType(tileXml.GetElementsByTagName("binding").Item(0), XmlElement)
        bindingElement.SetAttribute("branding", branding)

        ' Set the language of the notification. Though this is optional, it is recommended to specify the language.
        Dim lang As String = LangTextBox.Text ' this needs to be a BCP47 tag
        If Not String.IsNullOrEmpty(lang) Then
            ' Specify the language of the text in the notification.
            ' This ensures the correct font is used to render the text.
            Dim visualElement As XmlElement = CType(tileXml.GetElementsByTagName("visual").Item(0), XmlElement)
            visualElement.SetAttribute("lang", lang)
        End If

        Dim tile As New TileNotification(tileXml)
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tile)

        OutputTextBlock.Text = MainPage.PrettyPrint(tileXml.GetXml())
    End Sub
End Class

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
Imports Windows.ApplicationModel.Resources.Core
Imports Windows.UI.Notifications


Partial Public NotInheritable Class Globalization
    Inherits SDKTemplate.Common.LayoutAwarePage

#Region "TemplateCode"
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub
#End Region ' TemplateCode

    Private Sub ViewCurrentResources_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim defaultContextForCurrentView As ResourceContext = ResourceContext.GetForCurrentView()

        Dim asls As String = Nothing
        defaultContextForCurrentView.QualifierValues.TryGetValue("Language", asls)

        Dim scale As String = Nothing
        defaultContextForCurrentView.QualifierValues.TryGetValue("Scale", scale)

        Dim contrast As String = Nothing
        defaultContextForCurrentView.QualifierValues.TryGetValue("Contrast", contrast)

        OutputTextBlock.Text = "Your system is currently set to the following values: Application Language: " & asls & ", Scale: " & scale & ", Contrast: " & contrast & ". If using web images and addImageQuery, the following query string would be appened to the URL: ?ms-lang=" & asls & "&ms-scale=" & scale & "&ms-contrast=" & contrast
    End Sub

    Private Sub SendTileNotificationWithQueryStrings_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim square310x310TileContent As ITileSquare310x310Image = TileContentFactory.CreateTileSquare310x310Image()
        square310x310TileContent.Image.Src = ImageUrl.Text
        square310x310TileContent.Image.Alt = "Web image"

        ' enable AddImageQuery on the notification
        square310x310TileContent.AddImageQuery = True

        Dim wide310x150TileContent As ITileWide310x150ImageAndText01 = TileContentFactory.CreateTileWide310x150ImageAndText01()
        wide310x150TileContent.TextCaptionWrap.Text = "This tile notification uses query strings for the image src."
        wide310x150TileContent.Image.Src = ImageUrl.Text
        wide310x150TileContent.Image.Alt = "Web image"

        Dim square150x150TileContent As ITileSquare150x150Image = TileContentFactory.CreateTileSquare150x150Image()
        square150x150TileContent.Image.Src = ImageUrl.Text
        square150x150TileContent.Image.Alt = "Web image"

        wide310x150TileContent.Square150x150Content = square150x150TileContent
        square310x310TileContent.Wide310x150Content = wide310x150TileContent

        TileUpdateManager.CreateTileUpdaterForApplication().Update(square310x310TileContent.CreateNotification())

        OutputTextBlock.Text = MainPage.PrettyPrint(square310x310TileContent.GetContent())
    End Sub

    Private Sub SendTileNotificationScaledImage_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim scale As String = Nothing
        ResourceContext.GetForCurrentView().QualifierValues.TryGetValue("Scale", scale)

        Dim square310x310TileContent As ITileSquare310x310Image = TileContentFactory.CreateTileSquare310x310Image()
        square310x310TileContent.Image.Src = "ms-appx:///images/purpleSquare310x310.png"
        square310x310TileContent.Image.Alt = "Purple square"

        Dim wide310x150TileContent As ITileWide310x150SmallImageAndText03 = TileContentFactory.CreateTileWide310x150SmallImageAndText03()
        wide310x150TileContent.TextBodyWrap.Text = "blueWide310x150.png in the xml is actually blueWide310x150.scale-" & scale & ".png"
        wide310x150TileContent.Image.Src = "ms-appx:///images/blueWide310x150.png"
        wide310x150TileContent.Image.Alt = "Blue wide"

        Dim square150x150TileContent As ITileSquare150x150Image = TileContentFactory.CreateTileSquare150x150Image()
        square150x150TileContent.Image.Src = "ms-appx:///images/graySquare150x150.png"
        square150x150TileContent.Image.Alt = "Gray square"

        wide310x150TileContent.Square150x150Content = square150x150TileContent
        square310x310TileContent.Wide310x150Content = wide310x150TileContent

        TileUpdateManager.CreateTileUpdaterForApplication().Update(square310x310TileContent.CreateNotification())

        OutputTextBlock.Text = MainPage.PrettyPrint(square310x310TileContent.GetContent())
    End Sub

    Private Sub SendTextResourceTileNotification_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim square310x310TileContent As ITileSquare310x310Text09 = TileContentFactory.CreateTileSquare310x310Text09()
        ' Check out /en-US/resources.resw to understand where this string will come from.
        square310x310TileContent.TextHeadingWrap.Text = "ms-resource:greeting"

        Dim wide310x150TileContent As ITileWide310x150Text03 = TileContentFactory.CreateTileWide310x150Text03()
        wide310x150TileContent.TextHeadingWrap.Text = "ms-resource:greeting"

        Dim square150x150TileContent As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        square150x150TileContent.TextBodyWrap.Text = "ms-resource:greeting"

        wide310x150TileContent.Square150x150Content = square150x150TileContent
        square310x310TileContent.Wide310x150Content = wide310x150TileContent

        TileUpdateManager.CreateTileUpdaterForApplication().Update(square310x310TileContent.CreateNotification())

        OutputTextBlock.Text = MainPage.PrettyPrint(square310x310TileContent.GetContent())
    End Sub
End Class

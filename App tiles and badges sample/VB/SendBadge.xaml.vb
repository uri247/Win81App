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
Imports NotificationsExtensions.BadgeContent
Imports NotificationsExtensions.TileContent
Imports Windows.UI.Notifications

Partial Public NotInheritable Class SendBadge
    Inherits SDKTemplate.Common.LayoutAwarePage

#Region "TemplateCode"
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        NumberOrGlyph.SelectedIndex = 0
        GlyphList.SelectedIndex = 0
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub
#End Region ' TemplateCode

    Private Sub NumberOrGlyph_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If NumberOrGlyph.SelectedIndex = 0 Then
            NumberPanel.Visibility = Visibility.Visible
            GlyphPanel.Visibility = Visibility.Collapsed
        Else
            NumberPanel.Visibility = Visibility.Collapsed
            GlyphPanel.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub UpdateBadge_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim useStrings As Boolean = False
        If sender Is UpdateBadgeWithStringManipulation Then
            useStrings = True
        End If

        If NumberOrGlyph.SelectedIndex = 0 Then
            Dim number As Integer
            If Int32.TryParse(NumberInput.Text, number) Then
                If useStrings Then
                    UpdateBadgeWithNumberWithStringManipulation(number)
                Else
                    UpdateBadgeWithNumber(number)
                End If
            Else
                OutputTextBlock.Text = "Please enter a valid number!"
            End If
        Else
            If useStrings Then
                UpdateBadgeWithGlyphWithStringManipulation()
            Else
                UpdateBadgeWithGlyph(GlyphList.SelectedIndex)
            End If
        End If
    End Sub

    Private Sub ClearBadge_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Badge cleared"
    End Sub

    Private Sub UpdateBadgeWithNumber(ByVal number As Integer)
        ' Note: This sample contains an additional project, NotificationsExtensions.
        ' NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
        ' of the notification directly. See the additional function UpdateBadgeWithNumberWithStringManipulation to see how to do it
        ' by modifying strings directly.

        Dim badgeContent As New BadgeNumericNotificationContent(CUInt(number))

        ' Send the notification to the application’s tile.
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification())

        OutputTextBlock.Text = badgeContent.GetContent()
    End Sub

    Private Sub UpdateBadgeWithGlyph(ByVal index As Integer)
        ' Note: This sample contains an additional project, NotificationsExtensions.
        ' NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
        ' of the notification directly. See the additional function UpdateBadgeWithGlyphWithStringManipulation to see how to do it
        ' by modifying strings directly.

        ' Note: usually this would be created with new BadgeGlyphNotificationContent(GlyphValue.Alert) or any of the values of GlyphValue.
        Dim badgeContent As New BadgeGlyphNotificationContent(CType(index, GlyphValue))

        ' Send the notification to the application’s tile.
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification())

        OutputTextBlock.Text = badgeContent.GetContent()
    End Sub

    Private Sub UpdateBadgeWithNumberWithStringManipulation(ByVal number As Integer)
        ' Create a string with the badge template xml.
        Dim badgeXmlString As String = "<badge value='" & number & "'/>"
        Dim badgeDOM As New Windows.Data.Xml.Dom.XmlDocument()
        Try
            ' Create a DOM.
            badgeDOM.LoadXml(badgeXmlString)

            ' Load the xml string into the DOM, catching any invalid xml characters.
            Dim badge As New BadgeNotification(badgeDOM)

            ' Create a badge notification and send it to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge)

            OutputTextBlock.Text = badgeDOM.GetXml()
        Catch e1 As Exception
            OutputTextBlock.Text = "Error loading the xml, check for invalid characters in the input"
        End Try
    End Sub

    Private Sub UpdateBadgeWithGlyphWithStringManipulation()
        ' Create a string with the badge template xml.
        Dim badgeXmlString As String = "<badge value='" & CType(GlyphList.SelectedItem, ComboBoxItem).Content.ToString() & "'/>"
        Dim badgeDOM As New Windows.Data.Xml.Dom.XmlDocument()
        Try
            ' Create a DOM.
            badgeDOM.LoadXml(badgeXmlString)

            ' Load the xml string into the DOM, catching any invalid xml characters.
            Dim badge As New BadgeNotification(badgeDOM)

            ' Create a badge notification and send it to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge)

            OutputTextBlock.Text = badgeDOM.GetXml()
        Catch e1 As Exception
            OutputTextBlock.Text = "Error loading the xml, check for invalid characters in the input"
        End Try
    End Sub
End Class

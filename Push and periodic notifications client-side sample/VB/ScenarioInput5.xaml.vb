' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Networking.PushNotifications
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ScenarioInput5
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub StartBadgePolling_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim polledUrl As String = BadgePollingURL.Text

        ' The default string for this text box is "http://".
        ' Make sure the user has entered some data.
        If polledUrl <> "http://" Then
            Dim recurrence As PeriodicUpdateRecurrence = CType(PeriodicRecurrence.SelectedIndex, PeriodicUpdateRecurrence)

            ' You can also specify a time you would like to start polling. Secondary tiles can also receive
            ' polled updates using BadgeUpdateManager.createBadgeUpdaterForSecondaryTile(tileId).
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().StartPeriodicUpdate(New Uri(polledUrl), recurrence)

            rootPage.NotifyUser("Started polling " & polledUrl & ". Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Specify a URL that returns badge XML to begin badge polling.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Sub StopBadgePolling_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().StopPeriodicUpdate()
        rootPage.NotifyUser("Stopped polling.", NotifyType.StatusMessage)
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(ByVal sender As Object, ByVal e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = CType(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;
    End Sub

#End Region

End Class


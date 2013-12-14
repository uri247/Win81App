' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Networking.PushNotifications
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ScenarioInput4
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub StartTilePolling_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim urisToPoll As New List(Of Uri)(5)
        For Each input As TextBox In New TextBox() {PollURL1, PollURL2, PollURL3, PollURL4, PollURL5}
            Dim polledUrl As String = input.Text

            ' The default string for this text box is "http://".
            ' Make sure the user has entered some data.
            If polledUrl <> "http://" AndAlso polledUrl <> "" Then
                urisToPoll.Add(New Uri(polledUrl))
            End If
        Next input

        Dim recurrence As PeriodicUpdateRecurrence = CType(PeriodicRecurrence.SelectedIndex, PeriodicUpdateRecurrence)

        If urisToPoll.Count = 1 Then
            TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(urisToPoll(0), recurrence)
            rootPage.NotifyUser("Started polling " & urisToPoll(0).AbsolutePath & ". Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage)
        ElseIf urisToPoll.Count > 1 Then
            TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdateBatch(urisToPoll, recurrence)
            rootPage.NotifyUser("Started polling the specified URLs. Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Specify a URL that returns tile XML to begin tile polling.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Sub StopTilePolling_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().StopPeriodicUpdate()
        rootPage.NotifyUser("Stopped polling.", NotifyType.StatusMessage)
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' IMPORTANT NOTE: call this only if you plan on polling several different URLs, and only
        ' once after the user installs the app or creates a secondary tile
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(True)

        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#Region "Template-Related Code - Do not remove"
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


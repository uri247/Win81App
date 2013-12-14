' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Networking.PushNotifications

Partial Public NotInheritable Class ScenarioInput3
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub AddCallback_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim currentChannel As PushNotificationChannel = rootPage.Channel
        If currentChannel IsNot Nothing Then
            AddHandler currentChannel.PushNotificationReceived, AddressOf OnPushNotificationReceived
            rootPage.NotifyUser("Callback added.", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Channel not open. Open the channel in scenario 1.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Sub RemoveCallback_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim currentChannel As PushNotificationChannel = rootPage.Channel
        If currentChannel IsNot Nothing Then
            RemoveHandler currentChannel.PushNotificationReceived, AddressOf OnPushNotificationReceived
            rootPage.NotifyUser("Callback removed.", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Channel not open. Open the channel in scenario 1.", NotifyType.StatusMessage)
        End If
    End Sub

    Private Sub OnPushNotificationReceived(ByVal sender As PushNotificationChannel, ByVal e As PushNotificationReceivedEventArgs)
        Dim typeString As String = String.Empty
        Dim notificationContent As String = String.Empty
        Select Case e.NotificationType
            Case PushNotificationType.Badge
                typeString = "Badge"
                notificationContent = e.BadgeNotification.Content.GetXml()
            Case PushNotificationType.Tile
                notificationContent = e.TileNotification.Content.GetXml()
                typeString = "Tile"
            Case PushNotificationType.Toast
                notificationContent = e.ToastNotification.Content.GetXml()
                typeString = "Toast"
            Case PushNotificationType.Raw
                notificationContent = e.RawNotification.Content
                typeString = "Raw"
        End Select

        ' Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts:
        ' if your application is already on the screen, there's no need to display a toast from push notifications.
        e.Cancel = True

        Dim text As String = "Received a " & typeString & " notification, containing: " & notificationContent
        Dim ignored = Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser(text, NotifyType.StatusMessage))
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dim currentChannel As PushNotificationChannel = rootPage.Channel

        If currentChannel IsNot Nothing Then
            RemoveHandler currentChannel.PushNotificationReceived, AddressOf OnPushNotificationReceived
        End If

        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub


#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(ByVal sender As Object, ByVal e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame
        Dim outputFrame As Page = CType(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;

    End Sub
#End Region
End Class


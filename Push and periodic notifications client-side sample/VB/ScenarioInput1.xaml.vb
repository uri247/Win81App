' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports PushNotificationsHelper
Imports Windows.Networking.PushNotifications

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    ' The Notifier object allows us to use the same code in the maintenance task and this foreground application
    Private Async Sub OpenChannel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim channelAndWebResponse As ChannelAndWebResponse = Await rootPage.Notifier.OpenChannelAndUploadAsync(ServerText.Text)
        rootPage.NotifyUser("Channel uploaded! Response:" & channelAndWebResponse.WebResponse, NotifyType.StatusMessage)
        rootPage.Channel = channelAndWebResponse.Channel
    End Sub

    Private Sub CloseChannel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim currentChannel As PushNotificationChannel = rootPage.Channel
        If currentChannel IsNot Nothing Then
            ' Closing the channel prevents all future cloud notifications from 
            ' being delivered to the application or application related UI
            currentChannel.Close()
            rootPage.Channel = Nothing

            rootPage.NotifyUser("Channel closed", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Channel not open", NotifyType.ErrorMessage)
        End If
    End Sub


    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded

        If rootPage.Notifier Is Nothing Then
            rootPage.Notifier = New Notifier()
        End If
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


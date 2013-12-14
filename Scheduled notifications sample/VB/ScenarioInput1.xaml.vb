' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports NotificationsExtensions.TileContent
Imports NotificationsExtensions.ToastContent
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler ScheduleButton.Click, AddressOf ScheduleButton_Click
        AddHandler ScheduleButtonString.Click, AddressOf ScheduleButton_Click
    End Sub

    Private Sub ScheduleButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim useStrings As Boolean = False
        If sender Is ScheduleButtonString Then
            useStrings = True
        End If

        Try
            Dim dueTimeInSeconds As Int16 = Int16.Parse(FutureTimeBox.Text)
            If dueTimeInSeconds <= 0 Then
                Throw New ArgumentException()
            End If

            Dim updateString As String = StringBox.Text
            Dim dueTime As Date = Date.Now.AddSeconds(dueTimeInSeconds)

            Dim rand As New Random()
            Dim idNumber As Integer = rand.Next(0, 10000000)

            If ToastRadio.IsChecked IsNot Nothing AndAlso CBool(ToastRadio.IsChecked) Then
                If useStrings Then
                    ScheduleToastWithStringManipulation(updateString, dueTime, idNumber)
                Else
                    ScheduleToast(updateString, dueTime, idNumber)
                End If
            Else
                If useStrings Then
                    ScheduleTileWithStringManipulation(updateString, dueTime, idNumber)
                Else
                    ScheduleTile(updateString, dueTime, idNumber)
                End If
            End If
        Catch e1 As Exception
            rootPage.NotifyUser("You must input a valid time in seconds.", NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub ScheduleToast(ByVal updateString As String, ByVal dueTime As Date, ByVal idNumber As Integer)
        ' Scheduled toasts use the same toast templates as all other kinds of toasts.
        Dim toastContent As IToastText02 = ToastContentFactory.CreateToastText02()
        toastContent.TextHeading.Text = updateString
        toastContent.TextBodyWrap.Text = "Received: " & dueTime.ToLocalTime()

        Dim toast As ScheduledToastNotification
        If RepeatBox.IsChecked IsNot Nothing AndAlso CBool(RepeatBox.IsChecked) Then
            toast = New ScheduledToastNotification(toastContent.GetXml(), dueTime, TimeSpan.FromSeconds(60), 5)

            ' You can specify an ID so that you can manage toasts later.
            ' Make sure the ID is 15 characters or less.
            toast.Id = "Repeat" & idNumber
        Else
            toast = New ScheduledToastNotification(toastContent.GetXml(), dueTime)
            toast.Id = "Toast" & idNumber
        End If

        ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast)
        rootPage.NotifyUser("Scheduled a toast with ID: " & toast.Id, NotifyType.StatusMessage)
    End Sub

    Private Sub ScheduleTile(ByVal updateString As String, ByVal dueTime As Date, ByVal idNumber As Integer)
        ' Set up the wide tile text
        Dim tileContent As ITileWide310x150Text09 = TileContentFactory.CreateTileWide310x150Text09()
        tileContent.TextHeading.Text = updateString
        tileContent.TextBodyWrap.Text = "Received: " & dueTime.ToLocalTime()

        ' Set up square tile text
        Dim squareContent As ITileSquare150x150Text04 = TileContentFactory.CreateTileSquare150x150Text04()
        squareContent.TextBodyWrap.Text = updateString

        tileContent.Square150x150Content = squareContent

        ' Create the notification object
        Dim futureTile As New ScheduledTileNotification(tileContent.GetXml(), dueTime)
        futureTile.Id = "Tile" & idNumber

        ' Add to schedule
        ' You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
        ' See "Tiles" sample for more details
        TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(futureTile)
        rootPage.NotifyUser("Scheduled a tile with ID: " & futureTile.Id, NotifyType.StatusMessage)
    End Sub


    Private Sub ScheduleToastWithStringManipulation(ByVal updateString As String, ByVal dueTime As Date, ByVal idNumber As Integer)
        ' Scheduled toasts use the same toast templates as all other kinds of toasts.
        Dim toastXmlString As String = "<toast>" & "<visual version='2'>" & "<binding template='ToastText02'>" & "<text id='2'>" & updateString & "</text>" & "<text id='1'>" & "Received: " & dueTime.ToLocalTime() & "</text>" & "</binding>" & "</visual>" & "</toast>"

        Dim toastDOM As New Windows.Data.Xml.Dom.XmlDocument()
        Try
            toastDOM.LoadXml(toastXmlString)

            Dim toast As ScheduledToastNotification
            If RepeatBox.IsChecked IsNot Nothing AndAlso CBool(RepeatBox.IsChecked) Then
                toast = New ScheduledToastNotification(toastDOM, dueTime, TimeSpan.FromSeconds(60), 5)

                ' You can specify an ID so that you can manage toasts later.
                ' Make sure the ID is 15 characters or less.
                toast.Id = "Repeat" & idNumber
            Else
                toast = New ScheduledToastNotification(toastDOM, dueTime)
                toast.Id = "Toast" & idNumber
            End If

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast)
            rootPage.NotifyUser("Scheduled a toast with ID: " & toast.Id, NotifyType.StatusMessage)
        Catch e1 As Exception
            rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub ScheduleTileWithStringManipulation(ByVal updateString As String, ByVal dueTime As Date, ByVal idNumber As Integer)
        Dim tileXmlString As String = "<tile>" & "<visual version='2'>" & "<binding template='TileWide310x150Text09' fallback='TileWideText09'>" & "<text id='1'>" & updateString & "</text>" & "<text id='2'>" & "Received: " & dueTime.ToLocalTime() & "</text>" & "</binding>" & "<binding template='TileSquare150x150Text04' fallback='TileSquareText04'>" & "<text id='1'>" & updateString & "</text>" & "</binding>" & "</visual>" & "</tile>"

        Dim tileDOM As New Windows.Data.Xml.Dom.XmlDocument()
        Try
            tileDOM.LoadXml(tileXmlString)

            ' Create the notification object
            Dim futureTile As New ScheduledTileNotification(tileDOM, dueTime)
            futureTile.Id = "Tile" & idNumber

            ' Add to schedule
            ' You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
            ' See "Tiles" sample for more details
            TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(futureTile)
            rootPage.NotifyUser("Scheduled a tile with ID: " & futureTile.Id, NotifyType.StatusMessage)
        Catch e1 As Exception
            rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage)
        End Try
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region
End Class

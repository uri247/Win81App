'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports System.Globalization
Imports Windows.Data.Xml.Dom
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.UI.Notifications
Imports Windows.Web

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
Partial Public NotInheritable Class S3_Notifications
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private notificationsGroup As BackgroundTransferGroup

    Private Shared Shadows ReadOnly baseUri As New Uri("http://localhost/BackgroundTransferSample/notifications.aspx")

    Private Shared runId As Integer = 0

    Private Enum ScenarioType
        Toast
        Tile
    End Enum

    Public Sub New()
        ' Use a unique group name so that no other component in the app uses the same group. The recommended way
        ' is to generate a GUID and use it as group name as shown below.
        notificationsGroup = BackgroundTransferGroup.CreateGroup("{296628BF-5AE6-48CE-AA36-86A85A726B6A}")

        ' When creating a group, we can optionally define the transfer behavior of transfers associated with the
        ' group. A "parallel" transfer behavior allows multiple transfers in the same group to run concurrently
        ' (default). A "serialized" transfer behavior allows at most one default priority transfer at a time for
        ' the group.
        notificationsGroup.TransferBehavior = BackgroundTransferBehavior.Parallel

        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Async Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' An application must enumerate downloads when it gets started to prevent stale downloads/uploads.
        ' Typically this can be done in the App class by overriding OnLaunched() and checking for
        ' "args.Kind == ActivationKind.Launch" to detect an actual app launch.
        ' We do it here in the sample to keep the sample code consolidated.
        ' Note that for this specific scenario we're not interested in downloads from previous instances:
        ' We'll just enumerate downloads from previous instances and cancel them immediately.
        Await CancelActiveDownloadsAsync()
    End Sub

    Private Async Function CancelActiveDownloadsAsync() As Task
        Dim downloads As IReadOnlyList(Of DownloadOperation) = Nothing
        Try
            ' Note that we only enumerate transfers that belong to the transfer group used by this sample
            ' scenario. We'll not enumerate transfers started by other sample scenarios in this app.
            downloads = Await BackgroundDownloader.GetCurrentDownloadsForTransferGroupAsync(notificationsGroup)
        Catch ex As Exception
            If Not IsExceptionHandled("Discovery error", ex) Then
                Throw
            End If
            Return
        End Try

        ' If previous instances of this scenario started transfers that haven't completed yet, cancel them now
        ' so that we can start this scenario cleanly.
        If downloads.Count > 0 Then
            Dim canceledToken As New CancellationTokenSource()
            canceledToken.Cancel()

            Dim tasks(downloads.Count - 1) As Task
            For i As Integer = 0 To downloads.Count - 1
                tasks(i) = downloads(i).AttachAsync().AsTask(canceledToken.Token)
            Next i

            Try
                Await Task.WhenAll(tasks)
            Catch e1 As TaskCanceledException
            End Try

            Log(String.Format(CultureInfo.CurrentCulture, "Canceled {0} downloads from previous instances of this scenario.", downloads.Count))
        End If

        ' After cleaning up downloads from previous scenarios, enable buttons to allow the user to run the sample.
        ToastNotificationButton.IsEnabled = True
        TileNotificationButton.IsEnabled = True
    End Function

    Private Async Sub ToastNotificationButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create a downloader and associate all its downloads with the transfer group used for this scenario.
        Dim downloader As New BackgroundDownloader()
        downloader.TransferGroup = notificationsGroup

        ' Create a ToastNotification that should be shown when all transfers succeed.
        Dim successToastXml As XmlDocument = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01)
        successToastXml.GetElementsByTagName("text").Item(0).InnerText = "All three downloads completed successfully."
        Dim successToast As New ToastNotification(successToastXml)
        downloader.SuccessToastNotification = successToast

        ' Create a ToastNotification that should be shown if at least one transfer fails.
        Dim failureToastXml As XmlDocument = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01)
        failureToastXml.GetElementsByTagName("text").Item(0).InnerText = "At least one download completed with failure."
        Dim failureToast As New ToastNotification(failureToastXml)
        downloader.FailureToastNotification = failureToast

        ' Now create and start downloads for the configured BackgroundDownloader object.
        Await RunDownloadsAsync(downloader, ScenarioType.Toast)
    End Sub

    Private Async Sub TileNotificationButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create a downloader and associate all its downloads with the transfer group used for this scenario.
        Dim downloader As New BackgroundDownloader()
        downloader.TransferGroup = notificationsGroup

        ' Create a TileNotification that should be shown when all transfers succeed.
        Dim successTileXml As XmlDocument = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03)
        Dim successTextNodes As XmlNodeList = successTileXml.GetElementsByTagName("text")
        successTextNodes.Item(0).InnerText = "All three"
        successTextNodes.Item(1).InnerText = "downloads"
        successTextNodes.Item(2).InnerText = "completed"
        successTextNodes.Item(3).InnerText = "successfully."
        Dim successTile As New TileNotification(successTileXml)
        successTile.ExpirationTime = Date.Now.AddMinutes(10)
        downloader.SuccessTileNotification = successTile

        ' Create a TileNotification that should be shown if at least one transfer fails.
        Dim failureTileXml As XmlDocument = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03)
        Dim failureTextNodes As XmlNodeList = failureTileXml.GetElementsByTagName("text")
        failureTextNodes.Item(0).InnerText = "At least"
        failureTextNodes.Item(1).InnerText = "one download"
        failureTextNodes.Item(2).InnerText = "completed"
        failureTextNodes.Item(3).InnerText = "with failure."
        Dim failureTile As New TileNotification(failureTileXml)
        failureTile.ExpirationTime = Date.Now.AddMinutes(10)
        downloader.FailureTileNotification = failureTile

        ' Now create and start downloads for the configured BackgroundDownloader object.
        Await RunDownloadsAsync(downloader, ScenarioType.Tile)
    End Sub

    Private Async Function RunDownloadsAsync(ByVal downloader As BackgroundDownloader, ByVal type As ScenarioType) As Task
        ' Use a unique ID for every button click, to help the user associate downloads of the same run
        ' in the logs.
        runId += 1

        Dim downloads(2) As DownloadOperation

        Try
            ' First we create three download operations: Note that we don't start downloads immediately. It is
            ' important to first create all operations that should participate in the toast/tile update. Once all
            ' operations have been created, we can start them.
            ' If we start a download and create a second one afterwards, there is a race where the first download
            ' may complete before we were able to create the second one. This would result in the toast/tile being
            ' shown before we even create the second download.
            downloads(0) = Await CreateDownload(downloader, 1, String.Format(CultureInfo.InvariantCulture, "{0}.{1}.FastDownload.txt", type, runId))
            downloads(1) = Await CreateDownload(downloader, 5, String.Format(CultureInfo.InvariantCulture, "{0}.{1}.MediumDownload.txt", type, runId))
            downloads(2) = Await CreateDownload(downloader, 10, String.Format(CultureInfo.InvariantCulture, "{0}.{1}.SlowDownload.txt", type, runId))
        Catch e1 As FileNotFoundException
            ' We were unable to create the destination file.
            Return
        End Try

        ' Once all downloads participating in the toast/tile update have been created, start them.
        Dim downloadTasks(downloads.Length - 1) As Task
        For i As Integer = 0 To downloads.Length - 1
            downloadTasks(i) = DownloadAsync(downloads(i))
        Next i

        Await Task.WhenAll(downloadTasks)
    End Function

    Private Async Function CreateDownload(ByVal downloader As BackgroundDownloader, ByVal delaySeconds As Integer, ByVal fileName As String) As Task(Of DownloadOperation)
        Dim source As New Uri(baseUri, String.Format(CultureInfo.InvariantCulture, "?delay={0}", delaySeconds))

        Dim destinationFile As StorageFile
        Try
            destinationFile = Await KnownFolders.PicturesLibrary.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName)
        Catch ex As FileNotFoundException
            rootPage.NotifyUser("Error while creating file: " & ex.Message, NotifyType.ErrorMessage)
            Throw
        End Try

        Return downloader.CreateDownload(source, destinationFile)
    End Function

    Private Async Function DownloadAsync(ByVal download As DownloadOperation) As Task
        Log(String.Format(CultureInfo.CurrentCulture, "Downloading {0}", download.ResultFile.Name))

        Try
            Await download.StartAsync()

            LogStatus(String.Format(CultureInfo.CurrentCulture, "Downloading {0} completed.", download.ResultFile.Name), NotifyType.StatusMessage)
        Catch e1 As TaskCanceledException
        Catch ex As Exception
            If Not IsExceptionHandled("Execution error", ex, download) Then
                Throw
            End If
        End Try
    End Function

    Private Function IsExceptionHandled(ByVal title As String, ByVal ex As Exception, Optional ByVal download As DownloadOperation = Nothing) As Boolean
        Dim errorStatus As WebErrorStatus = BackgroundTransferError.GetStatus(ex.HResult)
        If errorStatus = WebErrorStatus.Unknown Then
            Return False
        End If

        If download Is Nothing Then
            LogStatus(String.Format(CultureInfo.CurrentCulture, "Error: {0}: {1}", title, errorStatus), NotifyType.ErrorMessage)
        Else
            LogStatus(String.Format(CultureInfo.CurrentCulture, "Error: {0} - {1}: {2}", download.ResultFile.Name, title, errorStatus), NotifyType.ErrorMessage)
        End If

        Return True
    End Function

    Private Sub Log(ByVal message As String)
        outputField.Text += message & vbCrLf
    End Sub

    Private Sub LogStatus(ByVal message As String, ByVal type As NotifyType)
        rootPage.NotifyUser(message, type)
        Log(message)
    End Sub
End Class

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports System.Globalization
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.Web

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class S1_Download
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private activeDownloads As List(Of DownloadOperation)
    Private cts As CancellationTokenSource

    Public Sub New()
        cts = New CancellationTokenSource()

        Me.InitializeComponent()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If cts IsNot Nothing Then
            cts.Dispose()
            cts = Nothing
        End If

        GC.SuppressFinalize(Me)
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
        Await DiscoverActiveDownloadsAsync()
    End Sub


    ' Enumerate the downloads that were going on in the background while the app was closed.
    Private Async Function DiscoverActiveDownloadsAsync() As Task
        activeDownloads = New List(Of DownloadOperation)()

        Dim downloads As IReadOnlyList(Of DownloadOperation) = Nothing
        Try
            downloads = Await BackgroundDownloader.GetCurrentDownloadsAsync()
        Catch ex As Exception
            If Not IsExceptionHandled("Discovery error", ex) Then
                Throw
            End If
            Return
        End Try

        Log("Loading background downloads: " & downloads.Count)

        If downloads.Count > 0 Then
            Dim tasks As New List(Of Task)()
            For Each download As DownloadOperation In downloads
                Log(String.Format(CultureInfo.CurrentCulture, "Discovered background download: {0}, Status: {1}", download.Guid, download.Progress.Status))

                ' Attach progress and completion handlers.
                tasks.Add(HandleDownloadAsync(download, False))
            Next download

            ' Don't await HandleDownloadAsync() in the foreach loop since we would attach to the second
            ' download only when the first one completed; attach to the third download when the second one
            ' completes etc. We want to attach to all downloads immediately.
            ' If there are actions that need to be taken once downloads complete, await tasks here, outside
            ' the loop.
            Await Task.WhenAll(tasks)
        End If
    End Function

    Private Async Sub StartDownload(ByVal priority As BackgroundTransferPriority, ByVal requestUnconstrainedDownload As Boolean)
        ' By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
        ' box validating the URI is required since it was received from an untrusted source (user input).
        ' The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
        ' Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
        ' the "Home or Work Networking" capability.
        Dim source As Uri = Nothing
        If Not Uri.TryCreate(serverAddressField.Text.Trim(), UriKind.Absolute, source) Then
            rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage)
            Return
        End If

        Dim destination As String = fileNameField.Text.Trim()

        If String.IsNullOrWhiteSpace(destination) Then
            rootPage.NotifyUser("A local file name is required.", NotifyType.ErrorMessage)
            Return
        End If

        Dim destinationFile As StorageFile
        Try
            destinationFile = Await KnownFolders.PicturesLibrary.CreateFileAsync(destination, CreationCollisionOption.GenerateUniqueName)
        Catch ex As FileNotFoundException
            rootPage.NotifyUser("Error while creating file: " & ex.Message, NotifyType.ErrorMessage)
            Return
        End Try

        Dim downloader As New BackgroundDownloader()
        Dim download As DownloadOperation = downloader.CreateDownload(source, destinationFile)

        Log(String.Format(CultureInfo.CurrentCulture, "Downloading {0} to {1} with {2} priority, {3}", source.AbsoluteUri, destinationFile.Name, priority, download.Guid))

        download.Priority = priority

        If Not requestUnconstrainedDownload Then
            ' Attach progress and completion handlers.
            Await HandleDownloadAsync(download, True)
            Return
        End If

        Dim requestOperations As New List(Of DownloadOperation)()
        requestOperations.Add(download)

        ' If the app isn't actively being used, at some point the system may slow down or pause long running
        ' downloads. The purpose of this behavior is to increase the device's battery life.
        ' By requesting unconstrained downloads, the app can request the system to not suspend any of the
        ' downloads in the list for power saving reasons.
        ' Use this API with caution since it not only may reduce battery life, but it may show a prompt to
        ' the user.
        Dim result As UnconstrainedTransferRequestResult = Await BackgroundDownloader.RequestUnconstrainedDownloadsAsync(requestOperations)

        Log(String.Format(CultureInfo.CurrentCulture, "Request for unconstrained downloads has been {0}", (If(result.IsUnconstrained, "granted", "denied"))))

        Await HandleDownloadAsync(download, True)
    End Sub

    Private Sub StartDownload_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        StartDownload(BackgroundTransferPriority.Default, False)
    End Sub

    Private Sub StartHighPriorityDownload_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        StartDownload(BackgroundTransferPriority.High, False)
    End Sub

    Private Sub StartUnconstrainedDownload_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        StartDownload(BackgroundTransferPriority.Default, True)
    End Sub

    Private Sub PauseAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Log("Downloads: " & activeDownloads.Count)

        For Each download As DownloadOperation In activeDownloads
            If download.Progress.Status = BackgroundTransferStatus.Running Then
                download.Pause()
                Log("Paused: " & download.Guid.ToString())
            Else
                Log(String.Format(CultureInfo.CurrentCulture, "Skipped: {0}, Status: {1}", download.Guid, download.Progress.Status))
            End If
        Next download
    End Sub

    Private Sub ResumeAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Log("Downloads: " & activeDownloads.Count)

        For Each download As DownloadOperation In activeDownloads
            If download.Progress.Status = BackgroundTransferStatus.PausedByApplication Then
                download.Resume()
                Log("Resumed: " & download.Guid.ToString())
            Else
                Log(String.Format(CultureInfo.CurrentCulture, "Skipped: {0}, Status: {1}", download.Guid, download.Progress.Status))
            End If
        Next download
    End Sub

    Private Sub CancelAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Log("Canceling Downloads: " & activeDownloads.Count)

        cts.Cancel()
        cts.Dispose()

        ' Re-create the CancellationTokenSource and activeDownloads for future downloads.
        cts = New CancellationTokenSource()
        activeDownloads = New List(Of DownloadOperation)()
    End Sub

    ' Note that this event is invoked on a background thread, so we cannot access the UI directly.
    Private Sub DownloadProgress(ByVal download As DownloadOperation)
        MarshalLog(String.Format(CultureInfo.CurrentCulture, "Progress: {0}, Status: {1}", download.Guid, download.Progress.Status))

        Dim percent As Double = 100
        If download.Progress.TotalBytesToReceive > 0 Then
            percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive
        End If

        MarshalLog(String.Format(CultureInfo.CurrentCulture, " - Transfered bytes: {0} of {1}, {2}%", download.Progress.BytesReceived, download.Progress.TotalBytesToReceive, percent))

        If download.Progress.HasRestarted Then
            MarshalLog(" - Download restarted")
        End If

        If download.Progress.HasResponseChanged Then
            ' We've received new response headers from the server.
            MarshalLog(" - Response updated; Header count: " & download.GetResponseInformation().Headers.Count)

            ' If you want to stream the response data this is a good time to start.
            ' download.GetResultStreamAt(0);
        End If
    End Sub

    Private Async Function HandleDownloadAsync(ByVal download As DownloadOperation, ByVal start As Boolean) As Task
        Try
            LogStatus("Running: " & download.Guid.ToString(), NotifyType.StatusMessage)

            ' Store the download so we can pause/resume.
            activeDownloads.Add(download)

            Dim progressCallback As New Progress(Of DownloadOperation)(AddressOf DownloadProgress)
            If start Then
                ' Start the download and attach a progress handler.
                Await download.StartAsync().AsTask(cts.Token, progressCallback)
            Else
                ' The download was already running when the application started, re-attach the progress handler.
                Await download.AttachAsync().AsTask(cts.Token, progressCallback)
            End If

            Dim response As ResponseInformation = download.GetResponseInformation()

            LogStatus(String.Format(CultureInfo.CurrentCulture, "Completed: {0}, Status Code: {1}", download.Guid, response.StatusCode), NotifyType.StatusMessage)
        Catch e1 As TaskCanceledException
            LogStatus("Canceled: " & download.Guid.ToString(), NotifyType.StatusMessage)
        Catch ex As Exception
            If Not IsExceptionHandled("Execution error", ex, download) Then
                Throw
            End If
        Finally
            activeDownloads.Remove(download)
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
            LogStatus(String.Format(CultureInfo.CurrentCulture, "Error: {0} - {1}: {2}", download.Guid, title, errorStatus), NotifyType.ErrorMessage)
        End If

        Return True
    End Function

    ' When operations happen on a background thread we have to marshal UI updates back to the UI thread.
    Private Sub MarshalLog(ByVal value As String)
        Dim ignore = Me.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() Log(value))
    End Sub

    Private Sub Log(ByVal message As String)
        outputField.Text += message & vbCrLf
    End Sub

    Private Sub LogStatus(ByVal message As String, ByVal type As NotifyType)
        rootPage.NotifyUser(message, type)
        Log(message)
    End Sub
End Class

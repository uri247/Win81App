'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports System.Globalization
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Pickers
Imports Windows.Web

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class S2_Upload
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private cts As CancellationTokenSource

    Private Const maxUploadFileSize As Integer = 100 * 1024 * 1024 ' 100 MB (arbitrary limit chosen for this sample)

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
        ' An application must enumerate uploads when it gets started to prevent stale downloads/uploads.
        ' Typically this can be done in the App class by overriding OnLaunched() and checking for
        ' "args.Kind == ActivationKind.Launch" to detect an actual app launch.
        ' We do it here in the sample to keep the sample code consolidated.
        Await DiscoverActiveUploadsAsync()
    End Sub

    ' Enumerate the uploads that were going on in the background while the app was closed.
    Private Async Function DiscoverActiveUploadsAsync() As Task
        Dim uploads As IReadOnlyList(Of UploadOperation) = Nothing
        Try
            uploads = Await BackgroundUploader.GetCurrentUploadsAsync()
        Catch ex As Exception
            If Not IsExceptionHandled("Discovery error", ex) Then
                Throw
            End If
            Return
        End Try

        Log("Loading background uploads: " & uploads.Count)

        If uploads.Count > 0 Then
            Dim tasks As New List(Of Task)()
            For Each upload As UploadOperation In uploads
                Log(String.Format(CultureInfo.CurrentCulture, "Discovered background upload: {0}, Status: {1}", upload.Guid, upload.Progress.Status))

                ' Attach progress and completion handlers.
                tasks.Add(HandleUploadAsync(upload, False))
            Next upload

            ' Don't await HandleUploadAsync() in the foreach loop since we would attach to the second
            ' upload only when the first one completed; attach to the third upload when the second one
            ' completes etc. We want to attach to all uploads immediately.
            ' If there are actions that need to be taken once uploads complete, await tasks here, outside
            ' the loop.
            Await Task.WhenAll(tasks)
        End If
    End Function

    Private Async Sub StartUpload_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
        ' box validating the URI is required since it was received from an untrusted source (user input).
        ' The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
        ' Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
        ' the "Home or Work Networking" capability.
        Dim uri As Uri = Nothing
        If Not System.Uri.TryCreate(serverAddressField.Text.Trim(), UriKind.Absolute, uri) Then
            rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage)
            Return
        End If

        Dim picker As New FileOpenPicker()
        picker.FileTypeFilter.Add("*")
        Dim file As StorageFile = Await picker.PickSingleFileAsync()

        If file Is Nothing Then
            rootPage.NotifyUser("No file selected.", NotifyType.ErrorMessage)
            Return
        End If

        Dim properties As BasicProperties = Await file.GetBasicPropertiesAsync()
        If properties.Size > maxUploadFileSize Then
            rootPage.NotifyUser(String.Format(CultureInfo.CurrentCulture, "Selected file exceeds max. upload file size ({0} MB).", maxUploadFileSize \ (1024 * 1024)), NotifyType.ErrorMessage)
            Return
        End If

        Dim uploader As New BackgroundUploader()
        uploader.SetRequestHeader("Filename", file.Name)

        Dim upload As UploadOperation = uploader.CreateUpload(uri, file)
        Log(String.Format(CultureInfo.CurrentCulture, "Uploading {0} to {1}, {2}", file.Name, uri.AbsoluteUri, upload.Guid))

        ' Attach progress and completion handlers.
        Await HandleUploadAsync(upload, True)
    End Sub

    Private Async Sub StartMultipartUpload_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text
        ' box validating the URI is required since it was received from an untrusted source (user input).
        ' The URI is validated by calling Uri.TryCreate() that will return 'false' for strings that are not valid URIs.
        ' Note that when enabling the text box users may provide URIs to machines on the intrAnet that require
        ' the "Home or Work Networking" capability.
        Dim uri As Uri = Nothing
        If Not System.Uri.TryCreate(serverAddressField.Text.Trim(), UriKind.Absolute, uri) Then
            rootPage.NotifyUser("Invalid URI.", NotifyType.ErrorMessage)
            Return
        End If

        Dim picker As New FileOpenPicker()
        picker.FileTypeFilter.Add("*")
        Dim files As IReadOnlyList(Of StorageFile) = Await picker.PickMultipleFilesAsync()

        If files.Count = 0 Then
            rootPage.NotifyUser("No file selected.", NotifyType.ErrorMessage)
            Return
        End If

        Dim totalFileSize As ULong = 0
        For i As Integer = 0 To files.Count - 1
            Dim properties As BasicProperties = Await files(i).GetBasicPropertiesAsync()
            totalFileSize += properties.Size

            If totalFileSize > maxUploadFileSize Then
                rootPage.NotifyUser(String.Format(CultureInfo.CurrentCulture, "Size of selected files exceeds max. upload file size ({0} MB).", maxUploadFileSize \ (1024 * 1024)), NotifyType.ErrorMessage)
                Return
            End If
        Next i

        Dim parts As New List(Of BackgroundTransferContentPart)()
        For i As Integer = 0 To files.Count - 1
            Dim part As New BackgroundTransferContentPart("File" & i, files(i).Name)
            part.SetFile(files(i))
            parts.Add(part)
        Next i

        Dim uploader As New BackgroundUploader()
        Dim upload As UploadOperation = Await uploader.CreateUploadAsync(uri, parts)

        Dim fileNames As String = files(0).Name
        For i As Integer = 1 To files.Count - 1
            fileNames &= ", " & files(i).Name
        Next i

        Log(String.Format(CultureInfo.CurrentCulture, "Uploading {0} to {1}, {2}", fileNames, uri.AbsoluteUri, upload.Guid))

        ' Attach progress and completion handlers.
        Await HandleUploadAsync(upload, True)
    End Sub

    Private Sub CancelAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Log("Canceling all active uploads")

        cts.Cancel()
        cts.Dispose()

        ' Re-create the CancellationTokenSource and activeUploads for future uploads.
        cts = New CancellationTokenSource()
    End Sub

    ' Note that this event is invoked on a background thread, so we cannot access the UI directly.
    Private Sub UploadProgress(ByVal upload As UploadOperation)
        MarshalLog(String.Format(CultureInfo.CurrentCulture, "Progress: {0}, Status: {1}", upload.Guid, upload.Progress.Status))

        Dim progress As BackgroundUploadProgress = upload.Progress

        Dim percentSent As Double = 100
        If progress.TotalBytesToSend > 0 Then
            percentSent = progress.BytesSent * 100 / progress.TotalBytesToSend
        End If

        MarshalLog(String.Format(CultureInfo.CurrentCulture, " - Sent bytes: {0} of {1} ({2}%), Received bytes: {3} of {4}", progress.BytesSent, progress.TotalBytesToSend, percentSent, progress.BytesReceived, progress.TotalBytesToReceive))

        If progress.HasRestarted Then
            MarshalLog(" - Upload restarted")
        End If

        If progress.HasResponseChanged Then
            ' We've received new response headers from the server.
            MarshalLog(" - Response updated; Header count: " & upload.GetResponseInformation().Headers.Count)

            ' If you want to stream the response data this is a good time to start.
            ' upload.GetResultStreamAt(0);
        End If
    End Sub

    Private Async Function HandleUploadAsync(ByVal upload As UploadOperation, ByVal start As Boolean) As Task
        Try
            LogStatus("Running: " & upload.Guid.ToString(), NotifyType.StatusMessage)

            Dim progressCallback As New Progress(Of UploadOperation)(AddressOf UploadProgress)
            If start Then
                ' Start the upload and attach a progress handler.
                Await upload.StartAsync().AsTask(cts.Token, progressCallback)
            Else
                ' The upload was already running when the application started, re-attach the progress handler.
                Await upload.AttachAsync().AsTask(cts.Token, progressCallback)
            End If

            Dim response As ResponseInformation = upload.GetResponseInformation()

            LogStatus(String.Format(CultureInfo.CurrentCulture, "Completed: {0}, Status Code: {1}", upload.Guid, response.StatusCode), NotifyType.StatusMessage)
        Catch e1 As TaskCanceledException
            LogStatus("Canceled: " & upload.Guid.ToString(), NotifyType.StatusMessage)
        Catch ex As Exception
            If Not IsExceptionHandled("Error", ex, upload) Then
                Throw
            End If
        End Try
    End Function

    Private Function IsExceptionHandled(ByVal title As String, ByVal ex As Exception, Optional ByVal upload As UploadOperation = Nothing) As Boolean
        Dim errorStatus As WebErrorStatus = BackgroundTransferError.GetStatus(ex.HResult)
        If errorStatus = WebErrorStatus.Unknown Then
            Return False
        End If

        If upload Is Nothing Then
            LogStatus(String.Format(CultureInfo.CurrentCulture, "Error: {0}: {1}", title, errorStatus), NotifyType.ErrorMessage)
        Else
            LogStatus(String.Format(CultureInfo.CurrentCulture, "Error: {0} - {1}: {2}", upload.Guid, title, errorStatus), NotifyType.ErrorMessage)
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

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************


Imports System
Imports System.Globalization
Imports Windows.Web.Http

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario7
    Inherits SDKTemplate.Common.LayoutAwarePage
    Implements IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private httpClient As HttpClient
    Private cts As CancellationTokenSource

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        httpClient = New HttpClient()
        cts = New CancellationTokenSource()
        UpdateAddressField()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        Dispose()
    End Sub

    Private Sub UpdateAddressField()
        ' Tell the server we want a transfer-encoding chunked response.
        Dim queryString As String = ""
        If ChunkedResponseToggle.IsOn Then
            queryString = "?chunkedResponse=1"
        End If

        Helpers.ReplaceQueryString(AddressField, queryString)
    End Sub

    Private Sub ChunkedResponseToggle_Toggled(ByVal sender As Object, ByVal e As RoutedEventArgs)
        UpdateAddressField()
    End Sub

    Private Async Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Helpers.ScenarioStarted(StartButton, CancelButton, Nothing)
        ResetFields()
        rootPage.NotifyUser("In progress", NotifyType.StatusMessage)

        Try
            ' 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
            ' text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
            Dim resourceAddress As New Uri(AddressField.Text)

            Const streamLength As UInteger = 100000
            Dim streamContent As New HttpStreamContent(New SlowInputStream(streamLength))

            ' If stream length is unknown, the request is chunked transfer encoded.
            If Not ChunkedRequestToggle.IsOn Then
                streamContent.Headers.ContentLength = streamLength
            End If

            Dim progress As IProgress(Of HttpProgress) = New Progress(Of HttpProgress)(AddressOf ProgressHandler)
            Dim response As HttpResponseMessage = Await httpClient.PostAsync(resourceAddress, streamContent).AsTask(cts.Token, progress)

            rootPage.NotifyUser("Completed", NotifyType.StatusMessage)
        Catch e1 As TaskCanceledException
            rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage)
        Catch ex As Exception
            rootPage.NotifyUser("Error: " & ex.Message, NotifyType.ErrorMessage)
        Finally
            Helpers.ScenarioCompleted(StartButton, CancelButton)
        End Try
    End Sub

    Private Sub ResetFields()
        StageField.Text = ""
        RetriesField.Text = "0"
        BytesSentField.Text = "0"
        TotalBytesToSendField.Text = "0"
        BytesReceivedField.Text = "0"
        TotalBytesToReceiveField.Text = "0"
        RequestProgressBar.Value = 0
    End Sub

    Private Sub ProgressHandler(ByVal progress As HttpProgress)
        StageField.Text = progress.Stage.ToString()
        RetriesField.Text = progress.Retries.ToString(CultureInfo.InvariantCulture)
        BytesSentField.Text = progress.BytesSent.ToString(CultureInfo.InvariantCulture)
        BytesReceivedField.Text = progress.BytesReceived.ToString(CultureInfo.InvariantCulture)

        Dim totalBytesToSend As ULong = 0
        If progress.TotalBytesToSend.HasValue Then
            totalBytesToSend = progress.TotalBytesToSend.Value
            TotalBytesToSendField.Text = totalBytesToSend.ToString(CultureInfo.InvariantCulture)
        Else
            TotalBytesToSendField.Text = "unknown"
        End If

        Dim totalBytesToReceive As ULong = 0
        If progress.TotalBytesToReceive.HasValue Then
            totalBytesToReceive = progress.TotalBytesToReceive.Value
            TotalBytesToReceiveField.Text = totalBytesToReceive.ToString(CultureInfo.InvariantCulture)
        Else
            TotalBytesToReceiveField.Text = "unknown"
        End If

        Dim requestProgress As Double = 0
        If progress.Stage = HttpProgressStage.SendingContent AndAlso totalBytesToSend > 0 Then
            requestProgress = progress.BytesSent * 50.0 / totalBytesToSend
        ElseIf progress.Stage = HttpProgressStage.ReceivingContent Then
            ' Start with 50 percent, request content was already sent.
            requestProgress += 50

            If totalBytesToReceive > 0 Then
                requestProgress += progress.BytesReceived * 50 / totalBytesToReceive
            End If
        Else
            Return
        End If

        RequestProgressBar.Value = requestProgress
    End Sub

    Private Sub Cancel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        cts.Cancel()
        cts.Dispose()

        ' Re-create the CancellationTokenSource.
        cts = New CancellationTokenSource()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If httpClient IsNot Nothing Then
            httpClient.Dispose()
            httpClient = Nothing
        End If

        If cts IsNot Nothing Then
            cts.Dispose()
            cts = Nothing
        End If
    End Sub
End Class

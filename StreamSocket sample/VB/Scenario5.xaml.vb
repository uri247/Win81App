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
Imports System.Text
Imports Windows.Networking
Imports Windows.Networking.Sockets
Imports Windows.Security.Cryptography.Certificates
Imports Windows.UI.Popups

''' <summary>
''' A page for second scenario.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private Const continueButtonId As Integer = 1
    Private Const abortButtonId As Integer = 0

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser().
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'ConnectSocket' button.
    ''' </summary>
    ''' <param name="sender">Object for which the event was generated.</param>
    ''' <param name="e">Event's parameters.</param>
    Private Async Sub ConnectSocket_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' By default 'HostNameForConnect' and 'ServiceNameForConnect' are disabled and host name validation 
        ' is not required. When enabling the text box validating the host name is required since it was received 
        ' from an untrusted source (user input). The host name is validated by catching ArgumentExceptions thrown 
        ' by the HostName constructor for invalid input.
        ' Note that when enabling the text box users may provide names for hosts on the Internet that require the
        ' "Internet (Client)" capability.
        If String.IsNullOrEmpty(ServiceNameForConnect.Text) Then
            rootPage.NotifyUser("Please provide a service name.", NotifyType.ErrorMessage)
            Return
        End If

        Dim hostName As HostName
        Try
            hostName = New HostName(HostNameForConnect.Text)
        Catch e1 As ArgumentException
            rootPage.NotifyUser("Error: Invalid host name.", NotifyType.ErrorMessage)
            Return
        End Try

        rootPage.NotifyUser("Connecting to: " & HostNameForConnect.Text, NotifyType.StatusMessage)

        Using socket As New Sockets.StreamSocket()
            Dim shouldRetry As Boolean = Await TryConnectSocketWithRetryAsync(socket, hostName)
            If shouldRetry Then
                ' Retry if the first attempt failed because of SSL errors.
                Await TryConnectSocketWithRetryAsync(socket, hostName)
            End If
        End Using
    End Sub

    Private Async Function TryConnectSocketWithRetryAsync(ByVal socket As Sockets.StreamSocket, ByVal hostName As HostName) As Task(Of Boolean)
        Try
            ' Connect to the server (in our case the local IIS server).
            Await socket.ConnectAsync(hostName, ServiceNameForConnect.Text, SocketProtectionLevel.Tls12)

            Dim certInformation As String = GetCertificateInformation(socket.Information.ServerCertificate, socket.Information.ServerIntermediateCertificates)

            rootPage.NotifyUser("Connected to server. Certificate information: " & Environment.NewLine & certInformation, NotifyType.StatusMessage)
            Return False
        Catch exception As Exception
            ' If this is an unknown status it means that the error is fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            ' If the exception was caused by an SSL error that is ignorable we are going to prompt the user
            ' with an enumeration of the errors and ask for permission to ignore.
            If socket.Information.ServerCertificateErrorSeverity <> SocketSslErrorSeverity.Ignorable Then
                rootPage.NotifyUser("Connect failed with error: " & exception.Message, NotifyType.ErrorMessage)
                Return False
            End If
        End Try

        ' Present the certificate issues and ask the user if we should continue.
        If Await ShouldIgnoreCertificateErrorsAsync(socket.Information.ServerCertificateErrors) Then
            ' ---------------------------------------------------------------------------
            ' WARNING: Only test applications may ignore SSL errors.
            ' In real applications, ignoring server certificate errors can lead to MITM
            ' attacks (while the connection is secure, the server is not authenticated).
            ' ---------------------------------------------------------------------------
            socket.Control.IgnorableServerCertificateErrors.Clear()
            For Each ignorableError In socket.Information.ServerCertificateErrors
                socket.Control.IgnorableServerCertificateErrors.Add(ignorableError)
            Next ignorableError
            rootPage.NotifyUser("Retrying connection", NotifyType.StatusMessage)
            Return True
        Else
            rootPage.NotifyUser("Connection aborted by user (certificate not trusted)", NotifyType.ErrorMessage)
            Return False
        End If
    End Function

    ''' <summary>
    ''' Allows the user to abort the connection in case of SSL errors
    ''' </summary>
    ''' <param name="connectionErrors">A string that contains the certificate errors</param>
    ''' <returns>False if the connection should be aborted</returns>
    Private Async Function ShouldIgnoreCertificateErrorsAsync(ByVal serverCertificateErrors As IReadOnlyList(Of ChainValidationResult)) As Task(Of Boolean)
        Dim connectionErrors As String = String.Join(", ", serverCertificateErrors)

        Dim dialogMessage As String = "The remote server certificate validation failed with the following errors: " & connectionErrors & Environment.NewLine & "Security certificate problems may" & " indicate an attempt to fool you or intercept any data you send to the server."

        Dim dialog As New MessageDialog(dialogMessage, "Server Certificate Validation Errors")

        dialog.Commands.Add(New UICommand("Continue (not recommended)", Nothing, continueButtonId))
        dialog.Commands.Add(New UICommand("Cancel", Nothing, abortButtonId))
        dialog.DefaultCommandIndex = 1
        dialog.CancelCommandIndex = 1

        Dim selected As IUICommand = Await dialog.ShowAsync()
        Return (TypeOf selected.Id Is Integer) AndAlso (CInt(selected.Id) = continueButtonId)
    End Function

    ''' <summary>
    ''' Gets detailed certificate information
    ''' </summary>
    ''' <param name="serverCert">The server certificate</param>
    ''' <param name="intermediateCertificates">The server certificate chain</param>
    ''' <returns>A string containing certificate details</returns>
    Private Function GetCertificateInformation(ByVal serverCert As Certificate, ByVal intermediateCertificates As IReadOnlyList(Of Certificate)) As String
        Dim sb As New StringBuilder()

        sb.AppendLine(vbTab & "Friendly Name: " & serverCert.FriendlyName)
        sb.AppendLine(vbTab & "Subject: " & serverCert.Subject)
        sb.AppendLine(vbTab & "Issuer: " & serverCert.Issuer)
        sb.AppendLine(vbTab & "Validity: " & serverCert.ValidFrom.ToString() & " - " & serverCert.ValidTo.ToString())

        ' Enumerate the entire certificate chain.
        If intermediateCertificates.Count > 0 Then
            sb.AppendLine(vbTab & "Certificate chain: ")
            For Each cert In intermediateCertificates
                sb.AppendLine(vbTab & vbTab & "Intermediate Certificate Subject: " & cert.Subject)
            Next cert
        Else
            sb.AppendLine(vbTab & "No certificates within the intermediate chain.")
        End If

        Return sb.ToString()
    End Function
End Class

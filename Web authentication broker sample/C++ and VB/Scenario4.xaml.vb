'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Security.Authentication.Web


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
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


    Private Sub DebugPrint(ByVal Trace As String)
        Dim GoogleDebugArea As TextBox = TryCast(rootPage.FindName("GoogleDebugArea"), TextBox)
        GoogleDebugArea.Text += Trace & vbCrLf
    End Sub

    Private Sub OutputToken(ByVal TokenUri As String)
        Dim GoogleReturnedToken As TextBox = TryCast(rootPage.FindName("GoogleReturnedToken"), TextBox)
        GoogleReturnedToken.Text = TokenUri
    End Sub

    Private Async Sub Launch_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If GoogleClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage)
        ElseIf GoogleCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage)
        End If

        Try
            Dim GoogleURL As String = "https://accounts.google.com/o/oauth2/auth?client_id=" & Uri.EscapeDataString(GoogleClientID.Text) & "&redirect_uri=" & Uri.EscapeDataString(GoogleCallbackUrl.Text) & "&response_type=code&scope=" & Uri.EscapeDataString("http://picasaweb.google.com/data")

            Dim StartUri As System.Uri = New Uri(GoogleURL)
            ' When using the desktop flow, the success code is displayed in the html title of this end uri
            Dim EndUri As System.Uri = New Uri("https://accounts.google.com/o/oauth2/approval?")

            DebugPrint("Navigating to: " & GoogleURL)

            Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, StartUri, EndUri)
            If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                OutputToken(WebAuthenticationResult.ResponseData.ToString())
            ElseIf WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.ErrorHttp Then
                OutputToken("HTTP Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseErrorDetail.ToString())
            Else
                OutputToken("Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseStatus.ToString())
            End If
        Catch [Error] As Exception
            '
            ' Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString())
        End Try
    End Sub


End Class

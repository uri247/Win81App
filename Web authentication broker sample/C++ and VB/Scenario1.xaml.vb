'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Security.Authentication.Web
Imports Windows.Data.Json
Imports Windows.Web.Http

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
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

        Dim FacebookDebugArea As TextBox = TryCast(rootPage.FindName("FacebookDebugArea"), TextBox)
        FacebookDebugArea.Text += Trace & vbCrLf
    End Sub

    Private Sub OutputToken(ByVal TokenUri As String)

        Dim FacebookReturnedToken As TextBox = TryCast(rootPage.FindName("FacebookReturnedToken"), TextBox)
        FacebookReturnedToken.Text = TokenUri
    End Sub

    Private Async Sub Launch_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If FacebookClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage)
        ElseIf FacebookCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage)
        End If

        Try
            Dim FacebookURL As String = "https://www.facebook.com/dialog/oauth?client_id=" & Uri.EscapeDataString(FacebookClientID.Text) & "&redirect_uri=" & Uri.EscapeDataString(FacebookCallbackUrl.Text) & "&scope=read_stream&display=popup&response_type=token"

            Dim StartUri As System.Uri = New Uri(FacebookURL)
            Dim EndUri As System.Uri = New Uri(FacebookCallbackUrl.Text)

            DebugPrint("Navigating to: " & FacebookURL)

            Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri)
            If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                OutputToken(WebAuthenticationResult.ResponseData.ToString())
                Await GetFacebookUserNameAsync(WebAuthenticationResult.ResponseData.ToString())
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

    ''' <summary>
    ''' This function extracts access_token from the response returned from web authentication broker
    ''' and uses that token to get user information using facebook graph api. 
    ''' </summary>
    ''' <param name="webAuthResultResponseData">responseData returned from AuthenticateAsync result.</param>
    Private Async Function GetFacebookUserNameAsync(ByVal webAuthResultResponseData As String) As Task
        'Get Access Token first
        Dim responseData As String = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("access_token"))
        Dim keyValPairs() As String = responseData.Split("&"c)
        Dim access_token As String = Nothing
        Dim expires_in As String = Nothing
        For i As Integer = 0 To keyValPairs.Length - 1
            Dim splits() As String = keyValPairs(i).Split("="c)
            Select Case splits(0)
                Case "access_token"
                    access_token = splits(1) 'you may want to store access_token for further use. Look at Scenario5 (Account Management).
                Case "expires_in"
                    expires_in = splits(1)
            End Select
        Next i

        DebugPrint("access_token = " & access_token)
        'Request User info.
        Dim httpClient As New HttpClient()
        Dim response As String = Await httpClient.GetStringAsync(New Uri("https://graph.facebook.com/me?access_token=" & access_token))
        Dim value As JsonObject = JsonValue.Parse(response).GetObject()
        Dim facebookUserName As String = value.GetNamedString("name")

        rootPage.NotifyUser(facebookUserName & " is connected!!", NotifyType.StatusMessage)
    End Function
End Class

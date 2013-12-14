'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Security.Authentication.Web
Imports Windows.Security.Cryptography
Imports Windows.Security.Cryptography.Core
Imports Windows.Storage.Streams
Imports Windows.Web.Http
Imports Windows.Web.Http.Headers

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
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

    Private Async Function SendDataAsync(ByVal Url As String) As Task(Of String)
        Try
            Dim httpClient As New HttpClient()
            Return Await httpClient.GetStringAsync(New Uri(Url))
        Catch Err As Exception
            rootPage.NotifyUser("Error getting data from server." & Err.Message, NotifyType.StatusMessage)
        End Try

        Return Nothing
    End Function

    Private Sub DebugPrint(ByVal Trace As String)

        Dim TwitterDebugArea As TextBox = TryCast(rootPage.FindName("TwitterDebugArea"), TextBox)
        TwitterDebugArea.Text += Trace & vbCrLf
    End Sub

    Private Sub OutputToken(ByVal TokenUri As String)

        Dim TwitterReturnedToken As TextBox = TryCast(rootPage.FindName("TwitterReturnedToken"), TextBox)
        TwitterReturnedToken.Text = TokenUri
    End Sub


    Private Async Sub Launch_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If TwitterClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage)
        ElseIf TwitterCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage)
        ElseIf TwitterClientSecret.Text = "" Then
            rootPage.NotifyUser("Please enter an Client Secret.", NotifyType.StatusMessage)
        End If

        Try
            Dim oauth_token As String = Await GetTwitterRequestTokenAsync(TwitterCallbackUrl.Text, TwitterClientID.Text)
            Dim TwitterUrl As String = "https://api.twitter.com/oauth/authorize?oauth_token=" & oauth_token
            Dim StartUri As System.Uri = New Uri(TwitterUrl)
            Dim EndUri As System.Uri = New Uri(TwitterCallbackUrl.Text)

            DebugPrint("Navigating to: " & TwitterUrl)

            Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri)
            If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                OutputToken(WebAuthenticationResult.ResponseData.ToString())
                Await GetTwitterUserNameAsync(WebAuthenticationResult.ResponseData.ToString())
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

    Private Async Function GetTwitterUserNameAsync(ByVal webAuthResultResponseData As String) As Task
        '
        ' Acquiring a access_token first
        '

        Dim responseData As String = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("oauth_token"))
        Dim request_token As String = Nothing
        Dim oauth_verifier As String = Nothing
        Dim keyValPairs() As String = responseData.Split("&"c)

        For i As Integer = 0 To keyValPairs.Length - 1
            Dim splits() As String = keyValPairs(i).Split("="c)
            Select Case splits(0)
                Case "oauth_token"
                    request_token = splits(1)
                Case "oauth_verifier"
                    oauth_verifier = splits(1)
            End Select
        Next i

        Dim TwitterUrl As String = "https://api.twitter.com/oauth/access_token"

        Dim timeStamp As String = GetTimeStamp()
        Dim nonce As String = GetNonce()

        Dim SigBaseStringParams As String = "oauth_consumer_key=" & TwitterClientID.Text
        SigBaseStringParams &= "&" & "oauth_nonce=" & nonce
        SigBaseStringParams &= "&" & "oauth_signature_method=HMAC-SHA1"
        SigBaseStringParams &= "&" & "oauth_timestamp=" & timeStamp
        SigBaseStringParams &= "&" & "oauth_token=" & request_token
        SigBaseStringParams &= "&" & "oauth_version=1.0"
        Dim SigBaseString As String = "POST&"
        SigBaseString &= Uri.EscapeDataString(TwitterUrl) & "&" & Uri.EscapeDataString(SigBaseStringParams)

        Dim Signature As String = GetSignature(SigBaseString, TwitterClientSecret.Text)

        Dim httpContent As New HttpStringContent("oauth_verifier=" & oauth_verifier, Windows.Storage.Streams.UnicodeEncoding.Utf8)
        httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
        Dim authorizationHeaderParams As String = "oauth_consumer_key=""" & TwitterClientID.Text & """, oauth_nonce=""" & nonce & """, oauth_signature_method=""HMAC-SHA1"", oauth_signature=""" & Uri.EscapeDataString(Signature) & """, oauth_timestamp=""" & timeStamp & """, oauth_token=""" & Uri.EscapeDataString(request_token) & """, oauth_version=""1.0"""

        Dim httpClient As New HttpClient()

        httpClient.DefaultRequestHeaders.Authorization = New HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams)
        Dim httpResponseMessage = Await httpClient.PostAsync(New Uri(TwitterUrl), httpContent)
        Dim response As String = Await httpResponseMessage.Content.ReadAsStringAsync()

        Dim Tokens() As String = response.Split("&"c)
        Dim oauth_token_secret As String = Nothing
        Dim access_token As String = Nothing
        Dim screen_name As String = Nothing

        For i As Integer = 0 To Tokens.Length - 1
            Dim splits() As String = Tokens(i).Split("="c)
            Select Case splits(0)
                Case "screen_name"
                    screen_name = splits(1)
                Case "oauth_token"
                    access_token = splits(1)
                Case "oauth_token_secret"
                    oauth_token_secret = splits(1)
            End Select
        Next i


        'you can store access_token and oauth_token_secret for further use. See Scenario5(Account Management).
        If access_token IsNot Nothing Then
            DebugPrint("access_token = " & access_token)
        End If

        If oauth_token_secret IsNot Nothing Then
            DebugPrint("oauth_token_secret = " & oauth_token_secret)
        End If
        If screen_name IsNot Nothing Then
            rootPage.NotifyUser(screen_name & " is connected!!", NotifyType.StatusMessage)
        End If
    End Function

    Private Async Function GetTwitterRequestTokenAsync(ByVal twitterCallbackUrl As String, ByVal consumerKey As String) As Task(Of String)
        '
        ' Acquiring a request token
        '
        Dim TwitterUrl As String = "https://api.twitter.com/oauth/request_token"

        Dim nonce As String = GetNonce()
        Dim timeStamp As String = GetTimeStamp()
        Dim SigBaseStringParams As String = "oauth_callback=" & Uri.EscapeDataString(twitterCallbackUrl)
        SigBaseStringParams &= "&" & "oauth_consumer_key=" & consumerKey
        SigBaseStringParams &= "&" & "oauth_nonce=" & nonce
        SigBaseStringParams &= "&" & "oauth_signature_method=HMAC-SHA1"
        SigBaseStringParams &= "&" & "oauth_timestamp=" & timeStamp
        SigBaseStringParams &= "&" & "oauth_version=1.0"
        Dim SigBaseString As String = "GET&"
        SigBaseString &= Uri.EscapeDataString(TwitterUrl) & "&" & Uri.EscapeDataString(SigBaseStringParams)
        Dim Signature As String = GetSignature(SigBaseString, TwitterClientSecret.Text)

        TwitterUrl &= "?" & SigBaseStringParams & "&oauth_signature=" & Uri.EscapeDataString(Signature)
        Dim httpClient As New HttpClient()
        Dim GetResponse As String = Await httpClient.GetStringAsync(New Uri(TwitterUrl))

        DebugPrint("Received Data: " & GetResponse)

        Dim request_token As String = Nothing
        Dim oauth_token_secret As String = Nothing
        Dim keyValPairs() As String = GetResponse.Split("&"c)

        For i As Integer = 0 To keyValPairs.Length - 1
            Dim splits() As String = keyValPairs(i).Split("="c)
            Select Case splits(0)
                Case "oauth_token"
                    request_token = splits(1)
                Case "oauth_token_secret"
                    oauth_token_secret = splits(1)
            End Select
        Next i

        Return request_token
    End Function

    Private Function GetNonce() As String
        Dim rand As New Random()
        Dim nonce As Integer = rand.Next(1000000000)
        Return nonce.ToString()
    End Function

    Private Function GetTimeStamp() As String
        Dim SinceEpoch As TimeSpan = Date.UtcNow - New Date(1970, 1, 1)
        Return Math.Round(SinceEpoch.TotalSeconds).ToString()
    End Function

    Private Function GetSignature(ByVal sigBaseString As String, ByVal consumerSecretKey As String) As String
        Dim KeyMaterial As IBuffer = CryptographicBuffer.ConvertStringToBinary(consumerSecretKey & "&", BinaryStringEncoding.Utf8)
        Dim HmacSha1Provider As MacAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1")
        Dim MacKey As CryptographicKey = HmacSha1Provider.CreateKey(KeyMaterial)
        Dim DataToBeSigned As IBuffer = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8)
        Dim SignatureBuffer As IBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned)
        Dim Signature As String = CryptographicBuffer.EncodeToBase64String(SignatureBuffer)

        Return Signature
    End Function

End Class

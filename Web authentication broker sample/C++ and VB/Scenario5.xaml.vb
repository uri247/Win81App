'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Data.Json
Imports Windows.Security.Authentication.Web
Imports Windows.Security.Cryptography
Imports Windows.Security.Cryptography.Core
Imports Windows.Security.Credentials
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.UI.Popups
Imports Windows.UI.ApplicationSettings
Imports Windows.Web.Http
Imports Windows.Web.Http.Headers

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()

    Private rootPage As MainPage = MainPage.Current

    Private facebookUserName As String
    Private twitterUserName As String
    Private facebookAccount As WebAccount
    Private twitterAccount As WebAccount
    Private facebookProvider As WebAccountProvider
    Private twitterProvider As WebAccountProvider

    Private isFacebookUserLoggedIn As Boolean
    Private isTwitterUserLoggedIn As Boolean

    Private Const FACEBOOK_ID As String = "Facebook.com"
    Private Const FACEBOOK_DISPLAY_NAME As String = "Facebook"
    Private Const TWITTER_ID As String = "Twitter.com"
    Private Const TWITTER_DISPLAY_NAME As String = "Twitter"

    Private Const FACEBOOK_OAUTH_TOKEN As String = "FACEBOOK_OAUTH_TOKEN"
    Private Const TWITTER_OAUTH_TOKEN As String = "TWITTER_OAUTH_TOKEN"
    Private Const TWITTER_OAUTH_TOKEN_SECRET As String = "TWITTER_OAUTH_TOKEN_SECRET"
    Private Const FACEBOOK_USER_NAME As String = "FACEBOOK_USER_NAME"
    Private Const TWITTER_USER_NAME As String = "TWITTER_USER_NAME"

    Private roamingSettings As ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings


    Public Sub New()
        Me.InitializeComponent()

        InitializeWebAccountProviders()
        InitializeWebAccounts()

        AddHandler SettingsPane.GetForCurrentView().CommandsRequested, AddressOf CommandsRequested
        AddHandler AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested, AddressOf AccountCommandsRequested
    End Sub

    Private Sub InitializeWebAccountProviders()
        facebookProvider = New WebAccountProvider(FACEBOOK_ID, FACEBOOK_DISPLAY_NAME, New Uri("ms-appx:///icons/Facebook.png"))

        twitterProvider = New WebAccountProvider(TWITTER_ID, TWITTER_DISPLAY_NAME, New Uri("ms-appx:///icons/Twitter.png"))
    End Sub

    Private Sub InitializeWebAccounts()
        'Initialize facebok account object if user was already logged in.
        Dim facebookToken As Object = roamingSettings.Values(FACEBOOK_OAUTH_TOKEN)
        If facebookToken Is Nothing Then
            isFacebookUserLoggedIn = False
        Else
            Dim facebookUser As Object = roamingSettings.Values(FACEBOOK_USER_NAME)
            If facebookUser IsNot Nothing Then
                facebookUserName = facebookUser.ToString()
                facebookAccount = New WebAccount(facebookProvider, facebookUserName, WebAccountState.Connected)
                isFacebookUserLoggedIn = True
            End If
        End If

        'Initialize twitter account if user was already logged in.
        Dim twitterToken As Object = roamingSettings.Values(TWITTER_OAUTH_TOKEN)
        Dim twitterTokenSecret As Object = roamingSettings.Values(TWITTER_OAUTH_TOKEN_SECRET)
        If twitterToken Is Nothing OrElse twitterTokenSecret Is Nothing Then
            isTwitterUserLoggedIn = False
        Else
            Dim twitteruser As Object = roamingSettings.Values(TWITTER_USER_NAME)
            If twitteruser IsNot Nothing Then
                twitterUserName = twitteruser.ToString()
                twitterAccount = New WebAccount(twitterProvider, twitterUserName, WebAccountState.Connected)
                isTwitterUserLoggedIn = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' Invoked when the navigation is about to change to a different page. You can use this function for cleanup.
    ''' </summary>
    ''' <param name="e">Event data describing the conditions that led to the event.</param>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested, AddressOf AccountCommandsRequested
    End Sub

#Region "AccountsSettings pane functions"

    ''' <summary>
    ''' This event is generated when the user opens the settings pane. During this event, append your
    ''' SettingsCommand objects to the available ApplicationCommands vector to make them available to the
    ''' SettingsPange UI.
    ''' </summary>
    ''' <param name="settingsPane">Instance that triggered the event.</param>
    ''' <param name="eventArgs">Event data describing the conditions that led to the event.</param>
    Private Sub CommandsRequested(ByVal settingsPane As SettingsPane, ByVal eventArgs As SettingsPaneCommandsRequestedEventArgs)
        eventArgs.Request.ApplicationCommands.Add(SettingsCommand.AccountsCommand) 'This will add Accounts command in settings pane
    End Sub

    ''' <summary>
    ''' This event is generated when the user clicks on Accounts command in settings pane. During this event, add your
    ''' WebAccountProviderCommand, WebAccountCommand, CredentialCommand and  SettingsCommand objects to make them available to the
    ''' AccountsSettingsPane UI.
    ''' </summary>
    ''' <param name="accountsSettingsPane">Instance that triggered the event.</param>
    ''' <param name="eventArgs">Event data describing the conditions that led to the event.</param>
    Private Sub AccountCommandsRequested(ByVal accountsSettingsPane As AccountsSettingsPane, ByVal eventArgs As AccountsSettingsPaneCommandsRequestedEventArgs)
        Dim deferral = eventArgs.GetDeferral()

        'Add header text.
        eventArgs.HeaderText = "This is sample text. You can put a message here to give context to user. This section is optional."

        'Add WebAccountProviders
        Dim providerCmdHandler As New WebAccountProviderCommandInvokedHandler(AddressOf WebAccountProviderInvokedHandler)
        Dim facebookProviderCommand As New WebAccountProviderCommand(facebookProvider, AddressOf WebAccountProviderInvokedHandler)
        eventArgs.WebAccountProviderCommands.Add(facebookProviderCommand)
        Dim twitterProviderCommand As New WebAccountProviderCommand(twitterProvider, AddressOf WebAccountProviderInvokedHandler)
        eventArgs.WebAccountProviderCommands.Add(twitterProviderCommand)

        'Add WebAccounts if available.
        Dim accountCmdHandler As New WebAccountCommandInvokedHandler(AddressOf WebAccountInvokedHandler)

        If isFacebookUserLoggedIn Then
            facebookAccount = New WebAccount(facebookProvider, facebookUserName, WebAccountState.Connected)
            Dim facebookAccountCommand As New WebAccountCommand(facebookAccount, AddressOf WebAccountInvokedHandler, SupportedWebAccountActions.Remove Or SupportedWebAccountActions.Manage)
            eventArgs.WebAccountCommands.Add(facebookAccountCommand)
        End If

        If isTwitterUserLoggedIn Then
            twitterAccount = New WebAccount(twitterProvider, twitterUserName, WebAccountState.Connected)
            Dim twitterAccountCommand As New WebAccountCommand(twitterAccount, AddressOf WebAccountInvokedHandler, SupportedWebAccountActions.Remove Or SupportedWebAccountActions.Manage)
            eventArgs.WebAccountCommands.Add(twitterAccountCommand)
        End If

        ' Add links if needed.
        Dim commandID As Object = 1
        Dim _globalLinkInvokedHandler As New UICommandInvokedHandler(AddressOf GlobalLinkInvokedhandler)
        Dim command As New SettingsCommand(commandID, "More details", _globalLinkInvokedHandler)
        eventArgs.Commands.Add(command)

        Dim command1 As New SettingsCommand(commandID, "Privacy policy", _globalLinkInvokedHandler)
        eventArgs.Commands.Add(command1)

        deferral.Complete()

    End Sub

    ''' <summary>
    ''' This is the event handler for links added to Accounts Settings pane. This method can do more work based on selected link.
    ''' </summary>
    ''' <param name="command">Link instance that triggered the event.</param>
    Private Sub GlobalLinkInvokedhandler(ByVal command As IUICommand)
        OutputText("Link clicked: " & command.Label)
    End Sub

    ''' <summary>
    ''' This event is generated when the user clicks on action button on account details pane. This method is 
    ''' responsible for handling what to do with selected action.
    ''' </summary>
    ''' <param name="command">Instance that triggered the event.</param>
    ''' <param name="eventArgs">Event data describing the conditions that led to the event.</param>
    Private Sub WebAccountInvokedHandler(ByVal command As WebAccountCommand, ByVal eventArgs As WebAccountInvokedArgs)
        OutputText("Account State = " & command.WebAccount.State.ToString() & " and Selected Action = " & eventArgs.Action)

        If eventArgs.Action = WebAccountAction.Remove Then
            'Remove user logon information since user requested to remove account.
            If command.WebAccount.WebAccountProvider.Id.Equals(FACEBOOK_ID) Then
                roamingSettings.Values.Remove(FACEBOOK_USER_NAME)
                roamingSettings.Values.Remove(FACEBOOK_OAUTH_TOKEN)
                isFacebookUserLoggedIn = False
            ElseIf command.WebAccount.WebAccountProvider.Id.Equals(TWITTER_ID) Then
                roamingSettings.Values.Remove(TWITTER_USER_NAME)
                roamingSettings.Values.Remove(TWITTER_OAUTH_TOKEN)
                isTwitterUserLoggedIn = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' This event is generated when the user clicks on Account provider tile. This method is 
    ''' responsible for deciding what to do further.
    ''' </summary>
    ''' <param name="command">WebAccountProviderCommand instance that triggered the event.</param>
    Private Async Sub WebAccountProviderInvokedHandler(ByVal command As WebAccountProviderCommand)
        If command.WebAccountProvider.Id.Equals(FACEBOOK_ID) Then
            If Not isFacebookUserLoggedIn Then
                Await AuthenticateToFacebookAsync()
            Else
                OutputText("User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.")
            End If
        ElseIf command.WebAccountProvider.Id.Equals(TWITTER_ID) Then
            If Not isTwitterUserLoggedIn Then
                Await AuthenticateToTwitterAsync()
            Else
                OutputText("User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Event handler for Show button. This method demonstrates how to show AccountsSettings pane programatically.
    ''' </summary>
    ''' <param name="sender">Instance that triggered the event.</param>
    ''' <param name="e">Event data describing the conditions that led to the event.</param>
    Private Sub Show_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        AccountsSettingsPane.Show()
    End Sub

#End Region


#Region "Facebook authentication and related functions"

    Private Async Function AuthenticateToFacebookAsync() As Task
        If FacebookClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage)
            Return
        ElseIf FacebookCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage)
            Return
        End If

        Try
            Dim FacebookURL As String = "https://www.facebook.com/dialog/oauth?client_id=" & Uri.EscapeDataString(FacebookClientID.Text) & "&redirect_uri=" & Uri.EscapeDataString(FacebookCallbackUrl.Text) & "&scope=read_stream&display=popup&response_type=token"

            Dim StartUri As System.Uri = New Uri(FacebookURL)
            Dim EndUri As System.Uri = New Uri(FacebookCallbackUrl.Text)

            Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri)
            If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                OutputText(WebAuthenticationResult.ResponseData.ToString())
                Await GetFacebookUserNameAsync(WebAuthenticationResult.ResponseData.ToString())
                isFacebookUserLoggedIn = True
            ElseIf WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.ErrorHttp Then
                OutputText("HTTP Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseErrorDetail.ToString())
            Else
                OutputText("Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseStatus.ToString())
            End If

        Catch [Error] As Exception
            '
            ' Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString())
        End Try


    End Function

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
                    access_token = splits(1)
                Case "expires_in"
                    expires_in = splits(1)
            End Select
        Next i

        roamingSettings.Values(FACEBOOK_OAUTH_TOKEN) = access_token 'store access token locally for further use.
        DebugPrint("access_token = " & access_token)

        'Request User info.
        Dim httpClient As New HttpClient()
        Dim response As String = Await httpClient.GetStringAsync(New Uri("https://graph.facebook.com/me?access_token=" & access_token))
        Dim value As JsonObject = JsonValue.Parse(response).GetObject()

        facebookUserName = value.GetNamedString("name")
        roamingSettings.Values(FACEBOOK_USER_NAME) = facebookUserName 'store user name locally for further use.
        rootPage.NotifyUser(facebookUserName & " is connected!!", NotifyType.StatusMessage)

    End Function

#End Region

#Region "Twitter authentication and related functions"
    Private Async Function AuthenticateToTwitterAsync() As Task
        If TwitterClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage)
            Return
        ElseIf TwitterCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage)
            Return
        ElseIf TwitterClientSecret.Text = "" Then
            rootPage.NotifyUser("Please enter an Client Secret.", NotifyType.StatusMessage)
            Return
        End If

        Try
            Dim oauth_token As String = Await GetTwitterRequestTokenAsync(TwitterCallbackUrl.Text, TwitterClientID.Text)
            Dim TwitterUrl As String = "https://api.twitter.com/oauth/authorize?oauth_token=" & oauth_token
            Dim StartUri As System.Uri = New Uri(TwitterUrl)
            Dim EndUri As System.Uri = New Uri(TwitterCallbackUrl.Text)

            Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri)
            If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                OutputText(WebAuthenticationResult.ResponseData.ToString())
                Await GetTwitterUserNameAsync(WebAuthenticationResult.ResponseData.ToString())
                isTwitterUserLoggedIn = True
            ElseIf WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.ErrorHttp Then
                OutputText("HTTP Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseErrorDetail.ToString())
            Else
                OutputText("Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseStatus.ToString())
            End If


        Catch [Error] As Exception
            '
            ' Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString())
        End Try
    End Function

    ''' <summary>
    ''' This function extracts oauth_token and oauth_verifier from the response returned from web authentication broker
    ''' and uses that token to get Twitter access token. 
    ''' </summary>
    ''' <param name="webAuthResultResponseData">responseData returned from AuthenticateAsync result.</param>
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

        If access_token IsNot Nothing Then
            roamingSettings.Values(TWITTER_OAUTH_TOKEN) = access_token 'store access token for further use.
            DebugPrint("access_token = " & access_token)
        End If

        If oauth_token_secret IsNot Nothing Then
            roamingSettings.Values(TWITTER_OAUTH_TOKEN_SECRET) = oauth_token_secret 'store token secret for further use.
            DebugPrint("oauth_token_secret = " & oauth_token_secret)
        End If

        If screen_name IsNot Nothing Then
            twitterUserName = screen_name
            roamingSettings.Values(TWITTER_USER_NAME) = twitterUserName 'Store user name locally for further use.
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

#End Region


    Private Sub OutputText(ByVal text As String)
        OutPutTextArea.Text = text & vbCrLf
    End Sub

    Private Sub DebugPrint(ByVal Trace As String)
        OutPutTextArea.Text += Trace & vbCrLf
    End Sub
End Class

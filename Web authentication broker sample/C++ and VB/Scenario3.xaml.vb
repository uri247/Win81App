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


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
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
        Dim FlickrDebugArea As TextBox = TryCast(rootPage.FindName("FlickrDebugArea"), TextBox)
        FlickrDebugArea.Text += Trace & vbCrLf
    End Sub

    Private Sub OutputToken(ByVal TokenUri As String)
        Dim FlickrReturnedToken As TextBox = TryCast(rootPage.FindName("FlickrReturnedToken"), TextBox)
        FlickrReturnedToken.Text = TokenUri
    End Sub


    Private Async Sub Launch_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If FlickrClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage)
        ElseIf FlickrCallbackUrl.Text = "" Then
            rootPage.NotifyUser("Please enter an Callback URL.", NotifyType.StatusMessage)
        ElseIf FlickrClientSecret.Text = "" Then
            rootPage.NotifyUser("Please enter an Client Secret.", NotifyType.StatusMessage)
        End If

        Try
            '
            ' Acquiring a request token
            '
            Dim SinceEpoch As TimeSpan = Date.UtcNow - New Date(1970, 1, 1)
            Dim Rand As New Random()
            Dim FlickrUrl As String = "https://secure.flickr.com/services/oauth/request_token"
            Dim Nonce As Int32 = Rand.Next(1000000000)
            '
            ' Compute base signature string and sign it.
            '    This is a common operation that is required for all requests even after the token is obtained.
            '    Parameters need to be sorted in alphabetical order
            '    Keys and values should be URL Encoded.
            '
            Dim SigBaseStringParams As String = "oauth_callback=" & Uri.EscapeDataString(FlickrCallbackUrl.Text)
            SigBaseStringParams &= "&" & "oauth_consumer_key=" & FlickrClientID.Text
            SigBaseStringParams &= "&" & "oauth_nonce=" & Nonce.ToString()
            SigBaseStringParams &= "&" & "oauth_signature_method=HMAC-SHA1"
            SigBaseStringParams &= "&" & "oauth_timestamp=" & Math.Round(SinceEpoch.TotalSeconds)
            SigBaseStringParams &= "&" & "oauth_version=1.0"
            Dim SigBaseString As String = "GET&"
            SigBaseString &= Uri.EscapeDataString(FlickrUrl) & "&" & Uri.EscapeDataString(SigBaseStringParams)

            Dim KeyMaterial As IBuffer = CryptographicBuffer.ConvertStringToBinary(FlickrClientSecret.Text & "&", BinaryStringEncoding.Utf8)
            Dim HmacSha1Provider As MacAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1")
            Dim MacKey As CryptographicKey = HmacSha1Provider.CreateKey(KeyMaterial)
            Dim DataToBeSigned As IBuffer = CryptographicBuffer.ConvertStringToBinary(SigBaseString, BinaryStringEncoding.Utf8)
            Dim SignatureBuffer As IBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned)
            Dim Signature As String = CryptographicBuffer.EncodeToBase64String(SignatureBuffer)

            FlickrUrl &= "?" & SigBaseStringParams & "&oauth_signature=" & Uri.EscapeDataString(Signature)
            Dim GetResponse As String = Await SendDataAsync(FlickrUrl)
            DebugPrint("Received Data: " & GetResponse)


            If GetResponse IsNot Nothing Then
                Dim oauth_token As String = Nothing
                Dim oauth_token_secret As String = Nothing
                Dim keyValPairs() As String = GetResponse.Split("&"c)

                For i As Integer = 0 To keyValPairs.Length - 1
                    Dim splits() As String = keyValPairs(i).Split("="c)
                    Select Case splits(0)
                        Case "oauth_token"
                            oauth_token = splits(1)
                        Case "oauth_token_secret"
                            oauth_token_secret = splits(1)
                    End Select
                Next i

                If oauth_token IsNot Nothing Then

                    FlickrUrl = "https://secure.flickr.com/services/oauth/authorize?oauth_token=" & oauth_token & "&perms=read"
                    Dim StartUri As System.Uri = New Uri(FlickrUrl)
                    Dim EndUri As System.Uri = New Uri(FlickrCallbackUrl.Text)

                    DebugPrint("Navigating to: " & FlickrUrl)

                    Dim WebAuthenticationResult As WebAuthenticationResult = Await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri)
                    If WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.Success Then
                        OutputToken(WebAuthenticationResult.ResponseData.ToString())
                    ElseIf WebAuthenticationResult.ResponseStatus = WebAuthenticationStatus.ErrorHttp Then
                        OutputToken("HTTP Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseErrorDetail.ToString())
                    Else
                        OutputToken("Error returned by AuthenticateAsync() : " & WebAuthenticationResult.ResponseStatus.ToString())
                    End If
                End If
            End If
        Catch [Error] As Exception
            '
            ' Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString())
        End Try
    End Sub

End Class

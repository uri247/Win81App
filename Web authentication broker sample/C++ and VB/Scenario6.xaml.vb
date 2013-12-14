'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System
Imports Windows.Web.Http
Imports Windows.Web.Http.Filters

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private autoPickerHttpClient As HttpClient = Nothing
    Private autoPicker As AuthFilters.SwitchableAuthFilter

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

    Private Function GetAutoPickerHttpClient(ByVal clientId As String) As HttpClient
        If autoPickerHttpClient Is Nothing Then
            Dim bpf = New HttpBaseProtocolFilter()
            autoPicker = New AuthFilters.SwitchableAuthFilter(bpf)
            'You can add multiple fiters (twitter, google etc) if you are connecting to more than one service.
            autoPicker.AddOAuth2Filter(MakeFacebook(clientId, bpf))
            autoPickerHttpClient = New HttpClient(autoPicker)
        End If
        Return autoPickerHttpClient
    End Function

    Private Async Sub Launch_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If FacebookClientID.Text = "" Then
            rootPage.NotifyUser("Please enter an Client ID.", NotifyType.StatusMessage)
            Return
        End If

        Dim uri = New Uri("https://graph.facebook.com/me")
        Dim httpClient As HttpClient = GetAutoPickerHttpClient(FacebookClientID.Text)

        DebugPrint("Getting data from facebook....")
        Dim request = New HttpRequestMessage(HttpMethod.Get, uri)
        Try
            Dim response = Await httpClient.SendRequestAsync(request)
            If response.IsSuccessStatusCode Then
                Dim userInfo As String = Await response.Content.ReadAsStringAsync()
                DebugPrint(userInfo)
            Else
                Dim str As String = ""
                If response.Content IsNot Nothing Then
                    str = Await response.Content.ReadAsStringAsync()
                End If
                DebugPrint("ERROR: " & response.StatusCode & " " & response.ReasonPhrase & vbCrLf & str)
            End If
        Catch ex As Exception
            DebugPrint("EXCEPTION: " & ex.Message)
        End Try
    End Sub


    Private Sub Clear_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If autoPicker IsNot Nothing Then
            autoPicker.ClearAll()
        End If
    End Sub


    Public Shared Function MakeFacebook(ByVal clientId As String, ByVal innerFilter As IHttpFilter) As AuthFilters.OAuth2Filter
        Dim f = New AuthFilters.OAuth2Filter(innerFilter)
        Dim config = New AuthFilters.AuthConfigurationData()
        config.ClientId = clientId

        config.TechnicalName = "facebook.com"
        config.ApiUriPrefix = "https://graph.facebook.com/"
        config.SampleUri = "https://graph.facebook.com/me"
        config.RedirectUri = "https://www.facebook.com/connect/login_success.html"
        config.ClientSecret = ""
        config.Scope = "read_stream"
        config.Display = "popup"
        config.State = ""
        config.AdditionalParameterName = ""
        config.AdditionalParameterValue = ""
        config.ResponseType = "" ' blank==default "token". null doesn't marshall.
        config.AccessTokenLocation = "" ' blank=default "query";
        config.AccessTokenQueryParameterName = "" ' blank=default "access_token";
        config.AuthorizationUri = "https://www.facebook.com/dialog/oauth"
        config.AuthorizationCodeToTokenUri = ""
        f.AuthConfiguration = config

        Return f
    End Function

    Private Sub DebugPrint(ByVal Trace As String)
        Dim FacebookDebugArea As TextBox = TryCast(rootPage.FindName("FacebookDebugArea"), TextBox)
        FacebookDebugArea.Text += Trace & vbCrLf
    End Sub

    Private Sub OutputToken(ByVal TokenUri As String)
        Dim FacebookReturnedToken As TextBox = TryCast(rootPage.FindName("FacebookReturnedToken"), TextBox)
        FacebookReturnedToken.Text = TokenUri
    End Sub


End Class

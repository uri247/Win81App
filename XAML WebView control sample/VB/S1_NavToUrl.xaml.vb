'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler address.KeyUp, AddressOf address_KeyUp
        AddHandler webView1.NavigationStarting, AddressOf webView1_NavigationStarting
        AddHandler webView1.ContentLoading, AddressOf webView1_ContentLoading
        AddHandler webView1.DOMContentLoaded, AddressOf webView1_DOMContentLoaded
        AddHandler webView1.UnviewableContentIdentified, AddressOf webView1_UnviewableContentIdentified
        AddHandler webView1.NavigationCompleted, AddressOf webView1_NavigationCompleted
    End Sub

    ''' <summary>
    ''' Invoked when this xaml page is about to be displayed in a Frame.
    ''' Note: This event is not related to the webview navigation.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        address.Text = "http://www.microsoft.com"
        'NavigateWebview("http://www.microsoft.com");
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Navigation' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub goButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Not pageIsLoading Then
            NavigateWebview(address.Text)
        Else
            webView1.Stop()
            pageIsLoading = False
        End If
    End Sub

    ''' <summary>
    ''' This handles the enter key in the url address box
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub address_KeyUp(ByVal sender As Object, ByVal e As KeyRoutedEventArgs)
        If e.Key = Windows.System.VirtualKey.Enter Then
            NavigateWebview(address.Text)
        End If
    End Sub

    ''' <summary>
    ''' Helper to perform the navigation in webview
    ''' </summary>
    ''' <param name="url"></param>
    Private Sub NavigateWebview(ByVal url As String)
        Try
            Dim targetUri As New Uri(url)
            webView1.Navigate(targetUri)
        Catch myE As FormatException
            ' Bad address
            webView1.NavigateToString(String.Format("<h1>Address is invalid, try again.  Details --> {0}.</h1>", myE.Message))
        End Try
    End Sub

    ''' <summary>
    ''' Property to control the "Go" button text, forward/backward buttons and progress ring
    ''' </summary>
    Private _pageIsLoading As Boolean
    Private Property pageIsLoading() As Boolean
        Get
            Return _pageIsLoading
        End Get
        Set(ByVal value As Boolean)
            _pageIsLoading = value
            goButton.Content = (If(value, "Stop", "Go"))
            progressRing1.Visibility = (If(value, Visibility.Visible, Visibility.Collapsed))

            If Not value Then
                navigateBack.IsEnabled = webView1.CanGoBack
                navigateForward.IsEnabled = webView1.CanGoForward
            End If
        End Set
    End Property

    ''' <summary>
    ''' Event to indicate webview is starting a navigation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub webView1_NavigationStarting(ByVal sender As WebView, ByVal args As WebViewNavigationStartingEventArgs)
        Dim url As String = ""
        Try
            url = args.Uri.ToString()
        Finally
            address.Text = url
            appendLog(String.Format("Starting navigation to: ""{0}""." & vbLf, url))
            pageIsLoading = True
        End Try
    End Sub

    ''' <summary>
    ''' Event is fired by webview when the content is not a webpage, such as a file download
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub webView1_UnviewableContentIdentified(ByVal sender As WebView, ByVal args As WebViewUnviewableContentIdentifiedEventArgs)
        appendLog(String.Format("Content for ""{0}"" cannot be loaded into webview. Invoking the default launcher instead." & vbLf, args.Uri.ToString()))
        ' We turn around and hand the Uri to the system launcher to launch the default handler for it
        Dim b As Windows.Foundation.IAsyncOperation(Of Boolean) = Windows.System.Launcher.LaunchUriAsync(args.Uri)
        pageIsLoading = False
    End Sub

    ''' <summary>
    ''' Event to indicate webview has resolved the uri, and that it is loading html content
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub webView1_ContentLoading(ByVal sender As WebView, ByVal args As WebViewContentLoadingEventArgs)
        Dim url As String = If(args.Uri IsNot Nothing, args.Uri.ToString(), "<null>")
        appendLog(String.Format("Loading content for ""{0}""." & vbLf, url))
    End Sub

    ''' <summary>
    ''' Event to indicate that the content is fully loaded in the webview. If you need to invoke script, it is best to wait for this event.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub webView1_DOMContentLoaded(ByVal sender As WebView, ByVal args As WebViewDOMContentLoadedEventArgs)
        Dim url As String = If(args.Uri IsNot Nothing, args.Uri.ToString(), "<null>")
        appendLog(String.Format("Content for ""{0}"" has finished loading." & vbLf, url))
    End Sub

    ''' <summary>
    ''' Event to indicate webview has completed the navigation, either with success or failure.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub webView1_NavigationCompleted(ByVal sender As WebView, ByVal args As WebViewNavigationCompletedEventArgs)
        pageIsLoading = False
        If args.IsSuccess Then
            Dim url As String = If(args.Uri IsNot Nothing, args.Uri.ToString(), "<null>")
            appendLog(String.Format("Navigation to ""{0}""completed successfully." & vbLf, url))
        Else
            Dim url As String = ""
            Try
                url = args.Uri.ToString()
            Finally
                address.Text = url
                appendLog(String.Format("Navigation to: ""{0}"" failed with error code {1}." & vbLf, url, args.WebErrorStatus.ToString()))
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Helper for logging
    ''' </summary>
    ''' <param name="logEntry"></param>
    Private Sub appendLog(ByVal logEntry As String)
        Dim r As New Run()
        r.Text = logEntry
        Dim p As New Paragraph()
        p.Inlines.Add(r)
        logResults.Blocks.Add(p)
    End Sub

    ''' <summary>
    ''' Handler for the GoBack button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub navigateBackButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If webView1.CanGoBack Then
            webView1.GoBack()
        End If
    End Sub

    ''' <summary>
    ''' Handler for the GoForward button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub navigateForwardButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If webView1.CanGoForward Then
            webView1.GoForward()
        End If
    End Sub
End Class

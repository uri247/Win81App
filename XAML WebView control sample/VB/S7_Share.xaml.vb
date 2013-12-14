'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel.DataTransfer

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario7
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

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
        AddHandler webView7.NavigationCompleted, AddressOf webView7_NavigationCompleted
        AddHandler webView7.NavigationStarting, AddressOf webView7_NavigationStarting
        webView7.Navigate(New Uri("http://msdn.microsoft.com"))

        ' Register for the share event
        AddHandler DataTransferManager.GetForCurrentView().DataRequested, AddressOf dataTransferManager_DataRequested
    End Sub

    Private Sub webView7_NavigationStarting(ByVal sender As WebView, ByVal args As WebViewNavigationStartingEventArgs)
        ProgressRing1.IsActive = True
    End Sub

    Private Sub webView7_NavigationCompleted(ByVal sender As WebView, ByVal args As WebViewNavigationCompletedEventArgs)
        ProgressRing1.IsActive = False
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Share Content' button.
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Share_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Show the share UI
        DataTransferManager.ShowShareUI()
    End Sub

    ''' <summary>
    ''' Called when a share is instigated either through the charms bar or the button in the app
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Async Sub dataTransferManager_DataRequested(ByVal sender As DataTransferManager, ByVal args As DataRequestedEventArgs)
        Dim request As DataRequest = args.Request
        'We are going to use an async API to talk to the webview, so get a deferral for the results
        Dim deferral As DataRequestDeferral = args.Request.GetDeferral()
        Dim dp As DataPackage = Await webView7.CaptureSelectedContentToDataPackageAsync()

        If dp IsNot Nothing AndAlso dp.GetView().AvailableFormats.Count > 0 Then
            ' Webview has a selection, so we'll share its data package
            dp.Properties.Title = "This is the selection from the webview control"
            request.Data = dp
        Else
            ' No selection, so we'll share the url of the webview
            Dim myData As New DataPackage()
            myData.SetWebLink(webView7.Source)
            myData.Properties.Title = "This is the URI from the webview control"
            myData.Properties.Description = webView7.Source.ToString()
            request.Data = myData
        End If
        deferral.Complete()
    End Sub
End Class

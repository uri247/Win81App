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
Partial Public NotInheritable Class Scenario3
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
        ' Initialize the local state directory with content
        createHtmlFileInLocalState()
    End Sub

    ' Copies the file "html\html_example2.html" from this package's installed location to
    ' a new file "NavigateToState\test.html" in the local state folder.  When this is
    ' done, enables the 'Load HTML' button.
    Private Async Sub createHtmlFileInLocalState()
        Dim stateFolder As StorageFolder = Await ApplicationData.Current.LocalFolder.CreateFolderAsync("NavigateToState", CreationCollisionOption.OpenIfExists)
        Dim htmlFile As StorageFile = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("html\html_example2.html")

        Await htmlFile.CopyAsync(stateFolder, "test.html", NameCollisionOption.ReplaceExisting)
        loadFromLocalState.IsEnabled = True
    End Sub

    ''' <summary>
    ''' Navigates the webview to the application package
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub loadFromPackage_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim url As String = "ms-appx-web:///html/html_example2.html"
        webView2.Navigate(New Uri(url))
        webViewLabel.Text = String.Format("Webview: {0}", url)
    End Sub

    ''' <summary>
    ''' Navigates the webview to the local state directory
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub loadFromLocalState_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim url As String = "ms-appdata:///local/NavigateToState/test.html"
        webView2.Navigate(New Uri(url))
        webViewLabel.Text = String.Format("Webview: {0}", url)
    End Sub
End Class




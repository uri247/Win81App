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
Partial Public NotInheritable Class Scenario2
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this xaml page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Async Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        'Using the storage classes to read the content from a file
        Dim f As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(New Uri("ms-appx:///html/html_example.html"))
        Dim htmlFragment As String = Await FileIO.ReadTextAsync(f)

        ' This is now a string so we can manipluate it before we use it.
        htmlFragment = htmlFragment.Replace("</body>", "    <p>This content will be handed to webview when you click the button.</p>" & vbLf & "  </body>")
        ' Put the string into the textbox
        HTML2.Text = htmlFragment
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Load' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Load_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Grab the HTML from the text box and load it into the WebView
        WebView2.NavigateToString(HTML2.Text)
    End Sub
End Class

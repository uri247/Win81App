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
Partial Public NotInheritable Class Scenario5
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
        ' Let's create an HTML fragment that contains some javascript code that we will invoke using
        ' InvokeScriptAsync().
        Dim htmlFragment As String = "" & vbCrLf & "<html>" & vbCrLf & "    <head>" & vbCrLf & "        <script type='text/javascript'>" & vbCrLf & "            function doSomething() " & vbCrLf & "            { " & vbCrLf & "                document.getElementById('myDiv').innerText = 'GoodBye';" & vbCrLf & "                return 'Hello World!'; " & vbCrLf & "            }" & vbCrLf & "        </script>" & vbCrLf & "    </head>" & vbCrLf & "    <body>" & vbCrLf & "        <div id='myDiv'>Hello</div>" & vbCrLf & "     </body>" & vbCrLf & "</html>"

        ' Load it into the HTML text box so it will be visible.
        HTML3.Text = htmlFragment
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Load' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub load_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Grab the HTML from the text box and load it into the WebView
        webView5.NavigateToString(HTML3.Text)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Script' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub script_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Invoke the javascript function called 'SayGoodbye' that is loaded into the WebView.
        Dim s As String = Await webView5.InvokeScriptAsync("doSomething", Nothing)
    End Sub
End Class

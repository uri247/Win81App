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
Partial Public NotInheritable Class Scenario6
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Dim src As String = "ms-appx-web:///html/scriptNotify_example.html"
        webViewLabel.Text = String.Format("Webview: {0}", src)
        webView6.Navigate(New Uri(src))
        AddHandler webView6.ScriptNotify, AddressOf webView6_ScriptNotify
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub


    Private Sub webView6_ScriptNotify(ByVal sender As Object, ByVal e As NotifyEventArgs)
        ' Be sure to verify the source of the message when performing actions with the data.
        ' As webview can be navigated, you need to check that the message is coming from a page/code
        ' that you trust.
        Dim c As Color = Colors.Red

        If e.CallingUri.Scheme = "ms-appx-web" Then
            If e.Value.ToLower() = "blue" Then
                c = Colors.Blue
            ElseIf e.Value.ToLower() = "green" Then
                c = Colors.Green
            End If
        End If
        appendLog(String.Format("Response from script at '{0}': '{1}'", e.CallingUri, e.Value), c)
    End Sub

    ''' <summary>
    ''' Helper to create log entries
    ''' </summary>
    ''' <param name="logEntry"></param>
    Private Sub appendLog(ByVal logEntry As String, ByVal c As Color)
        Dim r As New Run()
        r.Text = logEntry
        Dim p As New Paragraph()
        p.Foreground = New SolidColorBrush(c)
        p.Inlines.Add(r)
        logResults.Blocks.Add(p)
    End Sub

End Class

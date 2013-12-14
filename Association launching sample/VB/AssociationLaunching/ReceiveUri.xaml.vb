'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ReceiveUri
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page. This is needed if you want to call methods in MainPage such as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached. The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Display the result of the protocol activation if we got here as a result of being activated for a protocol.
        If rootPage.ProtocolEvent IsNot Nothing Then
            rootPage.NotifyUser("Protocol activation received. The received URI is " & rootPage.ProtocolEvent.Uri.AbsoluteUri & ".", NotifyType.StatusMessage)
        End If
    End Sub
End Class

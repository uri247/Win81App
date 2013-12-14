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
Partial Public NotInheritable Class Scenario11
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

    Private Sub DeleteCookie_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            Dim cookie As New HttpCookie(NameField.Text, DomainField.Text, PathField.Text)

            Dim filter As New HttpBaseProtocolFilter()
            filter.CookieManager.DeleteCookie(cookie)

            rootPage.NotifyUser("Cookie deleted.", NotifyType.StatusMessage)
        Catch ex As ArgumentException
            rootPage.NotifyUser(ex.Message, NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser("Error: " & ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub
End Class

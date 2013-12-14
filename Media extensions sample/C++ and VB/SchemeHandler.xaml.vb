'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SchemeHandler
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
        rootPage.ExtensionManager.RegisterSchemeHandler("GeometricSource.GeometricSchemeHandler", "myscheme:")

        AddHandler Video.MediaFailed, AddressOf rootPage.VideoOnError
    End Sub

    ''' <summary>
    ''' Called when a page is no longer the active page in a frame. 
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler Video.MediaFailed, AddressOf rootPage.VideoOnError
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Circle' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Circle_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.Source = New Uri("myscheme://circle")
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Square' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Square_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.Source = New Uri("myscheme://square")
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Triangle' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Triangle_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.Source = New Uri("myscheme://triangle")
    End Sub
End Class

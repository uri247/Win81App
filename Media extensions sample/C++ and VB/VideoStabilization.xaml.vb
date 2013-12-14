'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class VideoStabilization
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
        AddHandler Video.MediaFailed, AddressOf rootPage.VideoOnError
        AddHandler VideoStabilized.MediaFailed, AddressOf rootPage.VideoOnError
    End Sub

    ''' <summary>
    ''' Called when a page is no longer the active page in a frame. 
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler Video.MediaFailed, AddressOf rootPage.VideoOnError
        RemoveHandler VideoStabilized.MediaFailed, AddressOf rootPage.VideoOnError
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Open' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Open_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        VideoStabilized.RemoveAllEffects()
        VideoStabilized.AddVideoEffect(Windows.Media.VideoEffects.VideoStabilization, True, Nothing)
        rootPage.PickSingleFileAndSet(New String() {".mp4", ".wmv", ".avi"}, Video, VideoStabilized)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Stop' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Stop_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.Source = Nothing
        VideoStabilized.Source = Nothing
    End Sub
End Class


'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class CustomDecoder
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private ReadOnly MFVideoFormat_MPG1 As Guid = Guid.Parse("3147504d-0000-0010-8000-00aa00389b71")

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
        rootPage.ExtensionManager.RegisterByteStreamHandler("MPEG1Source.MPEG1ByteStreamHandler", ".mpg", "video/mpeg")
        rootPage.ExtensionManager.RegisterVideoDecoder("MPEG1Decoder.MPEG1Decoder", MFVideoFormat_MPG1, Guid.Empty)

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
    ''' This is the click handler for the 'Open' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Open_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.PickSingleFileAndSet(New String() {".mpg"}, Video)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Stop' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Stop_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.Source = Nothing
    End Sub
End Class


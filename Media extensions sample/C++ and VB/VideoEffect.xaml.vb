'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class VideoEffect
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
    End Sub

    ''' <summary>
    ''' Called when a page is no longer the active page in a frame. 
    ''' </summary>
    ''' <param name="e"></param>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler Video.MediaFailed, AddressOf rootPage.VideoOnError
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'OpenGrayscale' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenGrayscale_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.RemoveAllEffects()
        Video.AddVideoEffect("GrayscaleTransform.GrayscaleEffect", True, Nothing)

        rootPage.PickSingleFileAndSet(New String() {".mp4", ".wmv", ".avi"}, Video)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'OpenFisheye' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenFisheye_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        OpenVideoWithPolarEffect("Fisheye")
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'OpenPinch' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenPinch_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        OpenVideoWithPolarEffect("Pinch")
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'OpenWarp' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenWarp_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        OpenVideoWithPolarEffect("Warp")
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'OpenInvert' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenInvert_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.RemoveAllEffects()
        Video.AddVideoEffect("InvertTransform.InvertEffect", True, Nothing)

        rootPage.PickSingleFileAndSet(New String() {".mp4", ".wmv", ".avi"}, Video)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Stop' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Stop_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Video.Source = Nothing
    End Sub

    Private Sub OpenVideoWithPolarEffect(ByVal effectName As String)
        Video.RemoveAllEffects()
        Dim configuration As New PropertySet()
        configuration.Add("effect", effectName)
        Video.AddVideoEffect("PolarTransform.PolarEffect", True, configuration)

        rootPage.PickSingleFileAndSet(New String() {".mp4", ".wmv", ".avi"}, Video)
    End Sub
End Class


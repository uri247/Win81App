'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class LaunchUri
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
    End Sub

    ''' <summary>
    ' Launch a URI.
    ''' </summary>
    Private Async Sub LaunchUriButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create the URI to launch from a string.
        Dim uri = New Uri(UriToLaunch.Text)

        ' Launch the URI.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri)
        If success Then
            rootPage.NotifyUser("URI launched: " & uri.AbsoluteUri, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' Launch a URI. Show a warning prompt.
    ''' </summary>
    Private Async Sub LaunchUriWithWarningButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create the URI to launch from a string.
        Dim uri = New Uri(UriToLaunch.Text)

        ' Configure the warning prompt.
        Dim options = New Windows.System.LauncherOptions()
        options.TreatAsUntrusted = True

        ' Launch the URI.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri, options)
        If success Then
            rootPage.NotifyUser("URI launched: " & uri.AbsoluteUri, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' Launch a URI. Show an Open With dialog that lets the user chose the handler to use.
    ''' </summary>
    Private Async Sub LaunchUriOpenWithButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create the URI to launch from a string.
        Dim uri = New Uri(UriToLaunch.Text)

        ' Calulcate the position for the Open With dialog.
        ' An alternative to using the point is to set the rect of the UI element that triggered the launch.
        Dim openWithPosition As Point = GetOpenWithPosition(LaunchUriOpenWithButton)

        ' Next, configure the Open With dialog.
        Dim options = New Windows.System.LauncherOptions()
        options.DisplayApplicationPicker = True
        options.UI.InvocationPoint = openWithPosition
        options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below

        ' Launch the URI.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri, options)
        If success Then
            rootPage.NotifyUser("URI launched: " & uri.AbsoluteUri, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' Launch a URI. Request to share the screen with the launched app.
    ''' </summary>
    Private Async Sub LaunchUriSplitScreenButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create the URI to launch from a string.
        Dim uri = New Uri(UriToLaunch.Text)

        ' Configure the request for split screen launch.
        Dim options = New Windows.System.LauncherOptions()
        If DefaultItem.IsSelected = True Then
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.Default
        ElseIf UseLess.IsSelected = True Then
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseLess
        ElseIf UseHalf.IsSelected = True Then
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseHalf
        ElseIf UseMore.IsSelected = True Then
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMore
        ElseIf UseMinimum.IsSelected = True Then
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMinimum
        ElseIf UseNone.IsSelected = True Then
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseNone
        End If

        ' Launch the URI.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchUriAsync(uri, options)
        If success Then
            rootPage.NotifyUser("URI launched: " & uri.AbsoluteUri, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' The Open With dialog should be displayed just under the element that triggered it.
    ''' </summary>
    Private Function GetOpenWithPosition(ByVal element As FrameworkElement) As Windows.Foundation.Point
        Dim buttonTransform As Windows.UI.Xaml.Media.GeneralTransform = element.TransformToVisual(Nothing)

        Dim desiredLocation As Point = buttonTransform.TransformPoint(New Point())
        desiredLocation.Y = desiredLocation.Y + element.ActualHeight

        Return desiredLocation
    End Function
End Class

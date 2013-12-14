'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class LaunchFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page. This is needed if you want to call methods in MainPage such as NotifyUser().
    Private rootPage As MainPage = MainPage.Current
    Private fileToLaunch As String = "Assets\Icon.Targetsize-256.png"

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
    ' Launch a .png file that came with the package.
    ''' </summary>
    Private Async Sub LaunchFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' First, get the image file from the package's image directory.
        Dim file = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch)

        ' Next, launch the file.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file)
        If success Then
            rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' Launch a .png file that came with the package. Show a warning prompt.
    ''' </summary>
    Private Async Sub LaunchFileWithWarningButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' First, get the image file from the package's image directory.
        Dim file = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch)

        ' Next, configure the warning prompt.
        Dim options = New Windows.System.LauncherOptions()
        options.TreatAsUntrusted = True

        ' Finally, launch the file.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file, options)
        If success Then
            rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' Launch a .png file that came with the package. Show an Open With dialog that lets the user chose the handler to use.
    ''' </summary>
    Private Async Sub LaunchFileOpenWithButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' First, get the image file from the package's image directory.
        Dim file = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch)

        ' Calculate the position for the Open With dialog.
        ' An alternative to using the point is to set the rect of the UI element that triggered the launch.
        Dim openWithPosition As Point = GetOpenWithPosition(LaunchFileOpenWithButton)

        ' Next, configure the Open With dialog.
        Dim options = New Windows.System.LauncherOptions()
        options.DisplayApplicationPicker = True
        options.UI.InvocationPoint = openWithPosition
        options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below

        ' Finally, launch the file.
        Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file, options)
        If success Then
            rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' Launch a .png file that came with the package. Request to share the screen with the launched app.
    ''' </summary>
    Private Async Sub LaunchFileSplitScreenButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' First, get a file via the picker.
        Dim openPicker = New Windows.Storage.Pickers.FileOpenPicker()
        openPicker.FileTypeFilter.Add("*")

        Dim file As Windows.Storage.StorageFile = Await openPicker.PickSingleFileAsync()
        If file IsNot Nothing Then
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

            ' Next, launch the file.
            Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file, options)
            If success Then
                rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
            End If
        Else
            rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage)
        End If
    End Sub

    ''' <summary>
    ' Have the user pick a file, then launch it.
    ''' </summary>
    Private Async Sub PickAndLaunchFileButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' First, get a file via the picker.
        Dim openPicker = New Windows.Storage.Pickers.FileOpenPicker()
        openPicker.FileTypeFilter.Add("*")

        Dim file As Windows.Storage.StorageFile = Await openPicker.PickSingleFileAsync()
        If file IsNot Nothing Then
            ' Next, launch the file.
            Dim success As Boolean = Await Windows.System.Launcher.LaunchFileAsync(file)
            If success Then
                rootPage.NotifyUser("File launched: " & file.Name, NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage)
            End If
        Else
            rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage)
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

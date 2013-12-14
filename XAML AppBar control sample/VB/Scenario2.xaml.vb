'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits Windows.UI.Xaml.Controls.Page

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private leftPanel As StackPanel = Nothing
    Private rightPanel As StackPanel = Nothing
    Private originalBackgroundBrush As Brush = Nothing
    Private originalSeparatorBrush As Brush = Nothing
    Private originalButtonStyle As Style = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Save off original background
        originalBackgroundBrush = rootPage.BottomAppBar.Background

        ' Set the new AppBar Background
        rootPage.BottomAppBar.Background = New SolidColorBrush(Color.FromArgb(255, 20, 20, 90))

        ' Find our stack panels that contain our AppBarButtons

        leftPanel = TryCast(rootPage.FindName("LeftPanel"), StackPanel)
        rightPanel = TryCast(rootPage.FindName("RightPanel"), StackPanel)

        ' Change the color of all AppBarButtons in each panel
        ColorButtons(leftPanel)
        ColorButtons(rightPanel)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        rootPage.BottomAppBar.Background = originalBackgroundBrush
        RestoreButtons(leftPanel)
        RestoreButtons(rightPanel)
    End Sub

    ''' <summary>
    ''' This method will change the style of each AppBarButton to use a green foreground color
    ''' </summary>
    ''' <param name="panel"></param>
    Private Sub ColorButtons(ByVal panel As StackPanel)
        Dim count As Integer = 0
        For Each item In panel.Children
            ' For AppBarButton, change the style
            If item.GetType() Is GetType(AppBarButton) Then
                If count = 0 Then
                    originalButtonStyle = CType(item, AppBarButton).Style
                End If
                CType(item, AppBarButton).Style = TryCast(rootPage.Resources("GreenAppBarButtonStyle"), Style)
                count += 1
            Else
                ' For AppBarSeparator(s), just change the foreground color
                If item.GetType() Is GetType(AppBarSeparator) Then
                    originalSeparatorBrush = CType(item, AppBarSeparator).Foreground
                    CType(item, AppBarSeparator).Foreground = New SolidColorBrush(Color.FromArgb(255, 90, 200, 90))
                End If
            End If
        Next item
    End Sub

    ''' <summary>
    ''' This method will restore the style of each AppBarButton
    ''' </summary>
    ''' <param name="panel"></param>
    Private Sub RestoreButtons(ByVal panel As StackPanel)
        For Each item In panel.Children
            ' For AppBarButton, change the style
            If item.GetType() Is GetType(AppBarButton) Then
                CType(item, AppBarButton).Style = originalButtonStyle
            Else
                ' For AppBarSeparator(s), just change the foreground color
                If item.GetType() Is GetType(AppBarSeparator) Then
                    CType(item, AppBarSeparator).Foreground = originalSeparatorBrush
                End If
            End If
        Next item
    End Sub
End Class

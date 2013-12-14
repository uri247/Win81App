'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario6
    Inherits Windows.UI.Xaml.Controls.Page

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private leftPanel As StackPanel = Nothing
    Private rightPanel As StackPanel = Nothing
    Private commands As New List(Of UIElement)()

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        AddHandler rootPage.BottomAppBar.Opened, AddressOf BottomAppBar_Opened
        AddHandler rootPage.BottomAppBar.Closed, AddressOf BottomAppBar_Closed

        leftPanel = TryCast(rootPage.FindName("LeftPanel"), StackPanel)
        rightPanel = TryCast(rootPage.FindName("RightPanel"), StackPanel)

        ShowAppBar.IsEnabled = True
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        rootPage.BottomAppBar.IsOpen = False
        rootPage.BottomAppBar.IsSticky = False
        ShowAppBarButtons()
    End Sub

    Private Sub BottomAppBar_Closed(ByVal sender As Object, ByVal e As Object)
        ShowAppBar.IsEnabled = True
        HideCommands.IsEnabled = False
    End Sub

    Private Sub BottomAppBar_Opened(ByVal sender As Object, ByVal e As Object)
        ShowAppBar.IsEnabled = False

        Dim ab As AppBar = TryCast(sender, AppBar)
        If ab IsNot Nothing Then
            ab.IsSticky = True
            If leftPanel.Children.Count > 0 AndAlso rightPanel.Children.Count > 0 Then
                HideCommands.IsEnabled = True
            End If
        End If
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Show AppBar' button.  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowAppBarClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.BottomAppBar.IsOpen = True
        HideCommands.IsEnabled = True
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Show Commands' button.  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowCommands_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        HideCommands.IsEnabled = True
        ShowCommands.IsEnabled = False
        ShowAppBarButtons()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Hide Commands' button.  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub HideCommands_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            b.IsEnabled = False
            ShowCommands.IsEnabled = True
            commands.Clear()
            HideAppBarButtons(rightPanel)
        End If

    End Sub

    Private Sub HideAppBarButtons(ByVal panel As StackPanel)
        For Each item In panel.Children
            commands.Add(item)
        Next item
        panel.Children.Clear()
    End Sub

    Private Sub ShowAppBarButtons()
        For Each item In commands
            rightPanel.Children.Add(item)
        Next item
        commands.Clear()
    End Sub
End Class
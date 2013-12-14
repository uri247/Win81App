'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits Windows.UI.Xaml.Controls.Page

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private leftPanel As StackPanel
    Private leftItems As List(Of UIElement)

    Public Sub New()
        Me.InitializeComponent()
        leftItems = New List(Of UIElement)()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Add some custom (non-AppBarButton) content
        leftPanel = TryCast(rootPage.FindName("LeftPanel"), StackPanel)

        For Each element As UIElement In leftPanel.Children
            leftItems.Add(element)
        Next element
        leftPanel.Children.Clear()

        ' Create a combo box
        Dim cb As New ComboBox()
        cb.Height = 32.0
        cb.Width = 100.0
        cb.Items.Add("Baked")
        cb.Items.Add("Fried")
        cb.Items.Add("Frozen")
        cb.Items.Add("Chilled")

        cb.SelectedIndex = 0

        leftPanel.Children.Add(cb)

        ' Create a text box
        Dim tb As New TextBox()
        tb.Text = "Search for desserts."
        tb.Width = 300.0
        tb.Height = 30.0
        tb.Margin = New Thickness(10.0, 0.0, 0.0, 0.0)
        AddHandler tb.GotFocus, AddressOf tb_GotFocus

        leftPanel.Children.Add(tb)

        ' Add a button
        Dim b As New Button()
        b.Content = "Search"
        AddHandler b.Click, AddressOf b_Click

        leftPanel.Children.Add(b)
    End Sub

    Private Sub b_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.NotifyUser("Search button pressed", NotifyType.StatusMessage)
    End Sub

    Private Sub tb_GotFocus(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim tb As TextBox = TryCast(sender, TextBox)
        tb.Text = String.Empty
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        leftPanel.Children.Clear()
        For Each element As UIElement In leftItems
            leftPanel.Children.Add(element)
        Next element

    End Sub
End Class
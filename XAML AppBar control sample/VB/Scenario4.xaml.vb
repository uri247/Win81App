'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits Windows.UI.Xaml.Controls.Page

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private originalAppBar As AppBar = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Save original AppBar so we can restore it afterward
        originalAppBar = rootPage.BottomAppBar

        ' Use a CommandBar rather than an AppBar so that we get default layout
        Dim commandBar As New CommandBar()

        ' Create the 'Add' button
        Dim add As New AppBarButton()
        add.Label = "Add"
        add.Icon = New SymbolIcon(Symbol.Add)

        commandBar.PrimaryCommands.Add(add)

        ' Create the 'Remove' button
        Dim remove As New AppBarButton()
        remove.Label = "Remove"
        remove.Icon = New SymbolIcon(Symbol.Remove)

        commandBar.PrimaryCommands.Add(remove)

        commandBar.PrimaryCommands.Add(New AppBarSeparator())

        ' Create the 'Delete' button
        Dim delete As New AppBarButton()
        delete.Label = "Delete"
        delete.Icon = New SymbolIcon(Symbol.Delete)

        commandBar.PrimaryCommands.Add(delete)

        ' Create the 'Camera' button
        Dim camera As New AppBarButton()
        camera.Label = "Camera"
        camera.Icon = New SymbolIcon(Symbol.Camera)
        commandBar.SecondaryCommands.Add(camera)

        ' Create the 'Bold' button
        Dim bold As New AppBarButton()
        bold.Label = "Bold"
        bold.Icon = New SymbolIcon(Symbol.Bold)
        commandBar.SecondaryCommands.Add(bold)

        ' Create the 'Italic' button
        Dim italic As New AppBarButton()
        italic.Label = "Italic"
        italic.Icon = New SymbolIcon(Symbol.Italic)
        commandBar.SecondaryCommands.Add(italic)

        ' Create the 'Underline' button
        Dim underline As New AppBarButton()
        underline.Label = "Underline"
        underline.Icon = New SymbolIcon(Symbol.Underline)
        commandBar.SecondaryCommands.Add(underline)

        ' Create the 'Align Left' button
        Dim left As New AppBarButton()
        left.Label = "Align Left"
        left.Icon = New SymbolIcon(Symbol.AlignLeft)
        commandBar.SecondaryCommands.Add(left)

        ' Create the 'Align Center' button
        Dim center As New AppBarButton()
        center.Label = "Align Center"
        center.Icon = New SymbolIcon(Symbol.AlignCenter)
        commandBar.SecondaryCommands.Add(center)

        ' Create the 'Align Right' button
        Dim right As New AppBarButton()
        right.Label = "Align Right"
        right.Icon = New SymbolIcon(Symbol.AlignRight)
        commandBar.SecondaryCommands.Add(right)

        rootPage.BottomAppBar = commandBar



    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        rootPage.BottomAppBar = originalAppBar
    End Sub
End Class

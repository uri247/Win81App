'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits Windows.UI.Xaml.Controls.Page

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private leftPanel As StackPanel
    Private rightPanel As StackPanel
    Private leftItems As List(Of UIElement)
    Private rightItems As List(Of UIElement)
    Public Sub New()
        Me.InitializeComponent()
        leftItems = New List(Of UIElement)()
        rightItems = New List(Of UIElement)()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Find the stack panels that host our AppBarButtons within the AppBar
        leftPanel = TryCast(rootPage.FindName("LeftPanel"), StackPanel)
        rightPanel = TryCast(rootPage.FindName("RightPanel"), StackPanel)

        CopyButtons(leftPanel, leftItems)
        CopyButtons(rightPanel, rightItems)

        ' Remove existing AppBarButtons
        leftPanel.Children.Clear()
        rightPanel.Children.Clear()

        ' Create the AppBarToggle button for the 'Shuffle' command
        Dim shuffle As New AppBarToggleButton()
        shuffle.Label = "Shuffle"
        shuffle.Icon = New SymbolIcon(Symbol.Shuffle)

        rightPanel.Children.Add(shuffle)

        ' Create the AppBarButton for the 'Sun' command
        Dim sun As New AppBarButton()
        sun.Label = "Sun"

        ' This button will use the FontIcon class for its icon which allows us to choose
        ' any glyph from any FontFamily
        Dim sunIcon As New FontIcon()
        sunIcon.FontFamily = New Windows.UI.Xaml.Media.FontFamily("Wingdings")
        sunIcon.FontSize = 30.0
        sunIcon.Glyph = ChrW(&H52)
        sun.Icon = sunIcon

        rightPanel.Children.Add(sun)

        ' Create the AppBarButton for the 'Triangle' command
        Dim triangle As New AppBarButton()
        triangle.Label = "Triangle"

        ' This button will use the PathIcon class for its icon which allows us to 
        ' use vector data to represent the icon
        Dim trianglePathIcon As New PathIcon()
        Dim g As New PathGeometry()
        g.FillRule = FillRule.Nonzero

        ' Just create a simple triange shape
        Dim f As New PathFigure()
        f.IsFilled = True
        f.IsClosed = True
        f.StartPoint = New Windows.Foundation.Point(20.0, 5.0)
        Dim s1 As New LineSegment()
        s1.Point = New Windows.Foundation.Point(30.0, 30.0)
        Dim s2 As New LineSegment()
        s2.Point = New Windows.Foundation.Point(10.0, 30.0)
        Dim s3 As New LineSegment()
        s3.Point = New Windows.Foundation.Point(20.0, 5.0)
        f.Segments.Add(s1)
        f.Segments.Add(s2)
        f.Segments.Add(s3)
        g.Figures.Add(f)

        trianglePathIcon.Data = g

        triangle.Icon = trianglePathIcon

        rightPanel.Children.Add(triangle)

        ' Create the AppBarButton for the 'Smiley' command
        Dim smiley As New AppBarButton()
        smiley.Label = "Smiley"
        smiley.Icon = New BitmapIcon With {.UriSource = New Uri("ms-appx:/Assets/smiley.png")}

        rightPanel.Children.Add(smiley)


    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        leftPanel.Children.Clear()
        rightPanel.Children.Clear()

        RestoreButtons(leftPanel, leftItems)
        RestoreButtons(rightPanel, rightItems)
    End Sub

    Private Sub CopyButtons(ByVal panel As StackPanel, ByVal list As List(Of UIElement))
        For Each element As UIElement In panel.Children
            list.Add(element)
        Next element
    End Sub

    Private Sub RestoreButtons(ByVal panel As StackPanel, ByVal list As List(Of UIElement))
        For Each element As UIElement In list
            panel.Children.Add(element)
        Next element
    End Sub
End Class

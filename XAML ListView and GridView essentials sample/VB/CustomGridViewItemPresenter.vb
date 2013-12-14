Imports System
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Shapes
Imports Windows.UI.Xaml.Media.Animation

Friend Class CustomGridViewItemPresenter
    Inherits ContentPresenter

    ' These are the only objects we need to show item's content and visuals for
    ' focus and pointer over state. This is a huge reduction in total elements 
    ' over the expanded GridViewItem template. Even better is that these objects
    ' are only instantiated when they are needed instead of at startup!
    Private _contentGrid As Grid = Nothing
    Private _pointerOverBorder As Rectangle = Nothing
    Private _focusVisual As Rectangle = Nothing

    Private _pointerDownAnimation As PointerDownThemeAnimation = Nothing
    Private _pointerDownStoryboard As Storyboard = Nothing

    Private _parentGridView As GridView

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Overrides Function GoToElementStateCore(ByVal stateName As String, ByVal useTransitions As Boolean) As Boolean
        MyBase.GoToElementStateCore(stateName, useTransitions)

        ' change the visuals shown based on the state the item is going to
        Select Case stateName
            Case "Normal"
                HidePointerOverVisuals()
                HideFocusVisuals()
                If useTransitions Then
                    StopPointerDownAnimation()
                End If

            Case "Focused", "PointerFocused"
                ShowFocusVisuals()

            Case "Unfocused"
                HideFocusVisuals()

            Case "PointerOver"
                ShowPointerOverVisuals()
                If useTransitions Then
                    StopPointerDownAnimation()
                End If

            Case "Pressed", "PointerOverPressed"
                If useTransitions Then
                    StartPointerDownAnimation()
                End If

                ' this sample does not deal with the DataAvailable, NotDragging, NoReorderHint, NoSelectionHint,
                ' Unselected, SelectedUnfocused, or UnselectedPointerOver states
            Case Else
        End Select

        Return True
    End Function

    Private Sub StartPointerDownAnimation()
        ' create the storyboard for the pointer down animation if it doesn't exist 
        If _pointerDownStoryboard Is Nothing Then
            CreatePointerDownStoryboard()
        End If

        ' start the storyboard for the pointer down animation 
        _pointerDownStoryboard.Begin()
    End Sub

    Private Sub StopPointerDownAnimation()
        ' stop the pointer down animation
        If _pointerDownStoryboard IsNot Nothing Then
            _pointerDownStoryboard.Stop()
        End If
    End Sub

    Private Sub ShowFocusVisuals()
        ' create the elements necessary to show focus visuals if they have
        ' not been created yet.       
        If Not FocusElementsAreCreated() Then
            CreateFocusElements()
        End If

        ' make sure the elements necessary to show focus visuals are opaque
        _focusVisual.Opacity = 1
    End Sub

    Private Sub HideFocusVisuals()
        ' hide the elements that visualize focus if they have been created
        If FocusElementsAreCreated() Then
            _focusVisual.Opacity = 0
        End If
    End Sub

    Private Sub ShowPointerOverVisuals()
        ' create the elements necessary to show pointer over visuals if they have
        ' not been created yet.       
        If Not PointerOverElementsAreCreated() Then
            CreatePointerOverElements()
        End If

        ' make sure the elements necessary to show pointer over visuals are opaque
        _pointerOverBorder.Opacity = 1
    End Sub

    Private Sub HidePointerOverVisuals()
        ' hide the elements that visualize pointer over if they have been created
        If PointerOverElementsAreCreated() Then
            _pointerOverBorder.Opacity = 0
        End If
    End Sub

    Private Sub CreatePointerDownStoryboard()
        _pointerDownAnimation = New PointerDownThemeAnimation()
        Storyboard.SetTarget(_pointerDownAnimation, _contentGrid)

        _pointerDownStoryboard = New Storyboard()
        _pointerDownStoryboard.Children.Add(_pointerDownAnimation)
    End Sub

    Private Sub CreatePointerOverElements()
        ' create the "border" which is really a Rectangle with the correct attributes
        _pointerOverBorder = New Rectangle()
        _pointerOverBorder.IsHitTestVisible = False
        _pointerOverBorder.Opacity = 0
        ' note that this uses a statically declared brush and will not respond to changes in high contrast
        _pointerOverBorder.Fill = CType(_parentGridView.Resources("PointerOverBrush"), SolidColorBrush)

        ' add the pointer over visuals on top of all children of _InnerDragContent
        _contentGrid.Children.Insert(_contentGrid.Children.Count, _pointerOverBorder)
    End Sub

    Private Sub CreateFocusElements()
        ' create the focus visual which is a Rectangle with the correct attributes
        _focusVisual = New Rectangle()
        _focusVisual.IsHitTestVisible = False
        _focusVisual.Opacity = 0
        _focusVisual.StrokeThickness = 2
        ' note that this uses a statically declared brush and will not respond to changes in high contrast
        _focusVisual.Stroke = CType(_parentGridView.Resources("FocusBrush"), SolidColorBrush)

        ' add the focus elements behind all children of _InnerDragContent
        _contentGrid.Children.Insert(0, _focusVisual)
    End Sub

    Private Function FocusElementsAreCreated() As Boolean
        Return _focusVisual IsNot Nothing
    End Function

    Private Function PointerOverElementsAreCreated() As Boolean
        Return _pointerOverBorder IsNot Nothing
    End Function

    Protected Overrides Sub OnApplyTemplate()
        ' call the base method
        MyBase.OnApplyTemplate()

        Dim obj = VisualTreeHelper.GetParent(Me)
        Do While Not (TypeOf obj Is GridView)
            obj = VisualTreeHelper.GetParent(obj)
        Loop
        _parentGridView = CType(obj, GridView)

        _contentGrid = CType(VisualTreeHelper.GetChild(Me, 0), Grid)
    End Sub
End Class

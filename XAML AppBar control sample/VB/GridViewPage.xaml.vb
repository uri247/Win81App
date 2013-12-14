Imports Windows.UI.Popups

' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class GridViewPage
    Inherits Page

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' This collection will contain our flavors of ice cream
    Private Flavors As ObservableCollection(Of IceCream)

    Private random As Random = Nothing

    Public Sub New()
        Me.InitializeComponent()

        Flavors = New ObservableCollection(Of IceCream)()
        random = New Random()
        AddHandler IceCreamList.SelectionChanged, AddressOf IceCreamList_SelectionChanged
    End Sub

    Private Sub IceCreamList_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim gv As GridView = TryCast(sender, GridView)
        If gv IsNot Nothing Then
            If gv.SelectedItem IsNot Nothing Then
                ' We have selected items so show the AppBar and make it sticky
                BottomAppBar.IsSticky = True
                BottomAppBar.IsOpen = True
            Else
                ' No selections so hide the AppBar and don't make it sticky any longer
                BottomAppBar.IsSticky = False
                BottomAppBar.IsOpen = False
            End If
        End If
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Populate our collection of ice cream flavors
        For i As Integer = 0 To 49
            Flavors.Add(GenerateItem())
        Next i

        IceCreamList.ItemsSource = Flavors
    End Sub

    ''' <summary>
    ''' This method will generate random ice cream flavors
    ''' </summary>
    ''' <returns></returns>
    Private Function GenerateItem() As IceCream

        Dim type As Integer = CInt(Math.Floor(CDbl(random.Next(1, 6))))

        Select Case type
            Case 1
                Return New IceCream With {.Name = "Banana Blast", .Type = "Low-fat Frozen Yogurt", .Image = "Assets/60Banana.png"}
            Case 2
                Return New IceCream With {.Name = "Lavish Lemon Ice", .Type = "Sorbet", .Image = "Assets/60Lemon.png"}
            Case 3
                Return New IceCream With {.Name = "Marvelous Mint", .Type = "Gelato", .Image = "Assets/60Mint.png"}
            Case 4
                Return New IceCream With {.Name = "Creamy Orange", .Type = "Sorbet", .Image = "Assets/60Orange.png"}
            Case 5
                Return New IceCream With {.Name = "Very Vanilla", .Type = "Ice Cream", .Image = "Assets/60Vanilla.png"}
            Case Else
                Return New IceCream With {.Name = "Succulent Strawberry", .Type = "Sorbet", .Image = "Assets/60Strawberry.png"}
        End Select
    End Function

    ''' <summary>
    ''' This is the click handler for the 'Back' button.  When clicked we want to go back to the main sample page
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Back_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.Frame.GoBack()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Select All' button.  When clicked we want to select all flavors in our list
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SelectAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        IceCreamList.SelectAll()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Clear' button.  When clicked we want to clear all selected items in our list
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Clear_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        IceCreamList.SelectedIndex = -1

    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Delete' button.  When clicked we want to delete all selected items in our list
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Delete_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim items As New List(Of IceCream)()
        For Each item As IceCream In IceCreamList.SelectedItems
            items.Add(item)
            'Flavors.Remove(item);
        Next item
        For Each item As IceCream In items
            Flavors.Remove(item)
        Next item
    End Sub

    ''' <summary>
    ''' This is the click handler for our 'Add' button.  It doesn't really do much of anything interesting :)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub Add_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim d As New MessageDialog("XAML AppBar Control Sample")
        d.Content = "Add button pressed"
        Await d.ShowAsync()
    End Sub
End Class

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage.Streams
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.Graphics.Imaging

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario8
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private bookmarks As New ObservableCollection(Of BookmarkItem)()

    Public Sub New()
        Me.InitializeComponent()
        Me.bookmarkList.ItemsSource = bookmarks
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        webView8.Navigate(New Uri("http://www.bing.com"))
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Solution' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub bookmarkBtn_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim ms As New InMemoryRandomAccessStream()
        Await webView8.CapturePreviewToStreamAsync(ms)

        'Create a small thumbnail
        Dim longlength As Integer = 180, width As Integer = 0, height As Integer = 0
        Dim srcwidth As Double = webView8.ActualWidth, srcheight As Double = webView8.ActualHeight
        Dim factor As Double = srcwidth / srcheight
        If factor < 1 Then
            height = longlength
            width = CInt(longlength * factor)
        Else
            width = longlength
            height = CInt(longlength / factor)
        End If
        Dim small As BitmapSource = Await resize(width, height, ms)

        Dim item As New BookmarkItem()
        item.Title = webView8.DocumentTitle
        item.PageUrl = webView8.Source
        item.Preview = small

        bookmarks.Add(item)
    End Sub


    Private Async Function resize(ByVal width As Integer, ByVal height As Integer, ByVal source As Windows.Storage.Streams.IRandomAccessStream) As Task(Of BitmapSource)
        Dim small As New WriteableBitmap(width, height)
        Dim decoder As BitmapDecoder = Await BitmapDecoder.CreateAsync(source)
        Dim transform As New BitmapTransform()
        transform.ScaledHeight = CUInt(height)
        transform.ScaledWidth = CUInt(width)
        Dim pixelData As PixelDataProvider = Await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage)
        pixelData.DetachPixelData().CopyTo(small.PixelBuffer)
        Return small
    End Function

    Private Sub bookmarkList_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
        Dim b As BookmarkItem = CType(e.ClickedItem, BookmarkItem)
        webView8.Navigate(b.PageUrl)
    End Sub

End Class


Friend Class BookmarkItem
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ' This method is called by the Set accessor of each property. 
    ' The CallerMemberName attribute that is applied to the optional propertyName 
    ' parameter causes the property name of the caller to be substituted as an argument. 
    Private Sub NotifyPropertyChanged(<CallerMemberName> Optional ByVal propertyName As String = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub


    Private _pageUrl As Uri
    Public Property PageUrl() As Uri
        Get
            Return Me._pageUrl
        End Get
        Set(ByVal value As Uri)
            _pageUrl = value
            NotifyPropertyChanged()
        End Set
    End Property

    Private _preview As BitmapSource
    Public Property Preview() As BitmapSource
        Get
            Return Me._preview
        End Get
        Set(ByVal value As BitmapSource)
            _preview = value
            NotifyPropertyChanged()
        End Set
    End Property

    Private _title As String
    Public Property Title() As String
        Get
            Return Me._title
        End Get
        Set(ByVal value As String)
            _title = value
            NotifyPropertyChanged()
        End Set
    End Property
End Class

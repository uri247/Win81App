'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits Windows.UI.Xaml.Controls.Page

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' holds the sample data - See SampleData folder
    Private storeData As Expression.Blend.SampleData.SampleDataSource.StoreData = Nothing

    Public Sub New()
        Me.InitializeComponent()

        ' create a new instance of store data
        storeData = New Expression.Blend.SampleData.SampleDataSource.StoreData()
        ' set the source of the GridView to be the sample data
        ItemListView.ItemsSource = storeData.Collection
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub
End Class

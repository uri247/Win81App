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
Imports Windows.Web

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private myResolver As StreamUriWinRTResolver

    Public Sub New()
        myResolver = New StreamUriWinRTResolver()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' The 'Host' part of the URI for the ms-local-stream protocol needs to be a combination of the package name
        ' and an application defined key, which identifies the specfic resolver, in this case 'Mytag'.

        Dim url As Uri = webView4.BuildLocalStreamUri("MyTag", "/Minesweeper/default.html")

        ' The resolver object needs to be passed in to the navigate call.
        webView4.NavigateToLocalStreamUri(url, myResolver)
        webViewLabel.Text = String.Format("Webview: {0}", url)
    End Sub
End Class

''' <summary>
''' Sample URI resolver object for use with NavigateToLocalStreamUri
''' This sample uses the local storage of the package as an example of how to write a resolver.
''' The object needs to implement the IUriToStreamResolver interface
''' 
''' Note: If you really want to browse the package content, the ms-appx-web:// protocol demonstrated
''' in scenario 3, is the simpler way to do that.
''' </summary>
Public NotInheritable Class StreamUriWinRTResolver
    Implements IUriToStreamResolver

    ''' <summary>
    ''' The entry point for resolving a Uri to a stream.
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <returns></returns>
    Public Function UriToStreamAsync(ByVal uri As Uri) As IAsyncOperation(Of IInputStream) Implements IUriToStreamResolver.UriToStreamAsync
        If uri Is Nothing Then
            Throw New Exception()
        End If
        Dim path As String = uri.AbsolutePath

        If System.Diagnostics.Debugger.IsAttached Then
            System.Diagnostics.Debug.WriteLine(String.Format("Stream Requested: {0}", uri.ToString()))
        End If

        ' Because of the signature of the this method, it can't use await, so we 
        ' call into a seperate helper method that can use the VB await pattern.
        Return getContent(path).AsAsyncOperation()
    End Function

    ''' <summary>
    ''' Helper that cracks the path and resolves the Uri
    ''' Uses the VB await pattern to coordinate async operations
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Private Async Function getContent(ByVal path As String) As Task(Of IInputStream)
        ' We use a package folder as the source, but the same principle should apply
        ' when supplying content from other locations
        Dim current As StorageFolder = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("html")

        ' Trim the initial '/' if applicable
        If path.StartsWith("/") Then
            path = path.Remove(0, 1)
        End If
        ' Split the path into an array of nodes
        Dim nodes() As String = path.Split("/"c)

        ' Walk the nodes of the path checking against the filesystem along the way
        For i As Integer = 0 To nodes.Length - 1
            Try
                ' Try and get the node from the file system
                Dim item As IStorageItem = Await current.GetItemAsync(nodes(i))

                If item.IsOfType(StorageItemTypes.Folder) AndAlso i < nodes.Length - 1 Then
                    ' If the item is a folder and isn't the leaf node
                    current = TryCast(item, StorageFolder)
                ElseIf item.IsOfType(StorageItemTypes.File) AndAlso i = nodes.Length - 1 Then
                    ' If the item is a file and is the leaf node
                    Dim f As StorageFile = TryCast(item, StorageFile)

                    Dim stream As IRandomAccessStream = Await f.OpenAsync(FileAccessMode.Read)
                    Return stream
                Else
                    Return Nothing
                    'Leaf is not a file, or the file isn't the leaf node in the path
                    Throw New Exception("Invalid path")
                End If
            Catch e1 As Exception
                Throw New Exception("Invalid path")
            End Try
        Next i
        Return Nothing
    End Function
End Class

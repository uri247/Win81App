'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.Specialized
Imports System.Threading

' This class can used as a jumpstart for implementing ISupportIncrementalLoading. 
' Implementing the ISupportIncrementalLoading interfaces allows you to create a list that loads
'  more data automatically when the user scrolls to the end of of a GridView or ListView.
Public MustInherit Class IncrementalLoadingBase
    Implements IList, ISupportIncrementalLoading, INotifyCollectionChanged

#Region "IList"

    Public Function Add(ByVal value As Object) As Integer Implements IList.Add
        Throw New NotImplementedException()
    End Function

    Public Sub Clear() Implements IList.Clear
        Throw New NotImplementedException()
    End Sub

    Public Function Contains(ByVal value As Object) As Boolean Implements IList.Contains
        Return _storage.Contains(value)
    End Function

    Public Function IndexOf(ByVal value As Object) As Integer Implements IList.IndexOf
        Return _storage.IndexOf(value)
    End Function

    Public Sub Insert(ByVal index As Integer, ByVal value As Object) Implements IList.Insert
        Throw New NotImplementedException()
    End Sub

    Public ReadOnly Property IsFixedSize() As Boolean Implements IList.IsFixedSize
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property IsReadOnly() As Boolean Implements IList.IsReadOnly
        Get
            Return True
        End Get
    End Property

    Public Sub Remove(ByVal value As Object) Implements IList.Remove
        Throw New NotImplementedException()
    End Sub

    Public Sub RemoveAt(ByVal index As Integer) Implements IList.RemoveAt
        Throw New NotImplementedException()
    End Sub

    Default Public Property Item(ByVal index As Integer) As Object Implements IList.Item
        Get
            Return _storage(index)
        End Get
        Set(ByVal value As Object)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Sub CopyTo(ByVal array As Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
        CType(_storage, IList).CopyTo(array, index)
    End Sub

    Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
        Get
            Return _storage.Count
        End Get
    End Property

    Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Function GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return _storage.GetEnumerator()
    End Function

#End Region

#Region "ISupportIncrementalLoading"

    Public ReadOnly Property HasMoreItems() As Boolean Implements ISupportIncrementalLoading.HasMoreItems
        Get
            Return HasMoreItemsOverride()
        End Get
    End Property

    Public Function LoadMoreItemsAsync(ByVal count As UInteger) As Windows.Foundation.IAsyncOperation(Of LoadMoreItemsResult) Implements ISupportIncrementalLoading.LoadMoreItemsAsync
        If _busy Then
            Throw New InvalidOperationException("Only one operation in flight at a time")
        End If

        _busy = True

        Return AsyncInfo.Run(Function(c) LoadMoreItemsAsync(c, count))
    End Function

#End Region

#Region "INotifyCollectionChanged"

    Public Event CollectionChanged As NotifyCollectionChangedEventHandler Implements INotifyCollectionChanged.CollectionChanged

#End Region

#Region "Private methods"

    Private Async Function LoadMoreItemsAsync(ByVal c As CancellationToken, ByVal count As UInteger) As Task(Of LoadMoreItemsResult)
        Try
            Dim items = Await LoadMoreItemsOverrideAsync(c, count)
            Dim baseIndex = _storage.Count

            _storage.AddRange(items)

            ' Now notify of the new items
            NotifyOfInsertedItems(baseIndex, items.Count)

            Return New LoadMoreItemsResult With {.Count = CUInt(items.Count)}
        Finally
            _busy = False
        End Try
    End Function

    Private Sub NotifyOfInsertedItems(ByVal baseIndex As Integer, ByVal count As Integer)
        If CollectionChangedEvent Is Nothing Then
            Return
        End If

        For i As Integer = 0 To count - 1
            Dim args = New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _storage(i + baseIndex), i + baseIndex)
            RaiseEvent CollectionChanged(Me, args)
        Next i
    End Sub

#End Region

#Region "Overridable methods"

    Protected MustOverride Function LoadMoreItemsOverrideAsync(ByVal c As CancellationToken, ByVal count As UInteger) As Task(Of IList(Of Object))
    Protected MustOverride Function HasMoreItemsOverride() As Boolean

#End Region

#Region "State"

    Private _storage As New List(Of Object)()
    Private _busy As Boolean = False

#End Region
End Class

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports DataBinding

' This class implements IncrementalLoadingBase. 
' To create your own Infinite List, you can create a class like this one that doesn't have 'generator' or 'maxcount', 
'  and instead downloads items from a live data source in LoadMoreItemsOverrideAsync.
Public Class GeneratorIncrementalLoadingClass(Of T)
    Inherits IncrementalLoadingBase

    Public Sub New(ByVal maxCount As UInteger, ByVal generator As Func(Of Integer, T))
        _generator = generator
        _maxCount = maxCount
    End Sub

    Protected Overrides Async Function LoadMoreItemsOverrideAsync(ByVal c As System.Threading.CancellationToken, ByVal count As UInteger) As Task(Of IList(Of Object))
        Dim toGenerate As UInteger = System.Math.Min(count, _maxCount - _count)

        ' Wait for work 
        Await Task.Delay(10)

        ' This code simply generates
        Dim values = From j In Enumerable.Range(CInt(_count), CInt(toGenerate)) _
                     Select CObj(_generator(j))
        _count += toGenerate

        Return values.ToArray()
    End Function

    Protected Overrides Function HasMoreItemsOverride() As Boolean
        Return _count < _maxCount
    End Function

#Region "State"

    Private _generator As Func(Of Integer, T)
    Private _count As UInteger = 0
    Private _maxCount As UInteger

#End Region
End Class

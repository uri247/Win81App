'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' This Data Source is used in Scenarios 4, 5 and 6.
Public Class Team 'Has a custom string indexer
    Private _propBag As Dictionary(Of String, Object)
    Public Sub New()
        _propBag = New Dictionary(Of String, Object)()
    End Sub

    Public Property Name() As String
    Public Property City() As String
    Public Property Color() As SolidColorBrush

    ' this is how you can create a custom indexer in vb
    Default Public Property Item(ByVal indexer As String) As Object
        Get
            Return _propBag(indexer)
        End Get
        Set(ByVal value As Object)
            _propBag(indexer) = value
        End Set
    End Property

    Public Sub Insert(ByVal key As String, ByVal value As Object)
        _propBag.Add(key, value)
    End Sub

End Class

' This class is used to demonstrate grouping.
Public Class Teams
    Inherits List(Of Team)

    Public Sub New()
        Add(New Team() With {.Name = "The Reds", .City = "Liverpool", .Color = New SolidColorBrush(Colors.Green)})
        Add(New Team() With {.Name = "The Red Devils", .City = "Manchester", .Color = New SolidColorBrush(Colors.Yellow)})
        Add(New Team() With {.Name = "The Blues", .City = "London", .Color = New SolidColorBrush(Colors.Orange)})
        Dim _t As New Team() With {.Name = "The Gunners", .City = "London", .Color = New SolidColorBrush(Colors.Red)}
        _t("Gaffer") = "le Professeur"
        _t("Skipper") = "Mr Gooner"

        Add(_t)
    End Sub

End Class

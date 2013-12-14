'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' More information on INotifyPropertyChanged can be found @ http://go.microsoft.com/fwlink/?LinkId=254639#change_notification
' For another way to accomplish this, see SampleDataSource.vb and BindableBase.vb in the Grid Application VB project template.
Public Class Employee 'Implement INotifiyPropertyChanged interface to subscribe for property change notifications
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private _name As String
    Private _organization As String
    Private _age? As Integer

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name <> value Then
                _name = value
                RaisePropertyChanged("Name")
            End If
        End Set
    End Property

    Public Property Organization() As String
        Get
            Return _organization
        End Get
        Set(ByVal value As String)
            If _organization <> value Then
                _organization = value
                RaisePropertyChanged("Organization")
            End If
        End Set
    End Property

    Public Property Age() As Integer?
        Get
            Return _age
        End Get
        Set(ByVal value As Integer?)
            If Not _age.Equals(value) Then
                _age = value
                RaisePropertyChanged("Age")
            End If
        End Set
    End Property

    Protected Sub RaisePropertyChanged(ByVal name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

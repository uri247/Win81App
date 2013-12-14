'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports Windows.Devices.Geolocation
Imports Windows.Devices.Geolocation.Geofencing

' GeofenceItem implements IEquatable to allow
' removal of objects in the collection
Public Class GeofenceItem
    Implements IEquatable(Of GeofenceItem)

    Private _geofence As Geofence
    Private geoID As String

    Public Sub New(ByVal geofence As Geofence)
        Me._geofence = geofence
        Me.geoID = geofence.Id
    End Sub

    Public Overloads Function Equals(ByVal other As GeofenceItem) As Boolean Implements IEquatable(Of GeofenceItem).Equals
        Dim isEqual As Boolean = False
        If Id = other.Id Then
            isEqual = True
        End If

        Return isEqual
    End Function

    Public ReadOnly Property Geofence() As Windows.Devices.Geolocation.Geofencing.Geofence
        Get
            Return _geofence
        End Get
    End Property

    Public ReadOnly Property Id() As String
        Get
            Return geoID
        End Get
    End Property

    Public ReadOnly Property Latitude() As Double
        Get
            Dim circle As Geocircle = TryCast(_geofence.Geoshape, Geocircle)
            Return circle.Center.Latitude
        End Get
    End Property

    Public ReadOnly Property Longitude() As Double
        Get
            Dim circle As Geocircle = TryCast(_geofence.Geoshape, Geocircle)
            Return circle.Center.Longitude
        End Get
    End Property

    Public ReadOnly Property Radius() As Double
        Get
            Dim circle As Geocircle = TryCast(_geofence.Geoshape, Geocircle)
            Return circle.Radius
        End Get
    End Property

    Public ReadOnly Property SingleUse() As Boolean
        Get
            Return _geofence.SingleUse
        End Get
    End Property

    Public ReadOnly Property MonitoredStates() As MonitoredGeofenceStates
        Get
            Return _geofence.MonitoredStates
        End Get
    End Property

    Public ReadOnly Property DwellTime() As TimeSpan
        Get
            Return _geofence.DwellTime
        End Get
    End Property

    Public ReadOnly Property StartTime() As DateTimeOffset
        Get
            Return _geofence.StartTime
        End Get
    End Property

    Public ReadOnly Property Duration() As TimeSpan
        Get
            Return _geofence.Duration
        End Get
    End Property
End Class


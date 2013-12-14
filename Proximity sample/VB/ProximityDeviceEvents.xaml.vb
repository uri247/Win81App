'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Networking.Proximity

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ProximityDeviceEvents
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private _proximityDevice As Windows.Networking.Proximity.ProximityDevice

    Public Sub New()
        Me.InitializeComponent()
        _proximityDevice = ProximityDevice.GetDefault()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        If _proximityDevice IsNot Nothing Then
            AddHandler _proximityDevice.DeviceArrived, AddressOf DeviceArrived
            AddHandler _proximityDevice.DeviceDeparted, AddressOf DeviceDeparted
        Else
            rootPage.NotifyUser("No proximity device found", NotifyType.ErrorMessage)
        End If
    End Sub
    ' Invoked when the main page navigates to a different scenario
    Protected Overrides Sub OnNavigatingFrom(ByVal e As NavigatingCancelEventArgs)
        If _proximityDevice IsNot Nothing Then
            RemoveHandler _proximityDevice.DeviceArrived, AddressOf DeviceArrived
            RemoveHandler _proximityDevice.DeviceDeparted, AddressOf DeviceDeparted
        End If
    End Sub

    Private Sub DeviceArrived(ByVal proximityDevice As ProximityDevice)
        rootPage.UpdateLog("Proximate device arrived", ProximityDeviceEventsOutputText)
    End Sub

    Private Sub DeviceDeparted(ByVal proximityDevice As ProximityDevice)
        rootPage.UpdateLog("Proximate device departed", ProximityDeviceEventsOutputText)
    End Sub
End Class

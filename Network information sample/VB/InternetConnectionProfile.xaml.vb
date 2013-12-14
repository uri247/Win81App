'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Networking.Connectivity

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class InternetConnectionProfile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'InternetConnectionProfileButton' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub InternetConnectionProfile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        '
        'Get Internet Connected Profile Information
        '
        Dim connectionProfileInfo As String = String.Empty
        Try
            Dim InternetConnectionProfile As ConnectionProfile = NetworkInformation.GetInternetConnectionProfile()

            If InternetConnectionProfile Is Nothing Then
                rootPage.NotifyUser("Not connected to Internet" & vbLf, NotifyType.StatusMessage)
            Else
                connectionProfileInfo = GetConnectionProfile(InternetConnectionProfile)
                rootPage.NotifyUser(connectionProfileInfo, NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occurred: " & ex.ToString(), NotifyType.ErrorMessage)
        End Try

    End Sub

    '
    'Get Connection Profile name and cost information
    '
    Private Function GetConnectionProfile(ByVal connectionProfile As ConnectionProfile) As String
        Dim connectionProfileInfo As String = String.Empty
        If connectionProfile IsNot Nothing Then
            connectionProfileInfo = "Profile Name : " & connectionProfile.ProfileName & vbLf

            Select Case connectionProfile.GetNetworkConnectivityLevel()
                Case NetworkConnectivityLevel.None
                    connectionProfileInfo &= "Connectivity Level : None" & vbLf
                Case NetworkConnectivityLevel.LocalAccess
                    connectionProfileInfo &= "Connectivity Level : Local Access" & vbLf
                Case NetworkConnectivityLevel.ConstrainedInternetAccess
                    connectionProfileInfo &= "Connectivity Level : Constrained Internet Access" & vbLf
                Case NetworkConnectivityLevel.InternetAccess
                    connectionProfileInfo &= "Connectivity Level : Internet Access" & vbLf
            End Select

            Select Case connectionProfile.GetDomainConnectivityLevel()
                Case DomainConnectivityLevel.None
                    connectionProfileInfo &= "Domain Connectivity Level : None" & vbLf
                Case DomainConnectivityLevel.Unauthenticated
                    connectionProfileInfo &= "Domain Connectivity Level : Unauthenticated" & vbLf
                Case DomainConnectivityLevel.Authenticated
                    connectionProfileInfo &= "Domain Connectivity Level : Authenticated" & vbLf
            End Select

            'Get Connection Cost information
            Dim connectionCost As ConnectionCost = connectionProfile.GetConnectionCost()
            connectionProfileInfo &= GetConnectionCostInfo(connectionCost)

            'Get Dataplan Status information
            Dim dataPlanStatus As DataPlanStatus = connectionProfile.GetDataPlanStatus()
            connectionProfileInfo &= GetDataPlanStatusInfo(dataPlanStatus)

            'Get Network Security Settings
            Dim netSecuritySettings As NetworkSecuritySettings = connectionProfile.NetworkSecuritySettings
            connectionProfileInfo &= GetNetworkSecuritySettingsInfo(netSecuritySettings)

            'Get Wlan Connection Profile Details if this is a Wlan ConnectionProfile
            If connectionProfile.IsWlanConnectionProfile Then
                Dim wlanConnectionProfileDetails As WlanConnectionProfileDetails = connectionProfile.WlanConnectionProfileDetails
                connectionProfileInfo &= GetWlanConnectionProfileDetailsInfo(wlanConnectionProfileDetails)
            End If

            'Get Wwan Connection Profile Details if this is a Wwan ConnectionProfile
            If connectionProfile.IsWwanConnectionProfile Then
                Dim wwanConnectionProfileDetails As WwanConnectionProfileDetails = connectionProfile.WwanConnectionProfileDetails
                connectionProfileInfo &= GetWwanConnectionProfileDetailsInfo(wwanConnectionProfileDetails)
            End If

            'Get the Service Provider GUID
            If connectionProfile.ServiceProviderGuid.HasValue Then
                connectionProfileInfo &= "====================" & vbLf
                connectionProfileInfo &= "Service Provider GUID: " & connectionProfile.ServiceProviderGuid.ToString() & vbLf
            End If

            'Get the number of signal bars
            If connectionProfile.GetSignalBars().HasValue Then
                connectionProfileInfo &= "====================" & vbLf
                connectionProfileInfo &= "Signal Bars: " & connectionProfile.GetSignalBars() & vbLf
            End If
        End If
        Return connectionProfileInfo
    End Function

    '
    'Get Profile Connection cost
    '
    Private Function GetConnectionCostInfo(ByVal connectionCost As ConnectionCost) As String
        Dim cost As String = String.Empty
        cost &= "Connection Cost Information: " & vbLf
        cost &= "====================" & vbLf

        If connectionCost Is Nothing Then
            cost &= "Connection Cost not available" & vbLf
            Return cost
        End If

        Select Case connectionCost.NetworkCostType
            Case NetworkCostType.Unrestricted
                cost &= "Cost: Unrestricted"
            Case NetworkCostType.Fixed
                cost &= "Cost: Fixed"
            Case NetworkCostType.Variable
                cost &= "Cost: Variable"
            Case NetworkCostType.Unknown
                cost &= "Cost: Unknown"
            Case Else
                cost &= "Cost: Error"
        End Select
        cost &= vbLf
        cost &= "Roaming: " & connectionCost.Roaming & vbLf
        cost &= "Over Data Limit: " & connectionCost.OverDataLimit & vbLf
        cost &= "Approaching Data Limit : " & connectionCost.ApproachingDataLimit & vbLf

        'Display cost based suggestions to the user
        cost &= CostBasedSuggestions(connectionCost)
        Return cost
    End Function

    '
    'Display Cost based suggestions to the user
    '
    Private Function CostBasedSuggestions(ByVal connectionCost As ConnectionCost) As String
        Dim costSuggestions As String = String.Empty
        costSuggestions &= "Cost Based Suggestions: " & vbLf
        costSuggestions &= "====================" & vbLf

        If connectionCost.Roaming Then
            costSuggestions &= "Connection is out of MNO's network, using the connection may result in additional charge. Application can implement High Cost behavior in this scenario" & vbLf
        ElseIf connectionCost.NetworkCostType = NetworkCostType.Variable Then
            costSuggestions &= "Connection cost is variable, and the connection is charged based on usage, so application can implement the Conservative behavior" & vbLf
        ElseIf connectionCost.NetworkCostType = NetworkCostType.Fixed Then
            If connectionCost.OverDataLimit OrElse connectionCost.ApproachingDataLimit Then
                costSuggestions &= "Connection has exceeded the usage cap limit or is approaching the datalimit, and the application can implement High Cost behavior in this scenario" & vbLf
            Else
                costSuggestions &= "Application can implemement the Conservative behavior" & vbLf
            End If
        Else
            costSuggestions &= "Application can implement the Standard behavior" & vbLf
        End If
        Return costSuggestions
    End Function

    '
    'Display Profile Dataplan Status information
    '
    Private Function GetDataPlanStatusInfo(ByVal dataPlan As DataPlanStatus) As String
        Dim dataplanStatusInfo As String = String.Empty
        dataplanStatusInfo = "Dataplan Status Information:" & vbLf
        dataplanStatusInfo &= "====================" & vbLf

        If dataPlan Is Nothing Then
            dataplanStatusInfo &= "Dataplan Status not available" & vbLf
            Return dataplanStatusInfo
        End If

        If dataPlan.DataPlanUsage IsNot Nothing Then
            dataplanStatusInfo &= "Usage In Megabytes : " & dataPlan.DataPlanUsage.MegabytesUsed & vbLf
            dataplanStatusInfo &= "Last Sync Time : " & dataPlan.DataPlanUsage.LastSyncTime.ToString() & vbLf
        Else
            dataplanStatusInfo &= "Usage In Megabytes : Not Defined" & vbLf
        End If

        Dim inboundBandwidth? As ULong = dataPlan.InboundBitsPerSecond
        If inboundBandwidth.HasValue Then
            dataplanStatusInfo &= "InboundBitsPerSecond : " & inboundBandwidth & vbLf
        Else
            dataplanStatusInfo &= "InboundBitsPerSecond : Not Defined" & vbLf
        End If

        Dim outboundBandwidth? As ULong = dataPlan.OutboundBitsPerSecond
        If outboundBandwidth.HasValue Then
            dataplanStatusInfo &= "OutboundBitsPerSecond : " & outboundBandwidth & vbLf
        Else
            dataplanStatusInfo &= "OutboundBitsPerSecond : Not Defined" & vbLf
        End If

        Dim dataLimit? As UInteger = dataPlan.DataLimitInMegabytes
        If dataLimit.HasValue Then
            dataplanStatusInfo &= "DataLimitInMegabytes : " & dataLimit & vbLf
        Else
            dataplanStatusInfo &= "DataLimitInMegabytes : Not Defined" & vbLf
        End If

        Dim nextBillingCycle? As System.DateTimeOffset = dataPlan.NextBillingCycle
        If nextBillingCycle.HasValue Then
            dataplanStatusInfo &= "NextBillingCycle : " & nextBillingCycle.ToString() & vbLf
        Else
            dataplanStatusInfo &= "NextBillingCycle : Not Defined" & vbLf
        End If

        Dim maxTransferSize? As UInteger = dataPlan.MaxTransferSizeInMegabytes
        If maxTransferSize.HasValue Then
            dataplanStatusInfo &= "MaxTransferSizeInMegabytes : " & maxTransferSize & vbLf
        Else
            dataplanStatusInfo &= "MaxTransferSizeInMegabytes : Not Defined" & vbLf
        End If
        Return dataplanStatusInfo
    End Function

    '
    'Get Network Security Settings information
    '
    Private Function GetNetworkSecuritySettingsInfo(ByVal netSecuritySettings As NetworkSecuritySettings) As String
        Dim networkSecurity As String = String.Empty
        networkSecurity &= "Network Security Settings: " & vbLf
        networkSecurity &= "====================" & vbLf

        If netSecuritySettings Is Nothing Then
            networkSecurity &= "Network Security Settings not available" & vbLf
            Return networkSecurity
        End If

        'NetworkAuthenticationType
        Select Case netSecuritySettings.NetworkAuthenticationType
            Case NetworkAuthenticationType.None
                networkSecurity &= "NetworkAuthenticationType: None"
            Case NetworkAuthenticationType.Unknown
                networkSecurity &= "NetworkAuthenticationType: Unknown"
            Case NetworkAuthenticationType.Open80211
                networkSecurity &= "NetworkAuthenticationType: Open80211"
            Case NetworkAuthenticationType.SharedKey80211
                networkSecurity &= "NetworkAuthenticationType: SharedKey80211"
            Case NetworkAuthenticationType.Wpa
                networkSecurity &= "NetworkAuthenticationType: Wpa"
            Case NetworkAuthenticationType.WpaPsk
                networkSecurity &= "NetworkAuthenticationType: WpaPsk"
            Case NetworkAuthenticationType.WpaNone
                networkSecurity &= "NetworkAuthenticationType: WpaNone"
            Case NetworkAuthenticationType.Rsna
                networkSecurity &= "NetworkAuthenticationType: Rsna"
            Case NetworkAuthenticationType.RsnaPsk
                networkSecurity &= "NetworkAuthenticationType: RsnaPsk"
            Case Else
                networkSecurity &= "NetworkAuthenticationType: Error"
        End Select
        networkSecurity &= vbLf

        'NetworkEncryptionType
        Select Case netSecuritySettings.NetworkEncryptionType
            Case NetworkEncryptionType.None
                networkSecurity &= "NetworkEncryptionType: None"
            Case NetworkEncryptionType.Unknown
                networkSecurity &= "NetworkEncryptionType: Unknown"
            Case NetworkEncryptionType.Wep
                networkSecurity &= "NetworkEncryptionType: Wep"
            Case NetworkEncryptionType.Wep40
                networkSecurity &= "NetworkEncryptionType: Wep40"
            Case NetworkEncryptionType.Wep104
                networkSecurity &= "NetworkEncryptionType: Wep104"
            Case NetworkEncryptionType.Tkip
                networkSecurity &= "NetworkEncryptionType: Tkip"
            Case NetworkEncryptionType.Ccmp
                networkSecurity &= "NetworkEncryptionType: Ccmp"
            Case NetworkEncryptionType.WpaUseGroup
                networkSecurity &= "NetworkEncryptionType: WpaUseGroup"
            Case NetworkEncryptionType.RsnUseGroup
                networkSecurity &= "NetworkEncryptionType: RsnUseGroup"
            Case Else
                networkSecurity &= "NetworkEncryptionType: Error"
        End Select
        networkSecurity &= vbLf
        Return networkSecurity
    End Function

    Private Function GetWlanConnectionProfileDetailsInfo(ByVal wlanConnectionProfileDetails As WlanConnectionProfileDetails) As String
        Dim wlanDetails As String = String.Empty
        wlanDetails &= "Wireless LAN Connection Profile Details:" & vbLf
        wlanDetails &= "====================" & vbLf

        If wlanConnectionProfileDetails Is Nothing Then
            wlanDetails &= "Wireless LAN connection profile details unavailable." & vbLf
            Return wlanDetails
        End If

        wlanDetails &= "Connected SSID: " & wlanConnectionProfileDetails.GetConnectedSsid() & vbLf
        Return wlanDetails
    End Function

    Private Function GetWwanConnectionProfileDetailsInfo(ByVal wwanConnectionProfileDetails As WwanConnectionProfileDetails) As String
        Dim wwanDetails As String = String.Empty
        wwanDetails &= "Wireless WAN Connection Profile Details:" & vbLf
        wwanDetails &= "====================" & vbLf

        If wwanConnectionProfileDetails Is Nothing Then
            wwanDetails &= "Wireless WAN connection profile details unavailable." & vbLf
            Return wwanDetails
        End If

        wwanDetails &= "Home Provider ID: " & wwanConnectionProfileDetails.HomeProviderId & vbLf
        wwanDetails &= "Access Point Name: " & wwanConnectionProfileDetails.AccessPointName & vbLf
        wwanDetails &= "Network Registration State: " & wwanConnectionProfileDetails.GetNetworkRegistrationState() & vbLf
        wwanDetails &= "Current Data Class: " & wwanConnectionProfileDetails.GetCurrentDataClass() & vbLf

        Return wwanDetails
    End Function
End Class



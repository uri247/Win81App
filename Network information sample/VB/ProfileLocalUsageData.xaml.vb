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
Partial Public NotInheritable Class ProfileLocalUsageData
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' These are set in the UI
    Private InternetConnectionProfile As ConnectionProfile
    Private NetworkUsageStates As NetworkUsageStates
    Private Granularity As DataUsageGranularity
    Private StartTime As DateTimeOffset
    Private EndTime As DateTimeOffset

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile()

        NetworkUsageStates = New NetworkUsageStates()
    End Sub

    Private Function PrintConnectivityInterval(ByVal connectivityInterval As ConnectivityInterval) As String
        Dim result As String = "------------" & vbLf & "New Interval with duration " & connectivityInterval.ConnectionDuration.ToString() & vbLf & vbLf

        Return result
    End Function

    Private Function PrintNetworkUsage(ByVal networkUsage As NetworkUsage, ByVal startTime As DateTimeOffset) As String
        Dim result As String = "Usage from " & startTime.ToString() & " to " & (startTime + networkUsage.ConnectionDuration).ToString() & vbLf & vbTab & "Bytes sent: " & networkUsage.BytesSent & vbLf & vbTab & "Bytes received: " & networkUsage.BytesReceived & vbLf

        Return result
    End Function

    Private Async Sub GetConnectivityIntervalsAsyncHandler(ByVal asyncInfo As IAsyncOperation(Of IReadOnlyList(Of ConnectivityInterval)), ByVal status As AsyncStatus)
        If status = AsyncStatus.Completed Then
            Try
                Dim outputString As String = String.Empty
                Dim connectivityIntervals As IReadOnlyList(Of ConnectivityInterval) = asyncInfo.GetResults()

                If connectivityIntervals Is Nothing Then
                    rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser("The Start Time cannot be later than the End Time, or in the future", NotifyType.StatusMessage))
                    Return
                End If

                ' Get the NetworkUsage for each ConnectivityInterval
                For Each connectivityInterval As ConnectivityInterval In connectivityIntervals
                    outputString &= PrintConnectivityInterval(connectivityInterval)

                    Dim start As DateTimeOffset = connectivityInterval.StartTime
                    Dim finish As DateTimeOffset = start + connectivityInterval.ConnectionDuration
                    Dim networkUsages As IReadOnlyList(Of NetworkUsage) = Await InternetConnectionProfile.GetNetworkUsageAsync(start, finish, Granularity, NetworkUsageStates)

                    For Each networkUsage As NetworkUsage In networkUsages
                        outputString &= PrintNetworkUsage(networkUsage, start)
                        start += networkUsage.ConnectionDuration
                    Next networkUsage
                Next connectivityInterval

                rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser(outputString, NotifyType.StatusMessage))
            Catch ex As Exception
                Dim s As String = "An unexpected error occurred: " & ex.Message
                rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser(s, NotifyType.ErrorMessage))
            End Try
        Else
            rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser("GetConnectivityIntervalsAsync failed with message:" & vbLf & asyncInfo.ErrorCode.Message, NotifyType.ErrorMessage))
        End If
    End Sub

    Private Function ParseDataUsageGranularity(ByVal input As String) As DataUsageGranularity
        Select Case input
            Case "Per Minute"
                Return DataUsageGranularity.PerMinute
            Case "Per Hour"
                Return DataUsageGranularity.PerHour
            Case "Per Day"
                Return DataUsageGranularity.PerDay
            Case Else
                Return DataUsageGranularity.Total
        End Select
    End Function

    Private Function ParseTriStates(ByVal input As String) As TriStates
        Select Case input
            Case "Yes"
                Return TriStates.Yes
            Case "No"
                Return TriStates.No
            Case Else
                Return TriStates.DoNotCare
        End Select
    End Function

    ''' <summary>
    ''' This is the click handler for the 'ProfileLocalUsageDataButton' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ProfileLocalUsageData_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        '
        'Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
        '

        Try
            Granularity = ParseDataUsageGranularity(CType(GranularityComboBox.SelectedItem, ComboBoxItem).Content.ToString())
            NetworkUsageStates.Roaming = ParseTriStates(CType(RoamingComboBox.SelectedItem, ComboBoxItem).Content.ToString())
            NetworkUsageStates.Shared = ParseTriStates(CType(SharedComboBox.SelectedItem, ComboBoxItem).Content.ToString())
            StartTime = (StartDatePicker.Date.Date + StartTimePicker.Time)
            EndTime = (EndDatePicker.Date.Date + EndTimePicker.Time)

            If InternetConnectionProfile Is Nothing Then
                rootPage.NotifyUser("Not connected to Internet" & vbLf, NotifyType.StatusMessage)
            Else
                InternetConnectionProfile.GetConnectivityIntervalsAsync(StartTime, EndTime, NetworkUsageStates).Completed = AddressOf GetConnectivityIntervalsAsyncHandler
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occurred: " & ex.ToString(), NotifyType.ErrorMessage)
        End Try
    End Sub
End Class

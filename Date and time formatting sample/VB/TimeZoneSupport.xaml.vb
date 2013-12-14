'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Text
Imports Windows.Globalization
Imports Windows.Globalization.DateTimeFormatting

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class TimeZoneSupport
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
    ''' This is the click handler for the 'Display' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Display_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' This scenario illustrates TimeZone support in DateTimeFormatter class

        ' Displayed TimeZones (other than local timezone)
        Dim timeZones() As String = {"UTC", "America/New_York", "Asia/Kolkata"}

        ' Store results here.
        Dim results As New StringBuilder()

        ' Create formatter object using longdate and longtime template
        Dim formatter As New DateTimeFormatter("longdate longtime")

        ' Create date/time to format and display.
        Dim dateTime As Date = Date.Now

        ' Show current time in timezones desired to be displayed including local timezone
        results.AppendLine("Current date and time -")
        results.AppendLine("In Local timezone:   " & formatter.Format(dateTime))
        For Each timeZone As String In timeZones
            results.AppendLine("In " & timeZone & " timezone:   " & formatter.Format(dateTime, timeZone))
        Next timeZone
        results.AppendLine()

        ' Show a time on 14th day of second month of next year in local, and other desired Time Zones
        ' This will show if there were day light savings in time
        results.AppendLine("Same time on 14th day of second month of next year -")
        dateTime = New Date(dateTime.Year + 1, 2, 14, dateTime.Hour, dateTime.Minute, dateTime.Second)
        results.AppendLine("In Local timezone:   " & formatter.Format(dateTime))
        For Each timeZone As String In timeZones
            results.AppendLine("In " & timeZone & " timezone:   " & formatter.Format(dateTime, timeZone))
        Next timeZone
        results.AppendLine()

        ' Show a time on 14th day of 10th month of next year in local, and other desired Time Zones
        ' This will show if there were day light savings in time
        results.AppendLine("Same time on 14th day of tenth month of next year -")
        dateTime = dateTime.AddMonths(8)
        results.AppendLine("In Local timezone:   " & formatter.Format(dateTime))
        For Each timeZone As String In timeZones
            results.AppendLine("In " & timeZone & " timezone:   " & formatter.Format(dateTime, timeZone))
        Next timeZone
        results.AppendLine()

        ' Display the results
        rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage)
    End Sub

End Class


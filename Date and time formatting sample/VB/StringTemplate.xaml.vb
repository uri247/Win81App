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
Partial Public NotInheritable Class StringTemplate
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
    ''' This is the click handler for the 'Default' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Display_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
        ' to format a date/time via a string template.  Note that the order specifed in the string pattern does
        ' not determine the order of the parts of the formatted string.  The user's language and region preferences will
        ' determine the pattern of the date returned based on the specified parts.

        ' We keep results in this variable
        Dim results As New StringBuilder()
        results.AppendLine("Current application context language: " & ApplicationLanguages.Languages(0))
        results.AppendLine()

        ' Create template-based date/time formatters.
        Dim templateFormatters() As DateTimeFormatter = { _
            New DateTimeFormatter("day month"), _
            New DateTimeFormatter("month year"), _
            New DateTimeFormatter("day month year"), _
            New DateTimeFormatter("dayofweek day month year"), _
            New DateTimeFormatter("dayofweek.abbreviated"), _
            New DateTimeFormatter("month.abbreviated"), _
            New DateTimeFormatter("year.abbreviated"), _
            New DateTimeFormatter("hour"), _
            New DateTimeFormatter("hour minute"), _
            New DateTimeFormatter("hour minute second"), _
            New DateTimeFormatter("timezone"), _
            New DateTimeFormatter("timezone.full"), _
            New DateTimeFormatter("timezone.abbreviated"), _
            New DateTimeFormatter("hour minute second timezone.full"), _
            New DateTimeFormatter("day month year hour minute timezone"), _
            New DateTimeFormatter("dayofweek day month year hour minute second"), _
            New DateTimeFormatter("dayofweek.abbreviated day month hour minute"), _
            New DateTimeFormatter("dayofweek day month year hour minute second timezone.abbreviated") _
        }


        ' Create date/time to format and display.
        Dim dateTime As Date = Date.Now

        ' Try to format and display date/time if calendar supports it.
        For Each formatter As DateTimeFormatter In templateFormatters
            Try
                ' Format and display date/time.
                results.AppendLine(formatter.Template & ": " & formatter.Format(dateTime))
            Catch e1 As ArgumentException
                ' Retrieve and display formatter properties. 
                results.AppendLine(String.Format("Unable to format Gregorian DateTime {0} using formatter with template {1} for languages [{2}], region {3}, calendar {4} and clock {5}", dateTime, formatter.Template, String.Join(",", formatter.Languages), formatter.GeographicRegion, formatter.Calendar, formatter.Clock))
            End Try
        Next formatter

        ' Display the results
        rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage)
    End Sub
End Class


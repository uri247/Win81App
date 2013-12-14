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
Partial Public NotInheritable Class UsingUnicodeExtensions
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
        ' to format a date/time by using a formatter that uses Unicode extenstion in the specified
        ' language name

        ' We keep results in this variable
        Dim results As New StringBuilder()
        results.AppendLine("Current default application context language: " & ApplicationLanguages.Languages(0))
        results.AppendLine()

        ' Create formatters using various types of constructors specifying Language list with unicode extension in language names
        Dim unicodeExtensionFormatters() As DateTimeFormatter = { _
            New DateTimeFormatter("longdate longtime"), _
            New DateTimeFormatter("longdate longtime", {"te-in-u-ca-gregory-nu-latn", "en-US"}), _
            New DateTimeFormatter(YearFormat.Default, MonthFormat.Default, DayFormat.Default, DayOfWeekFormat.Default, HourFormat.Default, MinuteFormat.Default, SecondFormat.Default, {"he-IL-u-nu-arab", "en-US"}), _
            New DateTimeFormatter(YearFormat.Default, MonthFormat.Default, DayFormat.Default, DayOfWeekFormat.Default, HourFormat.Default, MinuteFormat.Default, SecondFormat.Default, {"he-IL-u-ca-hebrew-co-phonebk", "en-US"}, "US", CalendarIdentifiers.Gregorian, ClockIdentifiers.TwentyFourHour) _
        }

        ' Create date/time to format and display.
        Dim dateTime As Date = Date.Now

        ' Try to format and display date/time along with other relevant properites
        For Each formatter As DateTimeFormatter In unicodeExtensionFormatters
            Try
                ' Format and display date/time.
                results.AppendLine("Using DateTimeFormatter with Language List:   " & String.Join(", ", formatter.Languages))
                results.AppendLine(vbTab & " Template:   " & formatter.Template)
                results.AppendLine(vbTab & " Resolved Language:   " & formatter.ResolvedLanguage)
                results.AppendLine(vbTab & " Calendar System:   " & formatter.Calendar)
                results.AppendLine(vbTab & " Numeral System:   " & formatter.NumeralSystem)
                results.AppendLine("Formatted DateTime:   " & formatter.Format(dateTime))
                results.AppendLine()
            Catch e1 As ArgumentException
                ' Retrieve and display formatter properties. 
                results.AppendLine(String.Format("Unable to format Gregorian DateTime {0} using formatter with template {1} for languages [{2}], region {3}, calendar {4} and clock {5}", dateTime, formatter.Template, String.Join(", ", formatter.Languages), formatter.GeographicRegion, formatter.Calendar, formatter.Clock))
            End Try
        Next formatter

        ' Display the results
        rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage)
    End Sub
End Class


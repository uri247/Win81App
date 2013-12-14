//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

// ProfileLocalUsageData.xaml.cpp
// Implementation of the ProfileLocalUsageData class

#include "pch.h"
#include "ProfileLocalUsageData.xaml.h"

using namespace SDKSample::NetworkInformationApi;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;
using namespace concurrency;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;

ProfileLocalUsageData::ProfileLocalUsageData()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ProfileLocalUsageData::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    internetConnectionProfile = NetworkInformation::GetInternetConnectionProfile();
    networkUsageStates = {};
    formatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("year month day hour minute second");
}

String^ ProfileLocalUsageData::PrintConnectivityIntervals(IVectorView<ConnectivityInterval^>^ connectivityIntervals)
{
    String^ result = "";

    if (connectivityIntervals == nullptr)
    {
        return "The Start Time cannot be later than the End Time, or in the future";
    }

    // Loop through the connectivity intervals and get the data usage for each one
    for (ConnectivityInterval^ connectivityInterval : connectivityIntervals)
    {
        result += PrintConnectivityInterval(connectivityInterval);

        startTime = connectivityInterval->StartTime;
        endTime.UniversalTime = startTime.UniversalTime + connectivityInterval->ConnectionDuration.Duration;

        // Since we're already in an asynchronous thread, it's okay to wait on the GetNetworkUsageAsync function synchronously
        auto getNetworkUsageTask = create_task(internetConnectionProfile->GetNetworkUsageAsync(startTime, endTime, granularity, networkUsageStates));
        IVectorView<NetworkUsage^>^ networkUsages = getNetworkUsageTask.get();
        for (NetworkUsage^ networkUsage : networkUsages)
        {
            result += PrintNetworkUsage(networkUsage, startTime);
            startTime.UniversalTime += networkUsage->ConnectionDuration.Duration;
        }
    }

    return result;
}

String^ ProfileLocalUsageData::PrintConnectivityInterval(ConnectivityInterval^ connectivityInterval)
{
    String^ result = "------------\n" +
        "New Interval with duration " + connectivityInterval->ConnectionDuration.Duration + "\n\n";

    return result;
}

String^ ProfileLocalUsageData::PrintNetworkUsage(NetworkUsage^ networkUsage, DateTime startTime)
{
    DateTime endTime = startTime;
    endTime.UniversalTime += networkUsage->ConnectionDuration.Duration;

    String^ result = "Usage from " + formatter->Format(startTime) + " to " + formatter->Format(endTime) +
        "\n\tBytes sent: " + networkUsage->BytesSent +
        "\n\tBytes received: " + networkUsage->BytesReceived + "\n";

    return result;
}

DateTime ProfileLocalUsageData::GetDateTimeFromUi(DatePicker^ datePicker, TimePicker^ timePicker)
{
    DateTime dateTime;
    // DatePicker::Date is a DateTime that includes the current time of the day with the date.
    // We use the Calendar to set the time-related fields to the beginning of the day.
    auto calendar = ref new Windows::Globalization::Calendar();
    calendar->SetDateTime(datePicker->Date);
    calendar->Hour = 12;
    calendar->Minute = 0;
    calendar->Second = 0;
    calendar->Period = 1;

    // Set the start time to the Date from StartDatePicker and the time from StartTimePicker
    dateTime.UniversalTime = calendar->GetDateTime().UniversalTime + timePicker->Time.Duration;

    return dateTime;
}

DataUsageGranularity ProfileLocalUsageData::GetGranularityFromUi()
{
    String^ granularityString = ((ComboBoxItem^)GranularityComboBox->SelectedItem)->Content->ToString();

    if (granularityString == "Per Minute")
    {
        return DataUsageGranularity::PerMinute;
    }
    else if (granularityString == "Per Hour")
    {
        return DataUsageGranularity::PerHour;
    }
    else if (granularityString == "Per Day")
    {
        return DataUsageGranularity::PerDay;
    }
    else
    {
        return DataUsageGranularity::Total;
    }
}

TriStates ProfileLocalUsageData::GetTriStatesFromUi(ComboBox^ triStatesComboBox)
{
    String^ triStates = ((ComboBoxItem^)triStatesComboBox->SelectedItem)->Content->ToString();

    if (triStates == "Yes")
    {
        return TriStates::Yes;
    }
    else if (triStates == "No")
    {
        return TriStates::No;
    }
    else
    {
        return TriStates::DoNotCare;
    }
}

// Display Local Data Usage for Internet Connection Profile for the past 1 hour
void SDKSample::NetworkInformationApi::ProfileLocalUsageData::ProfileLocalUsageData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        // Get settings from the UI
        granularity = GetGranularityFromUi();
        networkUsageStates.Shared = GetTriStatesFromUi(SharedComboBox);
        networkUsageStates.Roaming = GetTriStatesFromUi(RoamingComboBox);
        startTime = GetDateTimeFromUi(StartDatePicker, StartTimePicker);
        endTime = GetDateTimeFromUi(EndDatePicker, EndTimePicker);

        if (internetConnectionProfile == nullptr)
        {
            rootPage->NotifyUser("Not connected to Internet\n", NotifyType::StatusMessage);
        }
        else
        {
            // Get the list of ConnectivityIntervals asynchronously
            auto getConnectivityIntervalsTask = create_task(internetConnectionProfile->GetConnectivityIntervalsAsync(startTime, endTime, networkUsageStates));
            getConnectivityIntervalsTask.then([this](task<IVectorView<ConnectivityInterval^>^> tConnectivityIntervals)
            {
                try
                {
                    auto connectivityIntervals = tConnectivityIntervals.get();
                    String^ outputString;

                    outputString += PrintConnectivityIntervals(connectivityIntervals);

                    rootPage->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, outputString]()
                    {
                        rootPage->NotifyUser(outputString, NotifyType::StatusMessage);
                    }));
                }
                catch (Exception^ ex)
                {
                    rootPage->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, ex]()
                    {
                        rootPage->NotifyUser("An unexpected exception has occurred: " + ex->Message, NotifyType::ErrorMessage);
                    }));
                }
            }, task_continuation_context::use_arbitrary()); // Ensure the continuation is run in an arbitrary thread, rather than the UI thread
        }
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occurred: " + ex->Message, NotifyType::ErrorMessage);
    }
}

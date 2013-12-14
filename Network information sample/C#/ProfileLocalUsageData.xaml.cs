//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Collections.Generic;
using Windows.Networking.Connectivity;
using Windows.Foundation;

namespace NetworkInformationApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfileLocalUsageData : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // These are set in the UI
        private ConnectionProfile InternetConnectionProfile;
        private NetworkUsageStates NetworkUsageStates;
        private DataUsageGranularity Granularity;
        private DateTimeOffset StartTime;
        private DateTimeOffset EndTime;

        public ProfileLocalUsageData()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

            NetworkUsageStates = new NetworkUsageStates();
        }

        private string PrintConnectivityInterval(ConnectivityInterval connectivityInterval)
        {
            string result = "------------\n" +
                "New Interval with duration " + connectivityInterval.ConnectionDuration + "\n\n";

            return result;
        }

        private string PrintNetworkUsage(NetworkUsage networkUsage, DateTimeOffset startTime)
        {
            string result = "Usage from " + startTime.ToString() + " to " + (startTime + networkUsage.ConnectionDuration).ToString() +
                "\n\tBytes sent: " + networkUsage.BytesSent +
                "\n\tBytes received: " + networkUsage.BytesReceived + "\n";

            return result;
        }

        async private void GetConnectivityIntervalsAsyncHandler(IAsyncOperation<IReadOnlyList<ConnectivityInterval>> asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                try
                {
                    String outputString = string.Empty;
                    IReadOnlyList<ConnectivityInterval> connectivityIntervals = asyncInfo.GetResults();

                    if (connectivityIntervals == null)
                    {
                        rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                rootPage.NotifyUser("The Start Time cannot be later than the End Time, or in the future", NotifyType.StatusMessage);
                            });
                        return;
                    }

                    // Get the NetworkUsage for each ConnectivityInterval
                    foreach (ConnectivityInterval connectivityInterval in connectivityIntervals)
                    {
                        outputString += PrintConnectivityInterval(connectivityInterval);

                        DateTimeOffset startTime = connectivityInterval.StartTime;
                        DateTimeOffset endTime = startTime + connectivityInterval.ConnectionDuration;
                        IReadOnlyList<NetworkUsage> networkUsages = await InternetConnectionProfile.GetNetworkUsageAsync(startTime, endTime, Granularity, NetworkUsageStates);

                        foreach (NetworkUsage networkUsage in networkUsages)
                        {
                            outputString += PrintNetworkUsage(networkUsage, startTime);
                            startTime += networkUsage.ConnectionDuration;
                        }
                    }

                    rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            rootPage.NotifyUser(outputString, NotifyType.StatusMessage);
                        });
                }
                catch (Exception ex)
                {
                    rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            rootPage.NotifyUser("An unexpected error occurred: " + ex.Message, NotifyType.ErrorMessage);
                        });
                }
            }
            else
            {
                rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        rootPage.NotifyUser("GetConnectivityIntervalsAsync failed with message:\n" + asyncInfo.ErrorCode.Message, NotifyType.ErrorMessage);
                    });
            }
        }

        private DataUsageGranularity ParseDataUsageGranularity(string input)
        {
            switch (input)
            {
                case "Per Minute":
                    return DataUsageGranularity.PerMinute;
                case "Per Hour":
                    return DataUsageGranularity.PerHour;
                case "Per Day":
                    return DataUsageGranularity.PerDay;
                default:
                    return DataUsageGranularity.Total;
            }
        }

        private TriStates ParseTriStates(string input)
        {
            switch (input)
            {
                case "Yes":
                    return TriStates.Yes;
                case "No":
                    return TriStates.No;
                default:
                    return TriStates.DoNotCare;
            }
        }

        /// <summary>
        /// This is the click handler for the 'ProfileLocalUsageDataButton' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProfileLocalUsageData_Click(object sender, RoutedEventArgs e)
        {      
            //
            //Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
            //

            try
            {
                Granularity = ParseDataUsageGranularity(((ComboBoxItem)GranularityComboBox.SelectedItem).Content.ToString());
                NetworkUsageStates.Roaming = ParseTriStates(((ComboBoxItem)RoamingComboBox.SelectedItem).Content.ToString());
                NetworkUsageStates.Shared = ParseTriStates(((ComboBoxItem)SharedComboBox.SelectedItem).Content.ToString());
                StartTime = (StartDatePicker.Date.Date + StartTimePicker.Time);
                EndTime = (EndDatePicker.Date.Date + EndTimePicker.Time);

                if (InternetConnectionProfile == null)
                {
                    rootPage.NotifyUser("Not connected to Internet\n", NotifyType.StatusMessage);
                }
                else
                {
                    InternetConnectionProfile.GetConnectivityIntervalsAsync(StartTime,
                        EndTime, NetworkUsageStates).Completed = GetConnectivityIntervalsAsyncHandler;
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occurred: " + ex.ToString(), NotifyType.ErrorMessage);
            }
        }
    }
}

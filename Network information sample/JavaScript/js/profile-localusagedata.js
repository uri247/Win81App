//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var networkInfo = Windows.Networking.Connectivity.NetworkInformation;
    var connectivityNS = Windows.Networking.Connectivity;
    var page = WinJS.UI.Pages.define("/html/profile-localusagedata.html", {
        ready: function (element, options) {
            document.getElementById("scenario2").addEventListener("click", displayLocalDataUsage, false);
            startDatePicker = new WinJS.UI.DatePicker(StartDatePicker);
            startTimePicker = new WinJS.UI.TimePicker(StartTimePicker);
            endDatePicker = new WinJS.UI.DatePicker(EndDatePicker);
            endTimePicker = new WinJS.UI.TimePicker(EndTimePicker);
        }
    });

    // Declare the Date and Time Picker objects
    var startDatePicker;
    var startTimePicker;
    var startDateTime;
    var endDatePicker;
    var endTimePicker;
    var endDateTime;

    var internetConnectionProfile = networkInfo.getInternetConnectionProfile();
    var granularity = connectivityNS.DataUsageGranularity.perHour;
    var networkUsageStates = {
        roaming: connectivityNS.TriStates.doNotCare,
        shared: connectivityNS.TriStates.doNotCare
    };

    function parseDataUsageGranularity(input) {
        switch (input) {
            case "Per Minute":
                return connectivityNS.DataUsageGranularity.perMinute;
            case "Per Hour":
                return connectivityNS.DataUsageGranularity.perHour;
            case "Per Day":
                return connectivityNS.DataUsageGranularity.perDay;
            default:
                return connectivityNS.DataUsageGranularity.total;
        }
    }

    function parseTriStates(input) {
        switch (input) {
            case "Yes":
                return connectivityNS.TriStates.yes;
            case "No":
                return connectivityNS.TriStates.no;
            default:
                return connectivityNS.TriStates.doNotCare;
        }
    }

    function printConnectivityInterval(connectivityInterval) {
        var result = "------------\n" +
            "New Interval with duration " + connectivityInterval.connectionDuration + "\n\n";

        return result;
    }

    function printNetworkUsage(networkUsage, startTime) {
        var endTime = new Date();
        endTime.setTime(startTime.getTime() + networkUsage.connectionDuration);

        var result = "Usage with start time " + startTime + " and end time " + endTime +
            "\n\tBytes sent: " + networkUsage.bytesSent +
            "\n\tBytes received: " + networkUsage.bytesReceived + "\n";

        return result;
    }

    function getConnectivityIntervalsCompletedHandler(connectivityIntervals) {
        // ConnectivityIntervals can be null if the start time is earlier than the end time
        if (connectivityIntervals === null) {
            WinJS.log && WinJS.log("An error has occurred: Ensure that the start time is earlier than the end time", "sample", "error");
            return;
        }

        var outputString = "";
        var networkUsagesPromises = [];

        // Loop through the ConnectivityIntervals and getNetworkUsageAsync for each of them
        // We add the promises returned by these calls to getNetworkUsageAsync to a list which we can join later
        for (var i = 0; i < connectivityIntervals.size; i++) {
            var connectivityInterval = connectivityIntervals[i];
            var startTime = connectivityIntervals[i].startTime;
            var endTime = new Date();
            endTime.setTime(startTime.getTime() + connectivityInterval.connectionDuration);

            networkUsagesPromises[i] = internetConnectionProfile.getNetworkUsageAsync(startTime, endTime, granularity, networkUsageStates);
        }

        // Use WinJS.Promise.join to ensure that all of the calls to getNetworkUsageAsync have finished before we print anything.
        WinJS.Promise.join(networkUsagesPromises).then(
            function () {
                for (var j = 0; j < networkUsagesPromises.length; j++) {
                    var time = new Date();
                    time.setTime(connectivityIntervals[j].startTime);
                    outputString += printConnectivityInterval(connectivityIntervals[j]);
                    var networkUsages = networkUsagesPromises[j].operation.getResults();

                    for (var k = 0; k < networkUsages.size; k++) {
                        outputString += printNetworkUsage(networkUsages[k], time);
                        startTime.setTime(startTime.getTime() + networkUsages[k].connectionDuration);
                    }
                }
                WinJS.log && WinJS.log(outputString, "sample", "status");
            },
            function (error) {
                // This can happen if you try to get the network usage for a long period of time with
                // too fine a granularity.
                WinJS.log && WinJS.log("An error has occurred: " + error, "sample", "status");
            });
    }

    function getConnectivityIntervalsErrorHandler(error) {
        WinJS.log && WinJS.log("An error has occurred: " + error, "sample", "error");
    }

    //
    //Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
    //
    function displayLocalDataUsage() {
        try {
            // Get settings from the UI
            granularity = parseDataUsageGranularity(GranularitySelect.children[GranularitySelect.selectedIndex].textContent);
            networkUsageStates.roaming = parseTriStates(RoamingSelect.children[RoamingSelect.selectedIndex].textContent);
            networkUsageStates.shared = parseTriStates(SharedSelect.children[SharedSelect.selectedIndex].textContent);

            // Add together the values from the DatePicker and the TimePicker
            // Note: The date portion of the value returned by TimePicker is always July 15, 2011
            startDateTime = startTimePicker.current;
            startDateTime.setFullYear(startDatePicker.current.getFullYear(), startDatePicker.current.getMonth(), startDatePicker.current.getDate());

            endDateTime = endTimePicker.current;
            endDateTime.setFullYear(endDatePicker.current.getFullYear(), endDatePicker.current.getMonth(), endDatePicker.current.getDate());

            if (internetConnectionProfile === null) {
                WinJS.log && WinJS.log("Not connected to Internet\n\r", "sample", "status");
            }
            else {
                internetConnectionProfile.getConnectivityIntervalsAsync(startDateTime, endDateTime, networkUsageStates).then(
                    getConnectivityIntervalsCompletedHandler, getConnectivityIntervalsErrorHandler);
            }
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();

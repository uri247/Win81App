//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Custom templating function
var RegisteredGeofencesItemTemplate = WinJS.Utilities.markSupportedForProcessing(function RegisteredGeofencesItemTemplate(itemPromise) {
    return itemPromise.then(function (currentItem) {

        // Build ListView Item Container div
        var result = document.createElement("div");
        result.className = "GeofenceListViewItemStyle";

        // Build content body
        var body = document.createElement("div");

        // Display title
        if (currentItem.data) {
            var title = document.createElement("span");
            title.innerText = currentItem.data.id + " (" + currentItem.data.latitude + ", " + currentItem.data.longitude + ", " + currentItem.data.radius + ")";
            body.appendChild(title);
        }

        // put the body into the ListView Item
        result.appendChild(body);

        return result;
    });
});

// Custom templating function
var GeofenceEventsItemTemplate = WinJS.Utilities.markSupportedForProcessing(function GeofenceEventsItemTemplate(itemPromise) {
    return itemPromise.then(function (currentItem) {

        // Build ListView Item Container div
        var result = document.createElement("div");
        result.className = "GeofenceListViewItemStyle";

        // Build content body
        var body = document.createElement("div");

        // Display title
        if (currentItem.data) {
            var title = document.createElement("span");
            title.innerText = currentItem.data;
            body.appendChild(title);
        }

        // put the body into the ListView Item
        result.appendChild(body);

        return result;
    });
});

var itemToRemove;
var itemToRemoveId;
var nameElement;
var charCount;
var latitude;
var longitude;
var radius;
var geofenceSingleUse;
var dwellTimeField;
var durationField;
var startTimeField;
var latitudeSet = false;
var longitudeSet = false;
var radiusSet = false;
var nameSet = false;
var secondsPerMinute = 60;
var secondsPerHour = 60 * secondsPerMinute;
var secondsPerDay = 24 * secondsPerHour;
var millisecondsPerSecond = 1000;
var defaultDwellTimeSeconds = 10;
var useStartTime = false;
var setStartTimeToNow;
var setPositionToHere;
var formatterShortDateLongTime;
var formatterLongTime;
var calendar;
var decimalFormatter;

function setupUI() {
    formatterShortDateLongTime = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("{month.integer}/{day.integer}/{year.full} {hour.integer}:{minute.integer(2)}:{second.integer(2)}", ["en-US"], "US", Windows.Globalization.CalendarIdentifiers.gregorian, Windows.Globalization.ClockIdentifiers.twentyFourHour);
    formatterLongTime = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("{hour.integer}:{minute.integer(2)}:{second.integer(2)}", ["en-US"], "US", Windows.Globalization.CalendarIdentifiers.gregorian, Windows.Globalization.ClockIdentifiers.twentyFourHour);
    calendar = new Windows.Globalization.Calendar();
    decimalFormatter = new Windows.Globalization.NumberFormatting.DecimalFormatter();

    document.getElementById("createGeofenceButton").disabled = true;
    document.getElementById("removeGeofenceItem").disabled = true;

    nameElement = document.getElementById("name");
    charCount = document.getElementById("charCount");
    latitude = document.getElementById("latitude");
    longitude = document.getElementById("longitude");
    radius = document.getElementById("radius");
    geofenceSingleUse = document.getElementById("geofenceSingleUse");
    dwellTimeField = document.getElementById("dwellTime");
    durationField = document.getElementById("duration");
    startTimeField = document.getElementById("startTime");
    setStartTimeToNow = document.getElementById("setStartTimeToNowButton");
    setPositionToHere = document.getElementById("setPositionToHereButton");

    nameElement.addEventListener("input", onNameChanged, false);
    latitude.addEventListener("input", onLatitudeChanged, false);
    longitude.addEventListener("input", onLongitudeChanged, false);
    radius.addEventListener("input", onRadiusChanged, false);
    setStartTimeToNow.addEventListener("click", onSetStartTimeToNow, false);
}

function getTimeStampedMessage(eventCalled) {
    calendar.setToNow();

    return eventCalled + " " + formatterLongTime.format(calendar.getDateTime());
}

function setStartTimeToNowFunction() {
    var startTime = new Date(); // with no params the startTime is Now

    startTimeField.value = formatterShortDateLongTime.format(startTime);
}

function onSetStartTimeToNow() {
    setStartTimeToNowFunction();
}

function onNameChanged() {
    // get number of characters
    if (nameElement.value) {
        var count = nameElement.value.length;
        charCount.innerText = count.toString() + " characters";
        if (0 !== count) {
            nameSet = true;
        } else {
            nameSet = false;
        }
    } else {
        charCount.innerText = "0 characters";
        nameSet = false;
    }

    document.getElementById("createGeofenceButton").disabled = !settingsAvailable();
}

function setLatitudeInUI(latitudeArg) {
    latitude.value = latitudeArg;
    onLatitudeChanged();
}

function setLongitudeInUI(longitudeArg) {
    longitude.value = longitudeArg;
    onLongitudeChanged();
}

function onLatitudeChanged() {
    if (textChangedHandlerDouble(false, "Latitude", latitude)) {
        latitudeSet = true;
    } else {
        latitudeSet = false;
    }

    document.getElementById("createGeofenceButton").disabled = !settingsAvailable();
}

function onLongitudeChanged() {
    if (textChangedHandlerDouble(false, "Longitude", longitude)) {
        longitudeSet = true;
    } else {
        longitudeSet = false;
    }

    document.getElementById("createGeofenceButton").disabled = !settingsAvailable();
}

function onRadiusChanged() {
    if (textChangedHandlerDouble(false, "Radius", radius)) {
        radiusSet = true;
    } else {
        radiusSet = false;
    }

    document.getElementById("createGeofenceButton").disabled = !settingsAvailable();
}

function textChangedHandlerDouble(nullAllowed, elementName, e) {
    var valueSet = false;

    var stringValue = new String(e.value);

    var value = decimalFormatter.parseDouble(stringValue);

    if (!value) {
        var msg;
        // value is either empty or alphabetic
        if (0 === stringValue.length) {
            if (!nullAllowed) {
                if (elementName) {
                    msg = elementName + " needs a value";
                    WinJS.log && WinJS.log(msg, "sample", "status");
                }
            }
        } else {
            if (elementName) {
                msg = elementName + " must be a number";
                WinJS.log && WinJS.log(msg, "sample", "status");
            }
        }
    } else {
        valueSet = true;
    }

    if (valueSet) {
        // clear out status message
        WinJS.log && WinJS.log("", "sample", "status");
    }

    return valueSet;
}

function getTimeComponentAsString(str, timeComponent, appendDelimiter) {
    if (timeComponent) {
        if (timeComponent < 10 && str.length) {
            str += "0";
        }

        str += timeComponent.toString();
    } else if (str.length) {
        str += "00";
    }

    if (appendDelimiter && str.length) {
        str += ":";
    }

    return str;
}

function getDurationString(duration) {
    // note that double negation turns a float to an int
    var totalSeconds = ~~(duration / millisecondsPerSecond);
    var days = ~~(totalSeconds / secondsPerDay);
    totalSeconds -= days * secondsPerDay;
    var hours = ~~(totalSeconds / secondsPerHour);
    totalSeconds -= hours * secondsPerHour;
    var minutes = ~~(totalSeconds / secondsPerMinute);
    totalSeconds -= minutes * secondsPerMinute;
    var seconds = totalSeconds;
    var str = new String("");

    str = getTimeComponentAsString(str, days, true);
    str = getTimeComponentAsString(str, hours, true);
    str = getTimeComponentAsString(str, minutes, true);
    str = getTimeComponentAsString(str, seconds, false);

    return str;
}

function refreshControlsFromGeofenceItem(item) {
    if (item) {
        nameElement.value = item.id;
        latitude.value = item.latitude;
        longitude.value = item.longitude;
        radius.value = item.radius;
        geofenceSingleUse.checked = item.singleUse;

        if (0 !== item.dwellTime) {
            dwellTimeField.value = getDurationString(item.dwellTime);
        } else {
            dwellTimeField.value = "";
        }
        if (0 !== item.duration) {
            durationField.value = getDurationString(item.duration);
        } else {
            durationField.value = "";
        }
        if (0 !== item.startTime) {
            startTimeField.value = formatterShortDateLongTime.format(item.startTime);
        } else {
            startTimeField.value = "";
        }

        // Update flags used to enable Create Geofence button
        onNameChanged();
        onLongitudeChanged();
        onLatitudeChanged();
        onRadiusChanged();
    }
}

// are settings available so a geofence can be created?
function settingsAvailable() {
    var fSettingsAvailable = false;

    if (nameSet && latitudeSet && longitudeSet && radiusSet) {
        // also need to test if data is good
        fSettingsAvailable = true;
    }

    return fSettingsAvailable;
}

var TimeSpanFields = Object.freeze({
    day: 0,
    hour: 1,
    minute: 2,
    second: 3
});

function parseTimeSpan(field, defaultValue) {
    var timeSpanValue = 0;
    var vars = field.value.split(':');
    var start = 4 - vars.length;

    if (start >= 0) {
        var loop = 0;
        var varsIndex = start;
        for (; loop < vars.length; loop++, varsIndex++) {
            switch (varsIndex) {
                case TimeSpanFields.day:
                    timeSpanValue += decimalFormatter.parseInt(vars[loop]) * secondsPerDay;
                    break;
                case TimeSpanFields.hour:
                    timeSpanValue += decimalFormatter.parseInt(vars[loop]) * secondsPerHour;
                    break;
                case TimeSpanFields.minute:
                    timeSpanValue += decimalFormatter.parseInt(vars[loop]) * secondsPerMinute;
                    break;
                case TimeSpanFields.second:
                    timeSpanValue += decimalFormatter.parseInt(vars[loop]);
                    break;
            }
        }
    }

    if (0 === timeSpanValue) {
        timeSpanValue = defaultValue;
    }

    timeSpanValue *= millisecondsPerSecond;

    return timeSpanValue;
}

function generateGeofence() {
    var geofence = null;
    try {
        var fenceKey = nameElement.value;

        var position = {
            latitude: decimalFormatter.parseDouble(latitude.value),
            longitude: decimalFormatter.parseDouble(longitude.value),
            altitude: 0
        };
        var radiusValue = decimalFormatter.parseDouble(radius.value);

        // the geofence is a circular region
        var geocircle = new Windows.Devices.Geolocation.Geocircle(position, radiusValue);

        var singleUse = false;

        if (geofenceSingleUse.checked) {
            singleUse = true;
        }

        // want to listen for enter geofence, exit geofence and remove geofence events
        var mask = 0;

        mask = mask | Windows.Devices.Geolocation.Geofencing.MonitoredGeofenceStates.entered;
        mask = mask | Windows.Devices.Geolocation.Geofencing.MonitoredGeofenceStates.exited;
        mask = mask | Windows.Devices.Geolocation.Geofencing.MonitoredGeofenceStates.removed;

        var dwellTimeSpan = new Number(parseTimeSpan(dwellTimeField, defaultDwellTimeSeconds));
        var durationTimeSpan = null;
        if (durationField.value.length) {
            durationTimeSpan = new Number(parseTimeSpan(durationField, 0));
        } else {
            durationTimeSpan = new Number(0); // duration needs to be set since start time is set below
        }
        var startDateTime = null;
        if (startTimeField.value.length) {
            startDateTime = new Date(startTimeField.value);
        } else {
            startDateTime = new Date(); // if you don't set start time in JavaScript the start time defaults to 1/1/1601
        }

        geofence = new Windows.Devices.Geolocation.Geofencing.Geofence(fenceKey, geocircle, mask, singleUse, dwellTimeSpan, startDateTime, durationTimeSpan);
    } catch (ex) {
        WinJS.log && WinJS.log(ex.toString(), "sample", "error");
    }

    return geofence;
}

//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var GeofenceItem = WinJS.Class.define(
    function (geofenceArg) {
        this.geofence = geofenceArg;
        this.id = geofenceArg.id;
        this.latitude = geofenceArg.geoshape.center.latitude;
        this.longitude = geofenceArg.geoshape.center.longitude;
        this.radius = geofenceArg.geoshape.radius;
        this.dwellTime = geofenceArg.dwellTime;
        this.singleUse = geofenceArg.singleUse;
        this.mask = geofenceArg.monitoredStates;
        this.duration = geofenceArg.duration;
        this.startTime = geofenceArg.startTime;
    }, {
        geofenceStored: function () {
            return this.geofence;
        },
        name: function () {
            return this.id;
        },
        latitude: function () {
            return this.latitude;
        },
        longitude: function () {
            return this.longitude;
        },
        radius: function () {
            return this.radius;
        },
        dwellTime: function () {
            return this.dwellTime;
        },
        singleUse: function () {
            return this.singleUse;
        },
        monitoredStates: function () {
            return this.mask;
        },
        duration: function () {
            return this.duration;
        },
        startTime: function () {
            return this.startTime;
        }
    }
);

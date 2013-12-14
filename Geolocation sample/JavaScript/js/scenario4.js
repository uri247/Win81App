//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var Enumeration = Windows.Devices.Enumeration;
    var DeviceAccessInformation = Enumeration.DeviceAccessInformation;
    var DeviceAccessStatus = Enumeration.DeviceAccessStatus;
    var geolocator = null;
    var geofences = null;
    var accessInfo = null;
    var geofence = null;
    var geofenceItem = null;
    var promise;
    var pageLoaded = false;
    var permissionsChecked = false;
    var inGetPositionAsync = false;
    var maxEventDescriptors = 42;   // Value determined by how many max length event descriptors (91 chars) 
                                    // stored as a JSON string can fit in 8K (max allowed for local settings)

    var registeredGeofenceData;
    var eventsData;
    var registeredGeofenceListView;
    var geofenceEventsListView;

    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            document.getElementById("createGeofenceButton").addEventListener("click", onCreateGeofence, false);
            document.getElementById("removeGeofenceItem").addEventListener("click", onRemoveGeofenceItem, false);
            document.getElementById("setPositionToHereButton").addEventListener("click", onSetPositionToHere, false);

            setupUI();

            try {
                geolocator = Windows.Devices.Geolocation.Geolocator();
                geofences = Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.geofences;

                registeredGeofenceData = new WinJS.Binding.List();
                eventsData = new WinJS.Binding.List();

                registeredGeofenceListView = element.querySelector('#RegisteredGeofenceListView').winControl;
                geofenceEventsListView = element.querySelector('#GeofenceEventsListView').winControl;
                registeredGeofenceListView.forceLayout();
                geofenceEventsListView.forceLayout();

                registeredGeofenceListView.addEventListener("selectionchanged", onRegisteredGeofenceListViewSelectionChanged, false);

                accessInfo = DeviceAccessInformation.createFromDeviceClass(Enumeration.DeviceClass.location);
                accessInfo.addEventListener("accesschanged", onAccessChanged);

                document.addEventListener("visibilitychange", onVisibilityChanged, false);

                // register for geofence state change events
                Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.addEventListener("geofencestatechanged", onGeofenceStateChanged);
                Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.addEventListener("statuschanged", onGeofenceStatusChanged);

                WinJS.UI.processAll().done(function () {
                    registeredGeofenceListView.itemDataSource = registeredGeofenceData.dataSource;
                    geofenceEventsListView.itemDataSource = eventsData.dataSource;

                    registeredGeofenceListView.itemTemplate = RegisteredGeofencesItemTemplate;
                    geofenceEventsListView.itemTemplate = GeofenceEventsItemTemplate;

                    fillRegisteredGeofenceListViewWithExistingGeofences();
                    fillEventListBoxWithExistingEvents();
                });
            } catch (ex) {
                // If there are no location sensors GetGeopositionAsync()
                // will timeout -- that is acceptable.

                // HRESULT_FROM_WIN32(WAIT_TIMEOUT) === -2147024891

                if (ex.number === -2147024891) {
                    if (DeviceAccessStatus.deniedByUser === accessInfo.currentStatus) {
                        WinJS.log && WinJS.log("Location has been disabled by the user. Enable access through the settings charm.", "sample", "status");
                    } else if (DeviceAccessStatus.deniedBySystem === accessInfo.currentStatus) {
                        WinJS.log && WinJS.log("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", "sample", "status");
                    } else if (DeviceAccessStatus.unspecified === accessInfo.currentStatus) {
                        WinJS.log && WinJS.log("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", "sample", "status");
                    }
                } else {
                    // if ex.number is RPC_E_DISCONNECTED (0x80010108):
                    // The Location Framework service event state is out of synchronization
                    // with the Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.
                    // To recover remove all event handlers on the GeofenceMonitor or restart the application.
                    // Once all event handlers are removed you may add back any event handlers and retry the operation.
                    WinJS.log && WinJS.log(ex.toString(), "sample", "error");
                }
            } finally {
                pageLoaded = true;
            }
        },
        unload: function () {
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.removeEventListener("geofencestatechanged", onGeofenceStateChanged);
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.removeEventListener("statuschanged", onGeofenceStatusChanged);
            pageLoaded = false;
            if (inGetPositionAsync) {
                promise.operation.cancel();
            }
        }
    });

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        if (eventObject.detail.location !== "/html/scenario4.html") {
            // unregister from geofence events
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.removeEventListener("geofencestatechanged", onGeofenceStateChanged);
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.removeEventListener("statuschanged", onGeofenceStatusChanged);

            // save off the contents of the event collection
            saveExistingEvents();
        }
    });

    function onRegisteredGeofenceListViewSelectionChanged(eventInfo) {
        // update controls with the values from this geofence item
        // get selected item
        registeredGeofenceListView.selection.getItems().done(function completed(result) {
            if (0 !== result.length) {
                // enable the remove button
                document.getElementById("removeGeofenceItem").disabled = false;

                var item = result[0].data;

                refreshControlsFromGeofenceItem(item);

                document.getElementById("createGeofenceButton").disabled = !settingsAvailable();
            } else {
                // disable the remove button
                document.getElementById("removeGeofenceItem").disabled = true;
            }
        });
    }

    function onSetPositionToHere() {
        // set cursor to a wait cursor
        document.body.style.cursor = 'wait';
        setPositionToHere.style.cursor = 'wait';
        latitude.style.cursor = 'wait';
        longitude.style.cursor = 'wait';
        setPositionToHere.disabled = true;
        latitude.disabled = true;
        longitude.disabled = true;

        promise = geolocator.getGeopositionAsync();
        promise.done(
            function (pos) {
                var coord = pos.coordinate;

                // restore cursor and re-enable controls
                document.body.style.cursor = 'default';
                setPositionToHere.style.cursor = 'default';
                latitude.style.cursor = 'default';
                longitude.style.cursor = 'default';
                setPositionToHere.disabled = false;
                latitude.disabled = false;
                longitude.disabled = false;

                setLatitudeInUI(coord.point.position.latitude);
                setLongitudeInUI(coord.point.position.longitude);

                // clear status
                WinJS.log && WinJS.log("", "sample", "status");
            },
            function (err) {
                if (pageLoaded) {

                    // restore cursor and re-enable controls
                    document.body.style.cursor = 'default';
                    setPositionToHere.style.cursor = 'default';
                    latitude.style.cursor = 'default';
                    longitude.style.cursor = 'default';
                    setPositionToHere.disabled = false;
                    latitude.disabled = false;
                    longitude.disabled = false;

                    WinJS.log && WinJS.log(err.message, "sample", "error");
                }
            }
        );
    }

    function fillRegisteredGeofenceListViewWithExistingGeofences() {
        geofences.forEach(createGeofenceItemAndAddToListView);
    }

    function createGeofenceItemAndAddToListView(fence) {
        var item = new GeofenceItem(fence);

        registeredGeofenceData.unshift(item);
    }

    function onGeofenceStateChanged(args) {
        try {
            args.target.readReports().forEach(processReport);
        } catch (ex) {
            WinJS.log && WinJS.log(ex.toString(), "sample", "error");
        }
    }

    function processReport(report) {
        var state = report.newState;

        geofence = report.geofence;
        var eventDescription = getTimeStampedMessage(geofence.id);

        if (state === Windows.Devices.Geolocation.Geofencing.GeofenceState.removed) {
            var reason = report.removalReason;

            if (reason === Windows.Devices.Geolocation.Geofencing.GeofenceRemovalReason.expired) {
                eventDescription += " (Removed/Expired)";
            } else if (reason === Windows.Devices.Geolocation.Geofencing.GeofenceRemovalReason.used) {
                eventDescription += " (Removed/Used)";
            }

            // remove the geofence from the client side geofences collection
            removeGeofence(geofence);

            // empty the registered geofence listbox and repopulate
            while (registeredGeofenceData.length > 0) {
                registeredGeofenceData.pop();
            }

            fillRegisteredGeofenceListViewWithExistingGeofences();
        } else if (state === Windows.Devices.Geolocation.Geofencing.GeofenceState.entered ||
                   state === Windows.Devices.Geolocation.Geofencing.GeofenceState.exited) {
            // NOTE: You might want to write your app to take particular
            // action based on whether the app has internet connectivity.

            if (state === Windows.Devices.Geolocation.Geofencing.GeofenceState.entered) {
                eventDescription += " (Entered)";
            } else if (state === Windows.Devices.Geolocation.Geofencing.GeofenceState.exited) {
                eventDescription += " (Exited)";
            }
        } else {
            eventDescription += " (Unknown)";
        }

        addEventDescription(eventDescription);
    }

    function onGeofenceStatusChanged(args) {
        try {
            var eventDescription = getTimeStampedMessage("Geofence Status");
            var item = null;
            var geofenceStatus = args.target.status;

            if (Windows.Devices.Geolocation.Geofencing.GeofenceMonitorStatus.ready === geofenceStatus) {
                eventDescription += " (Ready)";
            } else if (Windows.Devices.Geolocation.Geofencing.GeofenceMonitorStatus.initializing === geofenceStatus) {
                eventDescription += " (Initializing)";
            } else if (Windows.Devices.Geolocation.Geofencing.GeofenceMonitorStatus.noData === geofenceStatus) {
                eventDescription += " (NoData)";
            } else if (Windows.Devices.Geolocation.Geofencing.GeofenceMonitorStatus.disabled === geofenceStatus) {
                eventDescription += " (Disabled)";
            } else if (Windows.Devices.Geolocation.Geofencing.GeofenceMonitorStatus.notInitialized === geofenceStatus) {
                eventDescription += " (NotInitialized)";
            } else if (Windows.Devices.Geolocation.Geofencing.GeofenceMonitorStatus.notAvailable === geofenceStatus) {
                eventDescription += " (NotAvailable)";
            } else {
                eventDescription += " (Unknown)";
            }

            addEventDescription(eventDescription);
        } catch (ex) {
            WinJS.log && WinJS.log(ex.toString(), "sample", "error");
        }
    }

    function onVisibilityChanged() {
        // NOTE: After the app is no longer visible on the screen and before the app is suspended
        // you might want your app to use toast notification for any geofence activity.
        // By registering for VisibiltyChanged the app is notified when the app is no longer visible in the foreground.

        if (document.msVisibilityState === "visible") {
            // register for foreground events
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.addEventListener("geofencestatechanged", onGeofenceStateChanged);
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.addEventListener("statuschanged", onGeofenceStatusChanged);
        } else {
            // unregister foreground events (let background capture events)
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.removeEventListener("geofencestatechanged", onGeofenceStateChanged);
            Windows.Devices.Geolocation.Geofencing.GeofenceMonitor.current.removeEventListener("statuschanged", onGeofenceStatusChanged);
        }
    }

    function onAccessChanged(args) {
        var eventDescription = getTimeStampedMessage("Device Access Status");
        var item = null;

        if (DeviceAccessStatus.deniedByUser === args.status) {
            eventDescription += " (DeniedByUser)";

            WinJS.log && WinJS.log("Location has been disabled by the user. Enable access through the settings charm.", "sample", "status");
        } else if (DeviceAccessStatus.deniedBySystem === args.status) {
            eventDescription += " (DeniedBySystem)";

            WinJS.log && WinJS.log("Location has been disabled by the system. The administrator of the device must enable location access through the location control panel.", "sample", "status");
        } else if (DeviceAccessStatus.unspecified === args.status) {
            eventDescription += " (Unspecified)";

            WinJS.log && WinJS.log("Location has been disabled by unspecified source. The administrator of the device may need to enable location access through the location control panel, then enable access through the settings charm.", "sample", "status");
        } else if (DeviceAccessStatus.allowed === args.status) {
            eventDescription += " (Allowed)";

            // clear status
            WinJS.log && WinJS.log("", "sample", "status");
        } else {
            eventDescription += " (Unknown)";

            WinJS.log && WinJS.log("Unknown device access information status", "sample", "status");
        }

        addEventDescription(eventDescription);
    }

    // add geofence to listview
    function addGeofenceToRegisteredGeofenceListView() {
        // call method that adds element to start of list
        // push adds element to end of list
        registeredGeofenceData.unshift(geofenceItem);
    }

    function removeGeofence(geofenceToRemoveArg) {
        try {
            // IVector<T>.IndexOf returns two values - a Boolean return value and an index.
            // Multiple return values for JS projections are modeled as an object.
            var item = geofences.indexOf(geofenceToRemoveArg);

            if (item.returnValue) {
                geofences.removeAt(item.index);
            } else {
                var msg = "Could not find Geofence " + geofenceToRemoveArg.id + " in the geofences collection";
                WinJS.log && WinJS.log(msg, "sample", "status");
            }
        } catch (ex) {
            WinJS.log && WinJS.log(ex.toString(), "sample", "error");
        }
    }

    function onRemoveGeofenceItem() {
        try {
            registeredGeofenceListView.selection.getItems().done(function completed(result) {
                if (0 !== result.length) {
                    // get selected item
                    itemToRemove = result[0].data;

                    var geofenceToRemove = itemToRemove.geofenceStored();

                    // remove the geofence from the client side geofences collection
                    removeGeofence(geofenceToRemove);

                    // empty the registered geofence listbox and repopulate
                    while (registeredGeofenceData.length > 0) {
                        registeredGeofenceData.pop();
                    }

                    fillRegisteredGeofenceListViewWithExistingGeofences();
                }
            });
        } catch (ex) {
            WinJS.log && WinJS.log(ex.toString(), "sample", "error");
        }
    }

    function onCreateGeofence() {
        try {
            // This must be done here because there is no guarantee of 
            // getting the location consent from a geofence call.
            if (!permissionsChecked) {
                getGeoposition();
                permissionsChecked = true;
            }

            // get lat/long/radius, the fence name (fenceKey), 
            // and other properties from controls,
            // depending on data in controls for activation time
            // and duration the appropriate
            // constructor will be used.
            geofence = generateGeofence();

            if (geofence) {
                // Add the geofence to the GeofenceMonitor's
                // collection of fences
                geofences.push(geofence);

                geofenceItem = new GeofenceItem(geofence);

                if (geofenceItem) {
                    // add geofence to listview
                    addGeofenceToRegisteredGeofenceListView();
                }
            }
        } catch (ex) {
            WinJS.log && WinJS.log(ex.toString(), "sample", "error");
        }
    }

    function getGeoposition() {
        WinJS.log && WinJS.log("Checking permissions...", "sample", "status");

        inGetPositionAsync = true;

        promise = geolocator.getGeopositionAsync();
        promise.done(
            function (pos) {
                var coord = pos.coordinate;

                // clear status
                WinJS.log && WinJS.log("", "sample", "status");
            },
            function (err) {
                if (pageLoaded) {
                    WinJS.log && WinJS.log(err.message, "sample", "error");
                }
            }
        );

        inGetPositionAsync = false;
    }

    function fillEventListBoxWithExistingEvents() {
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        if (settings.values.hasKey("ForegroundGeofenceEventCollection")) {
            var geofenceEvent = settings.values["ForegroundGeofenceEventCollection"].toString();

            if (0 !== geofenceEvent.length) {
                var events = JSON.parse(geofenceEvent);

                // NOTE: the events are accessed in reverse order
                // because the events were added to JSON from
                // newer to older.  addEventDescription() adds
                // each new entry to the beginning of the collection.
                for (var pos = events.length - 1; pos >= 0; pos--) {
                    var element = events[pos].toString();
                    addEventDescription(element);
                }
            }

            settings.values["ForegroundGeofenceEventCollection"] = null;
        }
    }

    function saveExistingEvents() {
        var eventArray = new Array();

        eventsData.forEach(function (eventDescriptor) {
            eventArray.push(eventDescriptor);
        });

        var settings = Windows.Storage.ApplicationData.current.localSettings;
        settings.values["ForegroundGeofenceEventCollection"] = JSON.stringify(eventArray);
    }

    function addEventDescription(eventDescription) {
        if (eventsData.length === maxEventDescriptors) {
            eventsData.pop();
        }

        eventsData.unshift(eventDescription);
    }
})();

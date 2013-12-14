//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/S9-set-cookie.html", {
        ready: function (element, options) {
            document.getElementById("setCookieButton").addEventListener("click", setCookie, false);
        }
    });

    function setCookie() {
        var outputField = document.getElementById("outputField");
        var cookie = null;

        try {
            cookie = new Windows.Web.Http.HttpCookie(
                document.getElementById("nameField").value,
                document.getElementById("domainField").value,
                document.getElementById("pathField").value);

            cookie.value = document.getElementById("valueField").value;
        }
        catch (ex) {
            // Catch argument exceptions.
            WinJS.log && WinJS.log("Invalid argument: " + ex, "sample", "error");
            return;
        }

        try {
            if (document.getElementById("nullCheckBox").checked) {
                cookie.expires = null;
            } else {
                var currentDate = expiresDatePicker.winControl.current;
                var currentTime = expiresTimePicker.winControl.current;
                currentDate.setHours(currentTime.getHours());
                currentDate.setMinutes(currentTime.getMinutes());
                cookie.expires = currentDate;
            }

            cookie.secure = document.getElementById("secureToggle").winControl.checked;
            cookie.httpOnly = document.getElementById("httpOnlyToggle").winControl.checked;

            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            var replaced = filter.cookieManager.setCookie(cookie, false);

            if (replaced) {
                WinJS.log && WinJS.log("Cookie replaced.", "sample", "status");
            } else {
                WinJS.log && WinJS.log("Cookie set.", "sample", "status");
            }
        }
        catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
        }
    }
})();

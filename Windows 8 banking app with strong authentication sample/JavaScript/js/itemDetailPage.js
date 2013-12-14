(function () {
    "use strict";

    WinJS.UI.Pages.define("/html/itemDetailPage.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var /*@override*/ item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;

            // TODO: Initialize the page here.
        }
    });
})();

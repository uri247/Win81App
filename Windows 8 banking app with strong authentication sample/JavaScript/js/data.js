(function () {
    "use strict";

    var list = new WinJS.Binding.List();
    var groupedItems = list.createGrouped(
        function groupKeySelector(/*@override*/ item) { return item.group.key; },
        function groupDataSelector(/*@override*/ item) { return item.group; }
    );

    // TODO: Replace the data with your real data.
    // You can add data from asynchronous sources whenever it becomes available.
    generateSampleData().forEach(function (/*@override*/ item) {
        list.push(item);
    });

    WinJS.Namespace.define("Data", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemReference: getItemReference,
        getItemsFromGroup: getItemsFromGroup,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference
    });

    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(/*@override*/ item) {
        return [item.group.key, item.title];
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return list.createFiltered(function (/*@override*/item) { return item.group.key === group.key; });
    }

    // Get the unique group corresponding to the provided group key.
    function resolveGroupReference(key) {
	var itemObtained = null;
        for (var i = 0; i < groupedItems.groups.length; i++) {
            if (groupedItems.groups.getAt(i).key === key) {
                itemObtained = groupedItems.groups.getAt(i);
		break;
            }
        }
        
	return itemObtained;
    }

    // Get a unique item from the provided string array, which should contain a
    // group key and an item title.
    function resolveItemReference(reference) {
	var itemFound = null;

        for (var i = 0; i < groupedItems.length; i++) {
            itemFound = groupedItems.getAt(i);
            if (item.group.key === reference[0] && item.title === reference[1]) {
                break;
            }
        }

	return itemFound;
    }

    // Returns an array of sample data that can be added to the application's
    // data list. 
    function generateSampleData() {
        
        // These three strings encode placeholder images. You will want to set the
        // backgroundImage property in your real data to be URLs to images.
        var lightGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY7h4+cp/AAhpA3h+ANDKAAAAAElFTkSuQmCC";
        var mediumGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY5g8dcZ/AAY/AsAlWFQ+AAAAAElFTkSuQmCC";
        var darkGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY3B0cPoPAANMAcOba1BlAAAAAElFTkSuQmCC";

        // Groups are defined separately from items and must include a key to enable grouping of items into distinct groups. If the key is not unique, groups will be merged when displayed.
        var colors = ['rgba(144, 178, 41, 1)', 'rgba(240, 239, 136, 1)', 'rgba(191, 84, 46, 1)'];
        var sampleGroups = [
            {
                key: 'group' + 0,
                title: 'Sign In',
                backgroundImage: lightGray,
                description: 'Banking Application Scenario',
            },
            {
                key: 'group' + 1,
                title: 'Upgrade',
                backgroundImage: darkGray,
                description: 'Banking Application Scenario'
            },
        ];

        // Items are defined separately from groups and must include a group property that points to an actual group object.
        var sampleItems = [
            {
                group: sampleGroups[0],
                key: 'item0',
                title: 'Sign In to your account',
                subtitle: '',
                backgroundImage: lightGray,
                content: '<p>default</p>',
                description: 'Sign In to your account'
            },

            {
                group: sampleGroups[1], key: 'item1',
                title: 'Upgrade to stronger authentication',
                subtitle: '',
                backgroundImage: darkGray,
                content: '<p>default</p>',
                description: 'Upgrade to stronger authentication'
            },

        ];

        return sampleItems;
    }
})();

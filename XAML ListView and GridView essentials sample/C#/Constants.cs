//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using ListViewSimple;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "XAML ListView and GridView essentials";

        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Instantiating a GridView", ClassType = typeof(Scenario1) },
            new Scenario() { Title = "Responding to click events", ClassType = typeof(Scenario2) },
            new Scenario() { Title = "Instantiating a ListView", ClassType = typeof(Scenario3) },
            new Scenario() { Title = "Retemplating GridViewItems", ClassType = typeof(Scenario4) },
            new Scenario() { Title = "Retemplating ListViewItems", ClassType = typeof(Scenario5) }, 
            new Scenario() { Title = "Custom item container template structure", ClassType = typeof(Scenario6) }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}

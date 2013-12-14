//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Geolocation";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Track position", ClassType = typeof(Microsoft.Samples.Devices.Geolocation.Scenario1) },
            new Scenario() { Title = "Get position", ClassType = typeof(Microsoft.Samples.Devices.Geolocation.Scenario2) },
            new Scenario() { Title = "Get position in background task", ClassType = typeof(Microsoft.Samples.Devices.Geolocation.Scenario3) },
            new Scenario() { Title = "Foreground Geofencing", ClassType = typeof(Microsoft.Samples.Devices.Geolocation.Scenario4) },
            new Scenario() { Title = "Background Geofencing", ClassType = typeof(Microsoft.Samples.Devices.Geolocation.Scenario5) }
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

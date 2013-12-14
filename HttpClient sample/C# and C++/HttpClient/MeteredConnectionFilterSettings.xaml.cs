//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using HttpFilters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Microsoft.Samples.Networking.HttpClientSample
{
    public sealed partial class MeteredConnectionFilterSettings : UserControl
    {
        private HttpMeteredConnectionFilter meteredConnectionFilter;

        public MeteredConnectionFilterSettings(HttpMeteredConnectionFilter meteredConnectionFilter)
        {
            if (meteredConnectionFilter == null)
            {
                throw new ArgumentNullException("meteredConnectionFilter");
            }

            this.InitializeComponent();

            this.meteredConnectionFilter = meteredConnectionFilter;
            OptInSwitch.IsOn = meteredConnectionFilter.OptIn;
        }

        private void OptInSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            meteredConnectionFilter.OptIn = OptInSwitch.IsOn;
        }
    }
}


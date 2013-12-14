//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using SDKTemplate;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DisplayOrientation
{
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario3()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            screenOrientation.Text = DisplayInformation.GetForCurrentView().CurrentOrientation.ToString();
            DisplayInformation.GetForCurrentView().OrientationChanged += OnOrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DisplayInformation.GetForCurrentView().OrientationChanged -= OnOrientationChanged;
        }

        void OnOrientationChanged(DisplayInformation sender, object args)
        {
            screenOrientation.Text = sender.CurrentOrientation.ToString();
        }
    }
}

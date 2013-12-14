//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// MeteredConnectionFilterSettings.xaml.cpp
// Implementation of the MeteredConnectionFilterSettings class
//

#include "pch.h"
#include "MeteredConnectionFilterSettings.xaml.h"

using namespace SDKSample;

using namespace Platform;
using namespace HttpFilters;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

MeteredConnectionFilterSettings::MeteredConnectionFilterSettings(HttpMeteredConnectionFilter^ meteredConnectionFilter)
{
    if (meteredConnectionFilter == nullptr)
    {
        throw ref new InvalidArgumentException("meteredConnectionFilter");
    }

    InitializeComponent();

    this->meteredConnectionFilter = meteredConnectionFilter;
    OptInSwitch->IsOn = meteredConnectionFilter->OptIn;
}

void MeteredConnectionFilterSettings::OptInSwitch_Toggled(Object^ sender, RoutedEventArgs^ e)
{
    meteredConnectionFilter->OptIn = OptInSwitch->IsOn;
}

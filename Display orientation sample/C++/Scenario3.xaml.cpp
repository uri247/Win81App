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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample::DisplayOrientation;

using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
}

void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    screenOrientation->Text = DisplayInformation::GetForCurrentView()->CurrentOrientation.ToString();

    orientationChangedEventToken = DisplayInformation::GetForCurrentView()->OrientationChanged::add(
        ref new TypedEventHandler<DisplayInformation^, Platform::Object^>(this, &Scenario3::OnOrientationChanged)
        );
}

void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    DisplayInformation::GetForCurrentView()->OrientationChanged::remove(orientationChangedEventToken);
}


void Scenario3::OnOrientationChanged(_In_ DisplayInformation^ sender, _In_ Platform::Object^ args)
{
    screenOrientation->Text = sender->CurrentOrientation.ToString();
}

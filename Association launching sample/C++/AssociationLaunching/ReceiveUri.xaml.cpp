//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ReceiveUri.xaml.cpp
// Implementation of the ReceiveUri class
//

#include "pch.h"
#include "ReceiveUri.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AssociationLaunching;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ReceiveUri::ReceiveUri()
{
    InitializeComponent();
}

void ReceiveUri::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    auto rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // Display the result of the protocol activation if we got here as a result of being activated for a protocol.
    if (rootPage->ProtocolEvent != nullptr)
    {
        rootPage->NotifyUser("Protocol activation received. The received URI is " + rootPage->ProtocolEvent->Uri->AbsoluteUri + ".", NotifyType::StatusMessage);
    }
}

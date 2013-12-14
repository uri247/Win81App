//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::BluetoothRfcommChat;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Run Chat client", "SDKSample.BluetoothRfcommChat.S1_ChatClient" }, 
}; 

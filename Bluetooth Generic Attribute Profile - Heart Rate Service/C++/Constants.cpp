//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::BluetoothGattHeartRate;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Bluetooth Heart Rate Monitor - Device Eventing", "SDKSample.BluetoothGattHeartRate.S1_Eventing" }, 
    { "Bluetooth Heart Rate Monitor - Read Characteristic Value", "SDKSample.BluetoothGattHeartRate.S2_ReadCharacteristicValue" },
    { "Bluetooth Heart Rate Monitor - Write Characteristic Value", "SDKSample.BluetoothGattHeartRate.S3_WriteCharacteristicValue" }
}; 

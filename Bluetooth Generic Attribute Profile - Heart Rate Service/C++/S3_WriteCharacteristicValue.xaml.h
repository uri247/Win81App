//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3_WriteCharacteristicValue.xaml.h
// Declaration of the S3_WriteCharacteristicValue class
//

#pragma once
#include "S3_WriteCharacteristicValue.g.h"

namespace SDKSample
{
    namespace BluetoothGattHeartRate
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S3_WriteCharacteristicValue sealed
        {
        public:
            S3_WriteCharacteristicValue();

        protected:
            void LoadState(
                Platform::Object^ navigationParameter, 
                Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
            void SaveState(
                Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;

        private:
            void WriteCharacteristicValue_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Instance_ValueChangeCompleted(HeartRateMeasurement^ heartRateMeasurementValue);
            
            Windows::Foundation::EventRegistrationToken valueChangeCompletedRegistrationToken;
        };
    }
}

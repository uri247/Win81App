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
// Scenario6.xaml.h
// Declaration of the Scenario6 class
//

#pragma once

#include "pch.h"
#include "Scenario6.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario6 sealed
        {
        public:
            Scenario6();

        private:
            static Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ dateFormat;
            static Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ timeFormat;
            static Platform::String^ dateAccessedProperty;
            static Platform::String^ fileOwnerProperty;

            MainPage^ rootPage;

            void ShowPropertiesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}

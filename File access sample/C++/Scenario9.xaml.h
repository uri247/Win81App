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
// Scenario9.xaml.h
// Declaration of the Scenario9 class
//

#pragma once

#include "pch.h"
#include "Scenario9.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario9 sealed
        {
        public:
            Scenario9();

        private:
            MainPage^ rootPage;

            void CompareFilesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}

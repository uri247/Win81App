//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LaunchFile.xaml.h
// Declaration of the LaunchFile class
//

#pragma once
#include "LaunchFile.g.h"

namespace SDKSample
{
    namespace AssociationLaunching
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class LaunchFile sealed
        {
        public:
            LaunchFile();

        private:
            void LaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchFileWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchFileOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LaunchFileSplitScreenButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void PickAndLaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            static Windows::Foundation::Point GetOpenWithPosition(Windows::UI::Xaml::FrameworkElement^ element);
            static Platform::String^ fileToLaunch;
        };
    }
}

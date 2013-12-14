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
// MainPagePicker.xaml.h
// Declaration of the MainPagePicker.xaml class.
//

#pragma once

#include "pch.h"
#include "MainPagePicker.g.h"
#include "Common\LayoutAwarePage.h" // Required by generated header
#include "Constants.h"

namespace ContactPicker
{
    public ref class MainPagePickerSizeChangedEventArgs sealed
    {
    public:
        property double Width
        {
            double get()
            {
                return width;
            }

            void set(double value)
            {
                width = value;
            }
        }

    private:
        double width;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainPagePicker sealed
    {
    public:
        MainPagePicker();

        void NotifyUser(Platform::String^ strMessage, NotifyType type);
        void LoadScenario(Platform::String^ scenarioName);
        event Windows::Foundation::EventHandler<Platform::Object^>^ ScenarioLoaded;
        event Windows::Foundation::EventHandler<MainPagePickerSizeChangedEventArgs^>^ MainPagePickerResized;


    private:
        void SetContactPicker(Platform::String^ strFeature);
        void PopulateScenarios();
        void InvalidateSize();
        void InvalidateLayout();
        ~MainPagePicker();

        Platform::Collections::Vector<Object^>^ ScenarioList;
        Windows::UI::Xaml::Controls::Frame^ HiddenFrame;
        void Footer_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        void MainPagePicker_SizeChanged(Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
        void Scenarios_SelectionChanged(Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

    internal:
        static MainPagePicker^ Current;
        Windows::ApplicationModel::Contacts::Provider::ContactPickerUI^ contactPickerUI;
    };

}

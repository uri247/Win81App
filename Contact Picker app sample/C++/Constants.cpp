//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "MainPagePicker.xaml.h"
#include "Constants.h"

using namespace ContactPicker;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Contacts::Provider;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Pick a single contact", "ContactPicker.ScenarioSingle" },
    { "Pick multiple contacts", "ContactPicker.ScenarioMultiple" },
};

Platform::Array<Scenario>^ MainPagePicker::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Selection contact(s)", "ContactPicker.ContactPickerPage" }
};

void MainPagePicker::Activate(ContactPickerActivatedEventArgs^ args)
{
    // cache ContactPickerUI
    contactPickerUI = args->ContactPickerUI;
    Window::Current->Content = this;
    this->OnNavigatedTo(nullptr);
    Window::Current->Activate();
}

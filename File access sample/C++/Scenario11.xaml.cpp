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
// Scenario11.xaml.cpp
// Implementation of the Scenario8 class
//

#include "pch.h"
#include "Scenario11.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario11::Scenario11()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    rootPage->Initialize();
    GetFileButton->Click += ref new RoutedEventHandler(this, &Scenario11::GetFileButton_Click);
}

void Scenario11::GetFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    // Gets a file without throwing an exception
    create_task(KnownFolders::PicturesLibrary->TryGetItemAsync("sample.dat")).then([this](IStorageItem^ item)
    {
        if (item != nullptr)
        {
            OutputTextBlock->Text = "Operation result: " + item->Name;
        }
        else
        {
            OutputTextBlock->Text = "Operation result: null";
        }
    });
}

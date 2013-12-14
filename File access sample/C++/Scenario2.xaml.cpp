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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    rootPage->Initialize();
    rootPage->ValidateFile();
    GetParentButton->Click += ref new RoutedEventHandler(this, &Scenario2::GetParentButton_Click);
}

/// <summary>
/// Gets the file's parent folder
/// </summary>
void Scenario2::GetParentButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ sampleFile = rootPage->SampleFile;
    if (sampleFile != nullptr)
    {
        create_task(sampleFile->GetParentAsync()).then([this, sampleFile](StorageFolder^ parentFolder)
        {
            if (parentFolder != nullptr)
            {
                OutputTextBlock->Text += "Item: " + sampleFile->Name + " (" + sampleFile->Path + ")\n";
                OutputTextBlock->Text += "Parent: " + parentFolder->Name + " (" + parentFolder->Path + ")\n";
            }
        });
    }
    else
    {
        rootPage->NotifyUserFileNotExist();
    }
}

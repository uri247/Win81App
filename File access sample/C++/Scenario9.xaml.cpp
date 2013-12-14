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
// Scenario9.xaml.cpp
// Implementation of the Scenario9 class
//

#include "pch.h"
#include "Scenario9.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario9::Scenario9()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    rootPage->Initialize();
    rootPage->ValidateFile();
    CompareFilesButton->Click += ref new RoutedEventHandler(this, &Scenario9::CompareFilesButton_Click);
}

void Scenario9::CompareFilesButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ sampleFile = rootPage->SampleFile;
    if (sampleFile != nullptr)
    {
        // Compares a picked file with sample.dat
        FileOpenPicker^ picker = ref new FileOpenPicker();
        picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
        picker->FileTypeFilter->Append("*");
        create_task(picker->PickSingleFileAsync()).then([this, sampleFile](StorageFile^ comparand)
        {
            if (comparand != nullptr)
            {
                if (sampleFile->IsEqual(comparand))
                {
                    OutputTextBlock->Text = "Files are equal";
                }
                else
                {
                    OutputTextBlock->Text = "Files are not equal";
                }
            }
            else
            {
                OutputTextBlock->Text = "Operation cancelled";
            }
        });
    }
    else
    {
        rootPage->NotifyUserFileNotExist();
    }
}

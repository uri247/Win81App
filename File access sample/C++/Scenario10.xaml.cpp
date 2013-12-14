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
// Scenario10.xaml.cpp
// Implementation of the Scenario10 class
//

#include "pch.h"
#include "Scenario10.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario10::Scenario10()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    rootPage->Initialize();
    rootPage->ValidateFile();
    DeleteFileButton->Click += ref new RoutedEventHandler(this, &Scenario10::DeleteFileButton_Click);
}

void Scenario10::DeleteFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ sampleFile = rootPage->SampleFile;
    if (sampleFile != nullptr)
    {
        String^ filename = sampleFile->Name;
        // Deletes the file
        create_task(sampleFile->DeleteAsync()).then([this, filename](task<void> task)
        {
            try
            {
                task.get();
                rootPage->SampleFile = nullptr;
                OutputTextBlock->Text = "The file '" + filename + "' was deleted";
            }
            catch (COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        });
    }
    else
    {
    rootPage->NotifyUserFileNotExist();
    }
}

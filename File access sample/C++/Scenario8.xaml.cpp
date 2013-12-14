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
// Scenario8.xaml.cpp
// Implementation of the Scenario8 class
//

#include "pch.h"
#include "Scenario8.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario8::Scenario8()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    rootPage->Initialize();
    rootPage->ValidateFile();
    CopyFileButton->Click += ref new RoutedEventHandler(this, &Scenario8::CopyFileButton_Click);
}

void Scenario8::CopyFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        // Get the returned file and copy it
        StorageFolder^ picturesFolder = KnownFolders::PicturesLibrary;
        create_task(file->CopyAsync(picturesFolder, "sample - Copy.dat", NameCollisionOption::ReplaceExisting)).then([this, file](task<StorageFile^> task)
        {
            try
            {
                StorageFile^ sampleFileCopy = task.get();
                OutputTextBlock->Text = "The file '" + file->Name + "' was copied and the new file was named '" + sampleFileCopy->Name + "'.";
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

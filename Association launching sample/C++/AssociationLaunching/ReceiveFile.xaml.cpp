//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ReceiveFile.xaml.cpp
// Implementation of the ReceiveFile class
//

#include "pch.h"
#include "ReceiveFile.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::AssociationLaunching;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;

ReceiveFile::ReceiveFile()
{
    InitializeComponent();
}

void ReceiveFile::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    auto rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // Display the result of the file activation if we got here as a result of being activated for a file.
    if (rootPage->FileEvent != nullptr)
    {
        String^ output = "File activation received. The number of files received is " + rootPage->FileEvent->Files->Size + ". The received files are:\n";
        for (unsigned int i = 0; i < rootPage->FileEvent->Files->Size; i++)
        {
            output = output + rootPage->FileEvent->Files->GetAt(i)->Name + "\n";
        }

        rootPage->NotifyUser(output, NotifyType::StatusMessage);
   }
}


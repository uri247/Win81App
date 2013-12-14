//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::MediaExtensions;

using namespace Platform;

using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

using namespace concurrency;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Install a local decoder", "SDKSample.MediaExtensions.LocalDecoder" }, 
    { "Install a local scheme handler", "SDKSample.MediaExtensions.LocalSchemeHandler" },
    { "Install the built-in Video Stabilization Effect", "SDKSample.MediaExtensions.VideoStabilizationEffect" },
    { "Install a custom Video Effect", "SDKSample.MediaExtensions.CustomEffects" }
}; 

//
//  Open a single file picker [with fileTypeFilter].
//  And then, call media.SetSource(picked file).
//  If the file is successfully opened, VideoMediaOpened() will be called and call media.Play().
//
void SampleUtilities::PickSingleFileAndSet(IVector<String^>^ fileTypeFilters, IVector<MediaElement^>^ mediaElements)
{
    auto picker = ref new Pickers::FileOpenPicker();
    auto dispatcher = Window::Current->Dispatcher;
    picker->SuggestedStartLocation = Pickers::PickerLocationId::VideosLibrary;
    for (unsigned int index = 0; index < fileTypeFilters->Size; ++index)
    {
        picker->FileTypeFilter->Append(fileTypeFilters->GetAt(index));
    }

    task<StorageFile^> (picker->PickSingleFileAsync()).then(
        [mediaElements](StorageFile^ file)
    {
        if (file)
        {
            auto contentType = file->ContentType;
            task<Streams::IRandomAccessStream^> (file->OpenAsync(FileAccessMode::Read)).then(
                [contentType, mediaElements](Streams::IRandomAccessStream^ strm)
            {
                {
                    for (unsigned int i = 0; i < mediaElements->Size; ++i)
                    {
                        MediaElement^ media = mediaElements->GetAt(i);
                        media->Stop();

                        if (i + 1 < mediaElements->Size)
                        {
                            media->SetSource(strm->CloneStream(), contentType);
                        }
                        else
                        {
                            media->SetSource(strm, contentType);
                        }
                    }
                }
            });
        }
    });
}

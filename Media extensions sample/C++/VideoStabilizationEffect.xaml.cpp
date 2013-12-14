//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// VideoStabilizationEffect.xaml.cpp
// Implementation of the VideoStabilizationEffect class
//

#include "pch.h"
#include "VideoStabilizationEffect.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MediaExtensions;

using namespace Platform;
using namespace Platform::Collections;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

VideoStabilizationEffect::VideoStabilizationEffect()
{
    InitializeComponent();
}

void VideoStabilizationEffect::Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    VideoStabilized->RemoveAllEffects();
    VideoStabilized->AddVideoEffect(Windows::Media::VideoEffects::VideoStabilization, true, nullptr);

    auto v = ref new Vector<String^>();
    v->Append(".mp4");
    v->Append(".wmv");
    v->Append(".avi");
    auto me = ref new Vector<MediaElement^>();
    me->Append(Video);
    me->Append(VideoStabilized);
    SampleUtilities::PickSingleFileAndSet(v, me);
}

void VideoStabilizationEffect::Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->Source = nullptr;
    VideoStabilized->Source = nullptr;
}

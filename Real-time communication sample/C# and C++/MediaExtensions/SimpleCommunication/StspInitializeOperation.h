//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <wrl\async.h>
#include <StspAsyncBase.h>

#include "microsoft.samples.simplecommunication.h"

namespace Stsp {
class CMediaSink;

typedef ABI::Windows::Foundation::IAsyncAction IInitializeOperation;
typedef ABI::Windows::Foundation::IAsyncActionCompletedHandler IInitializeCompletedHandler;

extern __declspec(selectany) const WCHAR MediaSinkInitializeAsyncOperationName[] = L"Windows.Foundation.IAsyncAction Stsp.CMediaSink.InitializeAsync";
class CInitializeOperation :
    public CAsyncBase<IInitializeOperation, IInitializeCompletedHandler, MediaSinkInitializeAsyncOperationName>
{
    InspectableClass(RuntimeClass_Microsoft_Samples_SimpleCommunication_InitializeOperation, BaseTrust);

public:
    CInitializeOperation();
    ~CInitializeOperation();

    HRESULT RuntimeClassInitialize(
        CMediaSink *pMediaSink, 
        ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *audioEncodingProperties, 
        ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *videoEncodingProperties);

    // AsyncBase
    HRESULT OnStart(void);
    void OnClose(void);
    void OnCancel(void);

    ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *GetAudioEncodingProperties() {return _spAudioEncodingProperties.Get();}
    ABI::Windows::Media::MediaProperties::IMediaEncodingProperties *GetVideoEncodingProperties() {return _spVideoEncodingProperties.Get();}

private:
    ComPtr<ABI::Windows::Media::MediaProperties::IMediaEncodingProperties> _spAudioEncodingProperties;
    ComPtr<ABI::Windows::Media::MediaProperties::IMediaEncodingProperties> _spVideoEncodingProperties;
    ComPtr<CMediaSink> _spSink;
};
}
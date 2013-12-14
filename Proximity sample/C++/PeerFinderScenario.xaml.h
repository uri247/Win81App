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
// PeerFinderScenario.xaml.h
// Declaration of the PeerFinderScenario class
//

#pragma once

#include "pch.h"
#include "PeerFinderScenario.g.h"
#include "MainPage.xaml.h"


namespace ProximityCPP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PeerFinderScenario sealed
    {
    public:
        PeerFinderScenario();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;
    private:

        void TriggeredConnectionStateChangedEventHandler(Object^ sender, Windows::Networking::Proximity::TriggeredConnectionStateChangedEventArgs^ e);
        void ConnectionRequestedEventHandler(Object^ sender, Windows::Networking::Proximity::ConnectionRequestedEventArgs^ e);
        void PeerFinder_StartAdvertising(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_BrowsePeers(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        
        void PeerFinder_Accept(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Connect(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Send(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void SocketErrorHandler(SocketHelper^ sender, Platform::String^ errMessage);
        void MessageHandler(SocketHelper^ sender, Platform::String^ message);

        void PeerFinderInit();
        void PeerFinderReset();
        void Scenario2Reset();
        void PeerFinder_StartSendReceive(Windows::Networking::Sockets::StreamSocket^ socket, Windows::Networking::Proximity::PeerInformation^ peerInformation);

        //members
        MainPage^ m_rootPage;
        Windows::UI::Core::CoreDispatcher^ m_coreDispatcher;
        bool m_peerFinderStarted;

        Windows::Foundation::Collections::IVectorView<Windows::Networking::Proximity::PeerInformation^>^ m_peerInformationList;
        Windows::Networking::Proximity::PeerInformation^ m_requestingPeer;
        
        bool m_triggeredConnectSupported;
        bool m_browseConnectSupported;
        bool m_startPeerFinderImmediately;
        bool m_fLaunchByTap;

        Windows::Foundation::EventRegistrationToken m_triggerToken;
        Windows::Foundation::EventRegistrationToken m_connectionRequestedToken;

        SocketHelper^ m_socketHelper;		
    };
}

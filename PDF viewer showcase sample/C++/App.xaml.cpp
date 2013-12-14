//
// App.xaml.cpp
// Implementation of the App class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace PdfShowcase;

using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

/// <summary>
/// Initializes the singleton application object.  This is the first line of authored code
/// executed, and as such is the logical equivalent of main() or WinMain().
/// </summary>
App::App()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when the application is launched normally by the end user.  Other entry points
/// will be used when the application is launched to open a specific file, to display
/// search results, and so forth.
/// </summary>
/// <param name="args">Details about the launch request and process.</param>
void App::OnLaunched(_In_ LaunchActivatedEventArgs^ args)
{
	Window::Current->Content = ref new MainPage();
	Window::Current->Activate();

	Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
}

void App::OnSuspending(
	_In_ Object^ sender,
	_In_ SuspendingEventArgs^ args
	)
{
	// This is a good time to save your application's state in case the process gets terminated.
	auto mainPage = dynamic_cast<MainPage^>(Window::Current->Content);
	mainPage->OnSuspending(sender, args);
}

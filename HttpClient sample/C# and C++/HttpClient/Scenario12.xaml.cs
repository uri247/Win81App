//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using HttpFilters;
using SDKTemplate;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario12 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Popup settingsPopup;

        private HttpMeteredConnectionFilter meteredConnectionFilter;
        private HttpClient httpClient;
        private CancellationTokenSource cts;

        public Scenario12()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

            HttpBaseProtocolFilter baseProtocolFilter = new HttpBaseProtocolFilter();
            meteredConnectionFilter = new HttpMeteredConnectionFilter(baseProtocolFilter);
            httpClient = new HttpClient(meteredConnectionFilter);
            cts = new CancellationTokenSource();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SettingsPane.GetForCurrentView().CommandsRequested -= OnCommandsRequested;
            Dispose();
        }

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand settingsCommand = new SettingsCommand("filter", "Metered Connection Filter", (command) =>
            {
                const double settingsWidth = 400;

                settingsPopup = new Popup();
                settingsPopup.Closed += OnPopupClosed;
                settingsPopup.IsLightDismissEnabled = true;
                Window.Current.Activated += OnWindowActivated;

                MeteredConnectionFilterSettings pane = new MeteredConnectionFilterSettings(meteredConnectionFilter);
                pane.Width = settingsWidth;
                pane.Height = Window.Current.Bounds.Height;

                settingsPopup.Child = pane;
                settingsPopup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - settingsWidth);
                settingsPopup.SetValue(Canvas.TopProperty, 0);
                settingsPopup.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(settingsCommand);
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                settingsPopup.IsOpen = false;
            }
        }

        private void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            Helpers.ScenarioStarted(StartButton, CancelButton, OutputField);
            rootPage.NotifyUser("In progress", NotifyType.StatusMessage);

            try
            {
                // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
                // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
                Uri resourceAddress = new Uri(AddressField.Text);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, resourceAddress);

                MeteredConnectionPriority priority = MeteredConnectionPriority.Low;
                if (MediumRadio.IsChecked.Value)
                {
                    priority = MeteredConnectionPriority.Medium;
                }
                else if (HighRadio.IsChecked.Value)
                {
                    priority = MeteredConnectionPriority.High;
                }
                request.Properties["meteredConnectionPriority"] = priority;

                HttpResponseMessage response = await httpClient.SendRequestAsync(request).AsTask(cts.Token);

                await Helpers.DisplayTextResultAsync(response, OutputField, cts.Token);

                rootPage.NotifyUser("Completed", NotifyType.StatusMessage);
            }
            catch (TaskCanceledException)
            {
                rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Error: " + ex.Message, NotifyType.ErrorMessage);
            }
            finally
            {
                Helpers.ScenarioCompleted(StartButton, CancelButton);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            cts.Dispose();

            // Re-create the CancellationTokenSource.
            cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (meteredConnectionFilter != null)
            {
                meteredConnectionFilter.Dispose();
                meteredConnectionFilter = null;
            }
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }

            if (cts != null)
            {
                cts.Dispose();
                cts = null;
            }
        }
    }
}

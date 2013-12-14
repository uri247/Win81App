//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario9 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private HttpClient httpClient;
        private CancellationTokenSource cts;

        public Scenario9()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Helpers.CreateHttpClient(ref httpClient);
            cts = new CancellationTokenSource();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        private void GetCookies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
                // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
                Uri uri = new Uri(AddressField.Text);

                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(uri);

                OutputField.Text = cookieCollection.Count + " cookies found.\r\n";
                foreach (HttpCookie cookie in cookieCollection)
                {
                    OutputField.Text += "--------------------\r\n";
                    OutputField.Text += "Name: " + cookie.Name + "\r\n";
                    OutputField.Text += "Domain: " + cookie.Domain + "\r\n";
                    OutputField.Text += "Path: " + cookie.Path + "\r\n";
                    OutputField.Text += "Value: " + cookie.Value + "\r\n";
                    OutputField.Text += "Expires: " + cookie.Expires + "\r\n";
                    OutputField.Text += "Secure: " + cookie.Secure + "\r\n";
                    OutputField.Text += "HttpOnly: " + cookie.HttpOnly + "\r\n";
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Error: " + ex.Message, NotifyType.ErrorMessage);
            }
        }

        private async void SendHttpGetButton_Click(object sender, RoutedEventArgs e)
        {
            Helpers.ScenarioStarted(SendHttpGetButton, CancelButton, OutputField);
            rootPage.NotifyUser("In progress", NotifyType.StatusMessage);

            try
            {
                // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
                // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
                Uri resourceAddress = new Uri(AddressField.Text);

                HttpResponseMessage response = await httpClient.GetAsync(resourceAddress).AsTask(cts.Token);

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
                Helpers.ScenarioCompleted(SendHttpGetButton, CancelButton);
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

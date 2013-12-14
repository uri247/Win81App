//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Foundation;
using SDKTemplate;
using System;

namespace AssociationLaunching
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchFile : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page. This is needed if you want to call methods in MainPage such as NotifyUser().
        MainPage rootPage = MainPage.Current;
        string fileToLaunch = @"Assets\Icon.Targetsize-256.png";

        public LaunchFile()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        // Launch a .png file that came with the package.
        /// </summary>
        private async void LaunchFileButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get the image file from the package's image directory.
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch);

            // Next, launch the file.
            bool success = await Windows.System.Launcher.LaunchFileAsync(file);
            if (success)
            {
                rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        // Launch a .png file that came with the package. Show a warning prompt.
        /// </summary>
        private async void LaunchFileWithWarningButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get the image file from the package's image directory.
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch);

            // Next, configure the warning prompt.
            var options = new Windows.System.LauncherOptions();
            options.TreatAsUntrusted = true;

            // Finally, launch the file.
            bool success = await Windows.System.Launcher.LaunchFileAsync(file, options);
            if (success)
            {
                rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        // Launch a .png file that came with the package. Show an Open With dialog that lets the user chose the handler to use.
        /// </summary>
        private async void LaunchFileOpenWithButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get the image file from the package's image directory.
            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileToLaunch);

            // Calculate the position for the Open With dialog.
            // An alternative to using the point is to set the rect of the UI element that triggered the launch.
            Point openWithPosition = GetOpenWithPosition(LaunchFileOpenWithButton);

            // Next, configure the Open With dialog.
            var options = new Windows.System.LauncherOptions();
            options.DisplayApplicationPicker = true;
            options.UI.InvocationPoint = openWithPosition;
            options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below;

            // Finally, launch the file.
            bool success = await Windows.System.Launcher.LaunchFileAsync(file, options);
            if (success)
            {
                rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        // Launch a .png file that came with the package. Request to share the screen with the launched app.
        /// </summary>
        private async void LaunchFileSplitScreenButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get a file via the picker.
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Configure the request for split screen launch.
                var options = new Windows.System.LauncherOptions();
                if (Default.IsSelected == true)
                {
                    options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.Default;
                }
                else if (UseLess.IsSelected == true)
                {
                    options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseLess;
                }
                else if (UseHalf.IsSelected == true)
                {
                    options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseHalf;
                }
                else if (UseMore.IsSelected == true)
                {
                    options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMore;
                }
                else if (UseMinimum.IsSelected == true)
                {
                    options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMinimum;
                }
                else if (UseNone.IsSelected == true)
                {
                    options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseNone;
                }

                // Next, launch the file.
                bool success = await Windows.System.Launcher.LaunchFileAsync(file, options);
                if (success)
                {
                    rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        // Have the user pick a file, then launch it.
        /// </summary>
        private async void PickAndLaunchFileButton_Click(object sender, RoutedEventArgs e)
        {
            // First, get a file via the picker.
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Next, launch the file.
                bool success = await Windows.System.Launcher.LaunchFileAsync(file);
                if (success)
                {
                    rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        // The Open With dialog should be displayed just under the element that triggered it.
        /// </summary>
        private Windows.Foundation.Point GetOpenWithPosition(FrameworkElement element)
        {
            Windows.UI.Xaml.Media.GeneralTransform buttonTransform = element.TransformToVisual(null);

            Point desiredLocation = buttonTransform.TransformPoint(new Point());
            desiredLocation.Y = desiredLocation.Y + element.ActualHeight;

            return desiredLocation;
        }
    }
}

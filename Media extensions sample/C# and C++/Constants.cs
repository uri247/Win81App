//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using MediaExtensions;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Media Extensions";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Install a local decoder", ClassType = typeof(CustomDecoder) },
            new Scenario() { Title = "Install a local scheme handler", ClassType = typeof(SchemeHandler) },
            new Scenario() { Title = "Install the built in Video Stabilization Effect", ClassType = typeof(VideoStabilization) },
            new Scenario() { Title = "Install a custom Video Effect", ClassType = typeof(VideoEffect) }
        };

        private MediaExtensionManager _extensionManager = new MediaExtensionManager();

        public MediaExtensionManager ExtensionManager
        {
            get { return _extensionManager; }
        }

        //
        //  Open a single file picker [with fileTypeFilter].
        //  And then, call media.SetSource(picked file).
        //  If the file is successfully opened, VideoMediaOpened() will be called and call media.Play().
        //
        public async void PickSingleFileAndSet(string[] fileTypeFilter, params MediaElement[] mediaElements)
        {
            CoreDispatcher dispatcher = Window.Current.Dispatcher;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            foreach (string filter in fileTypeFilter)
            {
                picker.FileTypeFilter.Add(filter);
            }
            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);

                    for (int i = 0; i < mediaElements.Length; ++i)
                    {
                        MediaElement me = mediaElements[i];
                        me.Stop();
                        if (i + 1 < mediaElements.Length)
                        {
                            me.SetSource(stream.CloneStream(), file.ContentType);
                        }
                        else
                        {
                            me.SetSource(stream, file.ContentType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotifyUser("Cannot open video file - error: " + ex.Message, NotifyType.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Common video failed error handler.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void VideoOnError(Object obj, ExceptionRoutedEventArgs args)
        {
            NotifyUser("Cannot open video file - error: " + args.ErrorMessage, NotifyType.ErrorMessage);
        }
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}

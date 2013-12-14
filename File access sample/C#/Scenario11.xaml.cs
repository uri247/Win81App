//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;
using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario11 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario11()
        {
            this.InitializeComponent();
            GetFileButton.Click += new RoutedEventHandler(GetFileButton_Click);
        }

        /// <summary>
        /// Gets a file without throwing an exception
        /// </summary>
        private async void GetFileButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.ResetScenarioOutput(OutputTextBlock);
            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            StorageFile file = await storageFolder.TryGetItemAsync("sample.dat") as StorageFile;
            if (file != null)
            {
                OutputTextBlock.Text = "Operation result: " + file.Name;
            }
            else
            {
                OutputTextBlock.Text = "Operation result: null";
            }
        }
    }
}

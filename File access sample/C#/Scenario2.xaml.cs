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
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
            rootPage.ValidateFile();
            GetParentButton.Click += new RoutedEventHandler(GetParent_Click);
        }

        /// <summary>
        /// Gets the file's parent folder
        /// </summary>
        private async void GetParent_Click(object sender, RoutedEventArgs e)
        {
            rootPage.ResetScenarioOutput(OutputTextBlock);
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                StorageFolder parentFolder = await file.GetParentAsync();
                if (parentFolder != null)
                {
                    OutputTextBlock.Text += "Item: " + file.Name + " (" + file.Path + ")\n";
                    OutputTextBlock.Text += "Parent: " + parentFolder.Name + " (" + parentFolder.Path + ")\n";
                }
            }
            else
            {
                rootPage.NotifyUserFileNotExist();
            }
        }
    }
}

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
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FileAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario9 : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public Scenario9()
        {
            this.InitializeComponent();
            rootPage.ValidateFile();
            CompareFilesButton.Click += new RoutedEventHandler(CompareFilesButton_Click);
        }

        /// <summary>
        /// Compares a picked file with sample.dat
        /// </summary>
        private async void CompareFilesButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.ResetScenarioOutput(OutputTextBlock);
            StorageFile file = rootPage.sampleFile;
            if (file != null)
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add("*");
                StorageFile comparand = await picker.PickSingleFileAsync();
                if (comparand != null)
                {
                    if (file.IsEqual(comparand))
                    {
                        OutputTextBlock.Text = "Files are equal";
                    }
                    else
                    {
                        OutputTextBlock.Text = "Files are not equal";
                    }
                }
                else
                {
                    OutputTextBlock.Text = "Operation cancelled";
                }
            }
            else
            {
                rootPage.NotifyUserFileNotExist();
            }
        }
    }
}

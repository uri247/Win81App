'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SetVersion
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private appData As ApplicationData = Nothing

    Private Const settingName As String = "SetVersionSetting"
    Private Const settingValue0 As String = "Data.v0"
    Private Const settingValue1 As String = "Data.v1"

    Public Sub New()
        Me.InitializeComponent()

        appData = ApplicationData.Current

        DisplayOutput()
    End Sub

    Private Sub SetVersionHandler0(ByVal request As SetVersionRequest)
        Dim deferral As SetVersionDeferral = request.GetDeferral()

        Dim version As UInteger = appData.Version

        Select Case version
            Case 0
                ' Version is already 0.  Nothing to do.

            Case 1
                ' Need to convert data from v1 to v0.

                ' This sample simulates that conversion by writing a version-specific value.
                appData.LocalSettings.Values(settingName) = settingValue0


            Case Else
                Throw New Exception("Unexpected ApplicationData Version: " & version)
        End Select

        deferral.Complete()
    End Sub

    Private Sub SetVersionHandler1(ByVal request As SetVersionRequest)
        Dim deferral As SetVersionDeferral = request.GetDeferral()

        Dim version As UInteger = appData.Version

        Select Case version
            Case 0
                ' Need to convert data from v0 to v1.

                ' This sample simulates that conversion by writing a version-specific value.
                appData.LocalSettings.Values(settingName) = settingValue1


            Case 1
                ' Version is already 1.  Nothing to do.

            Case Else
                Throw New Exception("Unexpected ApplicationData Version: " & version)
        End Select

        deferral.Complete()
    End Sub

    Private Async Sub SetVersion0_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Await appData.SetVersionAsync(0, New ApplicationDataSetVersionHandler(AddressOf SetVersionHandler0))
        DisplayOutput()
    End Sub

    Private Async Sub SetVersion1_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Await appData.SetVersionAsync(1, New ApplicationDataSetVersionHandler(AddressOf SetVersionHandler1))
        DisplayOutput()
    End Sub

    Private Sub DisplayOutput()
        OutputTextBlock.Text = "Version: " & appData.Version
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub
End Class

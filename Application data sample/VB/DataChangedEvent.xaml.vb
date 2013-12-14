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
Partial Public NotInheritable Class DataChangedEvent
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private applicationData As ApplicationData = Nothing
    Private roamingSettings As ApplicationDataContainer = Nothing
    Private dataChangedEventHandler As TypedEventHandler(Of ApplicationData, Object) = Nothing

    Private Const settingName As String = "userName"

    Public Sub New()
        Me.InitializeComponent()

        applicationData = applicationData.Current
        roamingSettings = applicationData.RoamingSettings

        DisplayOutput()
    End Sub

    Private Sub SimulateRoaming_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        roamingSettings.Values(settingName) = UserName.Text

        ' Simulate roaming by intentionally signaling a data changed event.
        applicationData.SignalDataChanged()
    End Sub

    Private Async Sub DataChangedHandler(ByVal appData As Windows.Storage.ApplicationData, ByVal o As Object)
        ' DataChangeHandler may be invoked on a background thread, so use the Dispatcher to invoke the UI-related code on the UI thread.
        Await Me.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() DisplayOutput())
    End Sub

    Private Sub DisplayOutput()
        Dim value As Object = roamingSettings.Values(settingName)
        OutputTextBlock.Text = CStr("Name: " & (If(value Is Nothing, "<empty>", ("""" & CStr(value) & """"))))

    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        dataChangedEventHandler = New TypedEventHandler(Of ApplicationData, Object)(AddressOf DataChangedHandler)
        AddHandler applicationData.DataChanged, dataChangedEventHandler
        'applicationData.DataChanged += dataChangedHandler_Renamed
    End Sub

    ''' <summary>
    ''' Invoked immediately before the Page is unloaded and is no longer the current source of a parent Frame.
    ''' </summary>
    ''' <param name="e">
    ''' Event data that can be examined by overriding code. The event data is representative
    ''' of the navigation that will unload the current Page unless canceled. The
    ''' navigation can potentially be canceled by setting Cancel.
    ''' </param>
    Protected Overrides Sub OnNavigatingFrom(ByVal e As NavigatingCancelEventArgs)
        MyBase.OnNavigatingFrom(e)
        RemoveHandler applicationData.DataChanged, dataChangedEventHandler
        dataChangedEventHandler = Nothing
    End Sub
End Class


'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports Windows.System.Threading

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class DelayTimer
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        ThreadPoolSample.DelayTimerScenario = Me
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        DelayMs.SelectedIndex = ThreadPoolSample.DelayTimerSelectedIndex
        UpdateUI(ThreadPoolSample.DelayTimerStatus)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ThreadPoolSample.DelayTimerSelectedIndex = DelayMs.SelectedIndex
    End Sub

    ''' <summary>
    ''' Create a Delay timer that fires once after the specified delay elapses.
    ''' When the timer expires, its callback hander is called.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CreateDelayTimer(ByVal sender As Object, ByVal args As RoutedEventArgs)
        If Integer.TryParse(DelayMs.SelectionBoxItem.ToString(), ThreadPoolSample.DelayTimerMilliseconds) Then
            ThreadPoolSample.DelayTimer = ThreadPoolTimer.CreateTimer(Async Sub(timer)
                                                                          Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() ThreadPoolSample.DelayTimerScenario.UpdateUI(Status.Completed))
                                                                      End Sub, TimeSpan.FromMilliseconds(ThreadPoolSample.DelayTimerMilliseconds))

            UpdateUI(Status.Started)
        End If
    End Sub

    Private Sub CancelDelayTimer(ByVal sender As Object, ByVal args As RoutedEventArgs)
        If ThreadPoolSample.DelayTimer IsNot Nothing Then
            ThreadPoolSample.DelayTimer.Cancel()
            UpdateUI(Status.Canceled)
        End If
    End Sub

    Public Sub UpdateUI(ByVal status As Status)
        ThreadPoolSample.DelayTimerStatus = status
        DelayTimerInfo.Text = String.Format("Timer delay = {0} ms.", ThreadPoolSample.DelayTimerMilliseconds)
        DelayTimerStatus.Text = status.ToString("g")

        Dim createButtonEnabled = (status = ThreadPool.Status.Started)
        CreateDelayTimerButton.IsEnabled = Not createButtonEnabled
        CancelDelayTimerButton.IsEnabled = createButtonEnabled
    End Sub
End Class

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
Partial Public NotInheritable Class PeriodicTimer
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        ThreadPoolSample.PeriodicTimerScenario = Me
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        PeriodMs.SelectedIndex = ThreadPoolSample.PeriodicTimerSelectedIndex
        UpdateUI(ThreadPoolSample.PeriodicTimerStatus)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ThreadPoolSample.PeriodicTimerSelectedIndex = PeriodMs.SelectedIndex
    End Sub

    ''' <summary>
    ''' Create a periodic timer that fires every time the period elapses.
    ''' When the timer expires, its callback handler is called and the timer is reset.
    ''' This behavior continues until the periodic timer is cancelled.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CreatePeriodicTimer(ByVal sender As Object, ByVal args As RoutedEventArgs)
        If Integer.TryParse(PeriodMs.SelectionBoxItem.ToString(), ThreadPoolSample.PeriodicTimerMilliseconds) Then
            ThreadPoolSample.PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(Async Sub(timer)
                                                                                     System.Threading.Interlocked.Increment(ThreadPoolSample.PeriodicTimerCount)
                                                                                     Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub() ThreadPoolSample.PeriodicTimerScenario.UpdateUI(Status.Completed))
                                                                                 End Sub, TimeSpan.FromMilliseconds(ThreadPoolSample.PeriodicTimerMilliseconds))

            UpdateUI(Status.Started)
        End If
    End Sub

    Private Sub CancelPeriodicTimer(ByVal sender As Object, ByVal args As RoutedEventArgs)
        If ThreadPoolSample.PeriodicTimer IsNot Nothing Then
            ThreadPoolSample.PeriodicTimer.Cancel()
            ThreadPoolSample.PeriodicTimerCount = 0
            UpdateUI(Status.Canceled)
        End If
    End Sub

    Public Sub UpdateUI(ByVal status As Status)
        ThreadPoolSample.PeriodicTimerStatus = status

        Select Case status
            Case ThreadPool.Status.Completed
                PeriodicTimerStatus.Text = String.Format("Completion count: {0}", ThreadPoolSample.PeriodicTimerCount)
            Case Else
                PeriodicTimerStatus.Text = status.ToString("g")
        End Select

        PeriodicTimerInfo.Text = String.Format("Timer Period = {0} ms.", ThreadPoolSample.PeriodicTimerMilliseconds)

        Dim createButtonEnabled = ((status <> ThreadPool.Status.Started) AndAlso (status <> ThreadPool.Status.Completed))
        CreatePeriodicTimerButton.IsEnabled = createButtonEnabled
        CancelPeriodicTimerButton.IsEnabled = Not createButtonEnabled
    End Sub
End Class

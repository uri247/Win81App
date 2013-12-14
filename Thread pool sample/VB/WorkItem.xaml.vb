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
Partial Public NotInheritable Class WorkItem
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        ThreadPoolSample.WorkItemScenaioro = Me
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        Priority.SelectedIndex = ThreadPoolSample.WorkItemSelectedIndex
        UpdateUI(ThreadPoolSample.WorkItemStatus)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ThreadPoolSample.WorkItemSelectedIndex = Priority.SelectedIndex
    End Sub

    Private Sub CreateThreadPoolWorkItem(ByVal sender As Object, ByVal args As RoutedEventArgs)
        ' Variable that will be passed to WorkItemFunction, this variable is the number
        ' of interlocked increments that the woker function will complete, used to simulate
        ' work.

        Dim maxCount As Long = 10000000

        ' Create a thread pool work item with specified priority.

        Select Case Priority.SelectionBoxItem.ToString()
            Case "Low"
                ThreadPoolSample.WorkItemPriority = WorkItemPriority.Low
            Case "Normal"
                ThreadPoolSample.WorkItemPriority = WorkItemPriority.Normal
            Case "High"
                ThreadPoolSample.WorkItemPriority = WorkItemPriority.High
        End Select

        ' Create the work item with the specified priority.

        ThreadPoolSample.ThreadPoolWorkItem = Windows.System.Threading.ThreadPool.RunAsync(Sub(source)
                                                                                               ' Perform the thread pool work item activity.
                                                                                               ' When WorkItem.Cancel is called, work items that have not started are canceled.
                                                                                               ' If a work item is already running, it will run to completion unless it supports cancellation.
                                                                                               ' To support cancellation, the work item should check IAsyncAction.Status for cancellation status
                                                                                               ' and exit cleanly if it has been canceled.
                                                                                               ' Simulate doing work.
                                                                                               ' Update work item progress in the UI.
                                                                                               ' Only update if the progress value has changed.
                                                                                               Dim count As Long = 0
                                                                                               Dim oldProgress As Long = 0
                                                                                               Do While count < maxCount
                                                                                                   If source.Status = AsyncStatus.Canceled Then
                                                                                                       Exit Do
                                                                                                   End If
                                                                                                   System.Threading.Interlocked.Increment(count)
                                                                                                   Dim currentProgress As Long = CLng((CDbl(count) / CDbl(maxCount) * 100))
                                                                                                   If currentProgress > oldProgress Then
                                                                                                       Dim ignored = Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub() ThreadPoolSample.WorkItemScenaioro.UpdateWorkItemProgressUI(currentProgress))
                                                                                                   End If
                                                                                                   oldProgress = currentProgress
                                                                                               Loop
                                                                                           End Sub, ThreadPoolSample.WorkItemPriority)

        ' Register a completed-event handler to run when the work item finishes or is canceled.

        ThreadPoolSample.ThreadPoolWorkItem.Completed = New AsyncActionCompletedHandler(Async Sub(source As IAsyncAction, asyncStatus As AsyncStatus)
                                                                                            Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                                                                                                       Select Case asyncStatus
                                                                                                                                                           Case asyncStatus.Started
                                                                                                                                                               ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Started)
                                                                                                                                                           Case asyncStatus.Completed
                                                                                                                                                               ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Completed)
                                                                                                                                                           Case asyncStatus.Canceled
                                                                                                                                                               ThreadPoolSample.WorkItemScenaioro.UpdateUI(Status.Canceled)
                                                                                                                                                       End Select
                                                                                                                                                   End Sub)
                                                                                        End Sub)

        UpdateUI(Status.Started)
    End Sub

    Private Sub CancelThreadPoolWorkItem(ByVal sender As Object, ByVal args As RoutedEventArgs)
        If ThreadPoolSample.ThreadPoolWorkItem IsNot Nothing Then
            ThreadPoolSample.ThreadPoolWorkItem.Cancel()
        End If
    End Sub

    Public Sub UpdateUI(ByVal status As Status)
        ThreadPoolSample.WorkItemStatus = status

        WorkItemStatus.Text = status.ToString("g")
        WorkItemInfo.Text = String.Format("Work item priority = {0}", ThreadPoolSample.WorkItemPriority.ToString("g"))

        Dim createButtonEnabled = (status <> ThreadPool.Status.Started)
        CreateThreadPoolWorkItemButton.IsEnabled = createButtonEnabled
        CancelThreadPoolWorkItemButton.IsEnabled = Not createButtonEnabled
    End Sub

    Public Sub UpdateWorkItemProgressUI(ByVal percentComplete As Long)
        WorkItemStatus.Text = String.Format("Progress: {0}%", percentComplete.ToString())
    End Sub
End Class

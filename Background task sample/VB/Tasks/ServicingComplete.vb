' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Storage

'
' The namespace for the background tasks.
'
'
' A background task always implements the IBackgroundTask interface.
'
Public NotInheritable Class ServicingComplete
    Implements IBackgroundTask

    Private _cancelRequested As Boolean = False

    '
    ' The Run method is the entry point of a background task.
    '
    Public Sub Run(ByVal taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        Debug.WriteLine("ServicingComplete " & taskInstance.Task.Name & " starting...")
        '
        ' Associate a cancellation handler with the background task.
        '
        AddHandler taskInstance.Canceled, AddressOf OnCanceled
        '
        ' Do background task activity for servicing complete.
        '
        Dim Progress As UInteger
        For Progress = 0 To 100 Step 10
            '
            ' If the cancellation handler indicated that the task was canceled, stop doing the task.
            '
            If Volatile.Read(_cancelRequested) Then
                Exit For
            End If

            '
            ' Indicate progress to foreground application.
            '
            taskInstance.Progress = Progress
        Next Progress

        Dim settings = ApplicationData.Current.LocalSettings
        Dim key = taskInstance.Task.Name

        '
        ' Write to LocalSettings to indicate that this background task ran.
        '
        settings.Values(key) = If(Progress < 100, "Canceled", "Completed")
        Debug.WriteLine("ServicingComplete " & taskInstance.Task.Name + (If(Progress < 100, " Canceled", " Completed")))
    End Sub

    '
    ' Handles background task cancellation.
    '
    Private Sub OnCanceled(ByVal sender As IBackgroundTaskInstance, ByVal reason As BackgroundTaskCancellationReason)
        '
        ' Indicate that the background task is canceled.
        '
        Volatile.Write(_cancelRequested, True)

        Debug.WriteLine("ServicingComplete " & sender.Task.Name & " Cancel Requested...")
    End Sub
End Class


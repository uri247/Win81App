' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved


Public NotInheritable Class MaintenanceTask
    Implements IBackgroundTask

    Public Sub Run(ByVal taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        Dim notifier As New Notifier()

        ' It's important not to block UI threads. Since this is a background task, we do need
        ' to block on the channel operations completing
        notifier.RenewAllAsync(False).AsTask().Wait()
    End Sub
End Class


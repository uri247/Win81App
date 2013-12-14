Imports System.Net

' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Public NotInheritable Class ChannelAndWebResponse
    Public Property Channel() As PushNotificationChannel
    Public Property WebResponse() As String
End Class

<DataContract> _
Friend Class UrlData
    <DataMember> _
    Public Url As String
    <DataMember> _
    Public ChannelUri As String
    <DataMember> _
    Public IsAppId As Boolean
    <DataMember> _
    Public Renewed As Date
End Class

Public NotInheritable Class Notifier
    Private Const APP_TILE_ID_KEY As String = "appTileIds"
    Private Const MAIN_APP_TILE_KEY As String = "mainAppTileKey"
    Private Const DAYS_TO_RENEW As Integer = 15 ' Renew if older than 15 days
    Private urls As Dictionary(Of String, UrlData)

    Public Sub New()
        Me.urls = New Dictionary(Of String, UrlData)()
        Dim storedUrls As List(Of String) = Nothing
        Dim currentData As IPropertySet = ApplicationData.Current.LocalSettings.Values

        Try
            Dim urlString As String = CStr(currentData(APP_TILE_ID_KEY))
            Using stream As New MemoryStream(Encoding.Unicode.GetBytes(urlString))
                Dim deserializer As New DataContractJsonSerializer(GetType(List(Of String)))
                storedUrls = CType(deserializer.ReadObject(stream), List(Of String))
            End Using
        Catch e1 As Exception
        End Try

        If storedUrls IsNot Nothing Then
            For i As Integer = 0 To storedUrls.Count - 1
                Dim key As String = storedUrls(i)
                Try
                    Dim dataString As String = CStr(currentData(key))
                    Using stream As New MemoryStream(Encoding.Unicode.GetBytes(dataString))
                        Dim deserializer As New DataContractJsonSerializer(GetType(UrlData))
                        Me.urls(key) = CType(deserializer.ReadObject(stream), UrlData)
                    End Using
                Catch e2 As Exception
                End Try
            Next i
        End If
    End Sub

    Private Function TryGetUrlData(ByVal key As String) As UrlData
        Dim returnedData As UrlData = Nothing
        SyncLock Me.urls
            If Me.urls.ContainsKey(key) Then
                returnedData = Me.urls(key)
            End If
        End SyncLock

        Return returnedData
    End Function

    Private Sub SetUrlData(ByVal key As String, ByVal dataToSet As UrlData)
        SyncLock Me.urls
            Me.urls(key) = dataToSet
        End SyncLock
    End Sub

    ' Update the stored target URL
    Private Sub UpdateUrl(ByVal url As String, ByVal channelUri As String, ByVal inputItemId As String, ByVal isPrimaryTile As Boolean)
        Dim itemId As String = If(isPrimaryTile AndAlso inputItemId Is Nothing, MAIN_APP_TILE_KEY, inputItemId)

        Dim shouldSerializeTileIds As Boolean = TryGetUrlData(itemId) Is Nothing
        Dim storedData As New UrlData() With {.Url = url, .ChannelUri = channelUri, .IsAppId = isPrimaryTile, .Renewed = Date.Now}
        SetUrlData(itemId, storedData)

        Using stream As New MemoryStream()
            Dim serializer As New DataContractJsonSerializer(GetType(UrlData))
            serializer.WriteObject(stream, storedData)
            stream.Position = 0
            Using reader As New StreamReader(stream)
                ApplicationData.Current.LocalSettings.Values(itemId) = reader.ReadToEnd()
            End Using
        End Using

        If shouldSerializeTileIds Then
            SaveAppTileIds()
        End If
    End Sub

    Private Sub SaveAppTileIds()
        Dim dataToStore As List(Of String)

        SyncLock Me.urls
            dataToStore = New List(Of String)(Me.urls.Count)
            For Each key As String In Me.urls.Keys
                dataToStore.Add(key)
            Next key
        End SyncLock

        Using stream As New MemoryStream()
            Dim serializer As New DataContractJsonSerializer(GetType(List(Of String)))
            serializer.WriteObject(stream, dataToStore)
            stream.Position = 0
            Using reader As New StreamReader(stream)
                ApplicationData.Current.LocalSettings.Values(APP_TILE_ID_KEY) = reader.ReadToEnd()
            End Using
        End Using
    End Sub

    ' This method checks the freshness of each channel, and returns as necessary
    Public Function RenewAllAsync(ByVal force As Boolean) As IAsyncAction
        Dim now As Date = Date.Now
        Dim daysToRenew As New TimeSpan(DAYS_TO_RENEW, 0, 0, 0)
        Dim renewalTasks As List(Of Task(Of ChannelAndWebResponse))
        SyncLock Me.urls
            renewalTasks = New List(Of Task(Of ChannelAndWebResponse))(Me.urls.Count)
            For Each keyValue In Me.urls
                Dim dataForUpload As UrlData = keyValue.Value
                If force OrElse ((now.Subtract(dataForUpload.Renewed)) > daysToRenew) Then
                    If keyValue.Key = MAIN_APP_TILE_KEY Then
                        renewalTasks.Add(OpenChannelAndUploadAsync(dataForUpload.Url).AsTask())
                    Else
                        renewalTasks.Add(OpenChannelAndUploadAsync(dataForUpload.Url, keyValue.Key, dataForUpload.IsAppId).AsTask())
                    End If

                End If
            Next keyValue
        End SyncLock
        Return Task.WhenAll(renewalTasks).AsAsyncAction()
    End Function

    ' Instead of using the async and await keywords, actual Tasks will be returned.
    ' That way, components consuming these APIs can await the returned tasks
    Public Function OpenChannelAndUploadAsync(ByVal url As String) As IAsyncOperation(Of ChannelAndWebResponse)
        Dim channelOperation As IAsyncOperation(Of PushNotificationChannel) = PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync()
        Return ExecuteChannelOperation(channelOperation, url, MAIN_APP_TILE_KEY, True)
    End Function

    Public Function OpenChannelAndUploadAsync(ByVal url As String, ByVal inputItemId As String, ByVal isPrimaryTile As Boolean) As IAsyncOperation(Of ChannelAndWebResponse)
        Dim channelOperation As IAsyncOperation(Of PushNotificationChannel)
        If isPrimaryTile Then
            channelOperation = PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync(inputItemId)
        Else
            channelOperation = PushNotificationChannelManager.CreatePushNotificationChannelForSecondaryTileAsync(inputItemId)
        End If

        Return ExecuteChannelOperation(channelOperation, url, inputItemId, isPrimaryTile)
    End Function

    Private Function ExecuteChannelOperation(ByVal channelOperation As IAsyncOperation(Of PushNotificationChannel), ByVal url As String, ByVal itemId As String, ByVal isPrimaryTile As Boolean) As IAsyncOperation(Of ChannelAndWebResponse)
        Return channelOperation.AsTask().ContinueWith(Of ChannelAndWebResponse)(Function(channelTask As Task(Of PushNotificationChannel))
                                                                                    ' Upload the channel URI if the client hasn't recorded sending the same uri to the server
                                                                                    ' Only update the data on the client if uploading the channel URI succeeds.
                                                                                    ' If it fails, you may considered setting another AC task, trying again, etc.
                                                                                    ' OpenChannelAndUploadAsync will throw an exception if upload fails
                                                                                    Dim newChannel As PushNotificationChannel = channelTask.Result
                                                                                    Dim webResponse As String = "URI already uploaded"
                                                                                    Dim dataForItem As UrlData = TryGetUrlData(itemId)
                                                                                    If dataForItem Is Nothing OrElse newChannel.Uri <> dataForItem.ChannelUri Then
                                                                                        Dim webRequest As HttpWebRequest = CType(HttpWebRequest.Create(url), HttpWebRequest)
                                                                                        webRequest.Method = "POST"
                                                                                        webRequest.ContentType = "application/x-www-form-urlencoded"
                                                                                        Dim channelUriInBytes() As Byte = Encoding.UTF8.GetBytes("ChannelUri=" & WebUtility.UrlEncode(newChannel.Uri) & "&ItemId=" & WebUtility.UrlEncode(itemId))
                                                                                        Dim requestTask As Task(Of Stream) = webRequest.GetRequestStreamAsync()
                                                                                        Using requestStream As Stream = requestTask.Result
                                                                                            requestStream.Write(channelUriInBytes, 0, channelUriInBytes.Length)
                                                                                        End Using
                                                                                        Dim responseTask As Task(Of WebResponse) = webRequest.GetResponseAsync()
                                                                                        Using requestReader As New StreamReader(responseTask.Result.GetResponseStream())
                                                                                            webResponse = requestReader.ReadToEnd()
                                                                                        End Using
                                                                                    End If
                                                                                    UpdateUrl(url, newChannel.Uri, itemId, isPrimaryTile)
                                                                                    Return New ChannelAndWebResponse With {.Channel = newChannel, .WebResponse = webResponse}
                                                                                End Function).AsAsyncOperation()
    End Function
End Class


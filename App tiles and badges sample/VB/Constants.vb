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
Imports System.Collections.Generic
Imports System.Xml.Linq
Imports Tiles

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Tiles Sample (VB)"

        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Send tile notification with text", .ClassType = GetType(SendTextTile)}, _
            New Scenario() With {.Title = "Send tile notification with local images", .ClassType = GetType(SendLocalImageTile)}, _
            New Scenario() With {.Title = "Send tile notification with web images", .ClassType = GetType(SendWebImageTile)}, _
            New Scenario() With {.Title = "Send badge notification", .ClassType = GetType(SendBadge)}, _
            New Scenario() With {.Title = "Send push notifications from a Windows Azure Mobile Service", .ClassType = GetType(UsePushNotifications)}, _
            New Scenario() With {.Title = "Preview all tile notification templates", .ClassType = GetType(PreviewAllTemplates)}, _
            New Scenario() With {.Title = "Enable notification queue and tags", .ClassType = GetType(EnableNotificationQueue)}, _
            New Scenario() With {.Title = "Use notification expiration", .ClassType = GetType(NotificationExpiration)}, _
            New Scenario() With {.Title = "Image protocols and baseUri", .ClassType = GetType(ImageProtocols)}, _
            New Scenario() With {.Title = "Globalization, localization, scale, and accessibility", .ClassType = GetType(Globalization)}, _
            New Scenario() With {.Title = "Content deduplication", .ClassType = GetType(ContentDeduplication)} _
        }

        Friend Shared Function PrettyPrint(ByVal inputString As String) As String
            Dim doc As XDocument = XDocument.Parse(inputString)
            Return doc.ToString()
        End Function

    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

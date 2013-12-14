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
Imports Microsoft.Samples.Networking.WebSocket

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "WebSocket Sample (VB)"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "UTF-8 text messages", .ClassType = GetType(Scenario1)}, _
            New Scenario() With {.Title = "Binary data stream", .ClassType = GetType(Scenario2)} _
        }

        Public Function TryGetUri(ByVal uriString As String, <System.Runtime.InteropServices.Out()> ByRef uri As Uri) As Boolean
            uri = Nothing

            Dim webSocketUri As Uri = Nothing
            If Not System.Uri.TryCreate(uriString.Trim(), UriKind.Absolute, webSocketUri) Then
                NotifyUser("Error: Invalid URI", NotifyType.ErrorMessage)
                Return False
            End If

            ' Fragments are not allowed in WebSocket URIs.
            If Not String.IsNullOrEmpty(webSocketUri.Fragment) Then
                NotifyUser("Error: URI fragments not supported in WebSocket URIs.", NotifyType.ErrorMessage)
                Return False
            End If

            ' Uri.SchemeName returns the canonicalized scheme name so we can use case-sensitive, ordinal string
            ' comparison.
            If (webSocketUri.Scheme <> "ws") AndAlso (webSocketUri.Scheme <> "wss") Then
                NotifyUser("Error: WebSockets only support ws:// and wss:// schemes.", NotifyType.ErrorMessage)
                Return False
            End If

            uri = webSocketUri

            Return True
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

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.XAMLPopup
    ''' <summary>
    ''' An empty page that can be used on its own or navigated to within a Frame.
    ''' </summary>
    Partial Public NotInheritable Class Scenario1
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        ' as NotifyUser()
        Private rootPage As MainPage = MainPage.Current

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        ' Handles the Click event on the Button within the simple Popup control and simply closes it.
        Private Sub ClosePopupClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' if the Popup is open, then close it
            If StandardPopup.IsOpen Then
                StandardPopup.IsOpen = False
            End If
        End Sub

        ' Handles the Click event on the Button within the simple Popup control and simply opens it.
        Private Sub ShowPopupOffsetClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' open the Popup if it isn't open already
            If Not StandardPopup.IsOpen Then
                StandardPopup.IsOpen = True
            End If
        End Sub

    End Class
End Namespace

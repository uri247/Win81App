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
    Partial Public NotInheritable Class Scenario2
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        ' as NotifyUser()
        Private rootPage As MainPage = MainPage.Current
        Private nonParentPopup As Popup

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        ' Handles the Click event of the Button demonstrating a parented Popup with input content
        Private Sub ShowPopupWithParentClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If Not ParentedPopup.IsOpen Then
                ParentedPopup.IsOpen = True
            End If
        End Sub

        ' Handles the Click event of the Button demonstrating a non-parented Popup with input content
        Private Sub ShowPopupWithoutParentClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' if we already have one showing, don't create another one
            If nonParentPopup Is Nothing Then
                ' create the Popup in code
                nonParentPopup = New Popup()

                ' we are creating this in code and need to handle multiple instances
                ' so we are attaching to the Popup.Closed event to remove our reference
                AddHandler nonParentPopup.Closed, Sub(senderPopup, argsPopup) nonParentPopup = Nothing
                nonParentPopup.HorizontalOffset = 200
                nonParentPopup.VerticalOffset = Window.Current.Bounds.Height - 200

                ' set the content to our UserControl
                nonParentPopup.Child = New PopupInputContent()

                ' open the Popup
                nonParentPopup.IsOpen = True
            End If
        End Sub
    End Class
End Namespace

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
    Partial Public NotInheritable Class Scenario3
        Inherits SDKTemplate.Common.LayoutAwarePage

        ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        ' as NotifyUser()
        Private rootPage As MainPage = MainPage.Current

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        ' handles the Click event of the Button for showing the light dismiss behavior
        Private Sub ShowPopupLightDismissClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If Not LightDismissSimplePopup.IsOpen Then
                LightDismissSimplePopup.IsOpen = True
            End If
        End Sub

        ' Handles the Click event on the Button within the simple Popup control and simply closes it.
        Private Sub ClosePopupClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' if the Popup is open, then close it
            If LightDismissSimplePopup.IsOpen Then
                LightDismissSimplePopup.IsOpen = False
            End If
        End Sub

        ' handles the Click event of the Button for showing the light dismiss with animations behavior
        Private Sub ShowPopupAnimationClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If Not LightDismissAnimatedPopup.IsOpen Then
                LightDismissAnimatedPopup.IsOpen = True
            End If
        End Sub

        ' Handles the Click event on the Button within the simple Popup control and simply closes it.
        Private Sub CloseAnimatedPopupClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If LightDismissAnimatedPopup.IsOpen Then
                LightDismissAnimatedPopup.IsOpen = False
            End If
        End Sub

        ' handles the Click event of the Button for showing the light dismiss with settings behavior
        Private Sub ShowPopupSettingsClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If Not SettingsAnimatedPopup.IsOpen Then
                '                 The UI guidelines for a proper 'Settings' flyout are such that it should fill the height of the 
                '                current Window and be either narrow (346px) or wide (646px)
                '                Using the measurements of the Window.Curent.Bounds will help you position correctly.
                '                This sample here shows a simple *example* of this using the Width to get the HorizontalOffset but
                '                the app developer will have to perform these measurements depending on the structure of the app's 
                '                views in their code 
                RootPopupBorder.Width = 646
                SettingsAnimatedPopup.HorizontalOffset = Window.Current.Bounds.Width - 646

                SettingsAnimatedPopup.IsOpen = True
            End If
        End Sub

        ' Handles the Click event on the Button within the simple Popup control and simply closes it.
        Private Sub CloseSettingsPopupClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If SettingsAnimatedPopup.IsOpen Then
                SettingsAnimatedPopup.IsOpen = False
            End If
        End Sub
    End Class
End Namespace

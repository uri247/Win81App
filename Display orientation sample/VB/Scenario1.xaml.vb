'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Devices.Sensors
Imports Windows.Graphics.Display


Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rotationAngle As Double = 0.0
    Private Const toDegrees As Double = 180.0 / Math.PI
    Private accelerometer As Accelerometer

    Public Sub New()
        InitializeComponent()
        accelerometer = accelerometer.GetDefault()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        deviceRotation.Text = rotationAngle.ToString()

        If accelerometer IsNot Nothing Then
            AddHandler accelerometer.ReadingChanged, AddressOf CalculateDeviceRotation
        End If
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        If accelerometer IsNot Nothing Then
            RemoveHandler accelerometer.ReadingChanged, AddressOf CalculateDeviceRotation
        End If
    End Sub

    Private Async Sub CalculateDeviceRotation(ByVal sender As Accelerometer, ByVal args As AccelerometerReadingChangedEventArgs)
        ' Compute the rotation angle based on the accelerometer's position
        Dim angle = Math.Atan2(args.Reading.AccelerationY, args.Reading.AccelerationX) * toDegrees

        ' Since our arrow points upwards insted of the right, we rotate the coordinate system by 90 degrees
        angle += 90

        ' Ensure that the range of the value is between [0, 360)
        If angle < 0 Then
            angle += 360
        End If

        rotationAngle = angle

        ' Update the UI with the new value
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     deviceRotation.Text = rotationAngle.ToString()
                                                                     UpdateArrowForRotation()
                                                                 End Sub)
    End Sub

    Private Sub UpdateArrowForRotation()
        ' Obtain current rotation.
        Dim screenRotation = 0

        Select Case DisplayInformation.GetForCurrentView().CurrentOrientation
            Case DisplayOrientations.Landscape
                screenRotation = 0

            Case DisplayOrientations.Portrait
                screenRotation = 90

            Case DisplayOrientations.LandscapeFlipped
                screenRotation = 180

            Case DisplayOrientations.PortraitFlipped
                screenRotation = 270

            Case Else
                screenRotation = 0
        End Select

        Dim steeringAngle As Double = rotationAngle - screenRotation

        ' Keep the steering angle positive             
        If steeringAngle < 0 Then
            steeringAngle += 360
        End If

        ' Update the UI based on steering action
        Dim transform As New RotateTransform()
        transform.Angle = -steeringAngle
        transform.CenterX = scenario1Image.ActualWidth / 2
        transform.CenterY = scenario1Image.ActualHeight / 2

        scenario1Image.RenderTransform = transform
    End Sub
End Class

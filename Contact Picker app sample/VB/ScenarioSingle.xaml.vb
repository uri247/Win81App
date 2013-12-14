'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.ApplicationModel.Contacts
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml.Media.Imaging

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ScenarioSingle
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickAContactButton.Click, AddressOf PickAContactButton_Click
    End Sub

    Private Async Sub PickAContactButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim contactPicker = New Windows.ApplicationModel.Contacts.ContactPicker()
        contactPicker.CommitButtonText = "Select"
        Dim contact As Contact = Await contactPicker.PickContactAsync()

        If contact IsNot Nothing Then
            OutputFields.Visibility = Visibility.Visible
            OutputEmpty.Visibility = Visibility.Collapsed

            OutputName.Text = contact.DisplayName
            AppendContactFieldValues(OutputEmailHeader, OutputEmails, contact.Emails)
            AppendContactFieldValues(OutputPhoneNumberHeader, OutputPhoneNumbers, contact.Phones)
            AppendContactFieldValues(OutputAddressHeader, OutputAddresses, contact.Addresses)

            If contact.Thumbnail IsNot Nothing Then
                Dim stream As IRandomAccessStreamWithContentType = Await contact.Thumbnail.OpenReadAsync()
                If stream IsNot Nothing AndAlso stream.Size > 0 Then
                    Dim bitmap As New BitmapImage()
                    bitmap.SetSource(stream)
                    OutputThumbnail.Source = bitmap
                Else
                    OutputThumbnail.Source = Nothing
                End If
            End If
        Else
            OutputEmpty.Visibility = Visibility.Visible
            OutputFields.Visibility = Visibility.Collapsed
            OutputThumbnail.Source = Nothing
        End If

    End Sub

    Private Sub AppendContactFieldValues(Of T)(ByVal header As TextBlock, ByVal content As TextBlock, ByVal fields As IList(Of T))
        If fields.Count > 0 Then
            Dim output As New StringBuilder()
            If fields(0).GetType() Is GetType(ContactEmail) Then
                For Each email As ContactEmail In TryCast(fields, IList(Of ContactEmail))
                    output.AppendFormat("Email Address: {0} ({1})" & vbLf, email.Address, email.Kind)
                Next email
            ElseIf fields(0).GetType() Is GetType(ContactPhone) Then
                For Each phone As ContactPhone In TryCast(fields, IList(Of ContactPhone))
                    output.AppendFormat("Phone: {0} ({1})" & vbLf, phone.Number, phone.Kind)
                Next phone
            ElseIf fields(0).GetType() Is GetType(ContactAddress) Then
                Dim addressParts As List(Of String) = Nothing
                Dim unstructuredAddress As String = ""

                For Each address As ContactAddress In TryCast(fields, IList(Of ContactAddress))
                    addressParts = (New List(Of String) From {address.StreetAddress, address.Locality, address.Region, address.PostalCode})
                    unstructuredAddress = String.Join(", ", addressParts.FindAll(Function(s) (Not String.IsNullOrEmpty(s))))
                    output.AppendFormat("Address: {0} ({1})" & vbLf, unstructuredAddress, address.Kind)
                Next address
            End If

            header.Visibility = Visibility.Visible
            content.Visibility = Visibility.Visible
            content.Text = output.ToString()
        Else
            header.Visibility = Visibility.Collapsed
            content.Visibility = Visibility.Collapsed
        End If
    End Sub
End Class


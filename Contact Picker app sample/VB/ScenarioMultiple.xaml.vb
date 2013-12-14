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

Partial Public NotInheritable Class ScenarioMultiple
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current
    Public contacts As IList(Of Contact)

    Public Sub New()
        Me.InitializeComponent()
        AddHandler PickContactsButton.Click, AddressOf PickContactsButton_Click
        OutputContacts.Visibility = Windows.UI.Xaml.Visibility.Collapsed
    End Sub

    Private Async Sub PickContactsButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim contactPicker = New Windows.ApplicationModel.Contacts.ContactPicker()
        contactPicker.CommitButtonText = "Select"
        contacts = Await contactPicker.PickContactsAsync()

        OutputContacts.Items.Clear()

        If contacts.Count > 0 Then
            OutputContacts.Visibility = Windows.UI.Xaml.Visibility.Visible
            OutputEmpty.Visibility = Visibility.Collapsed

            For Each contact As Contact In contacts
                OutputContacts.Items.Add(New ContactItemAdapter(contact))
            Next contact
        Else
            OutputEmpty.Visibility = Visibility.Visible
        End If
    End Sub
End Class

Public Class ContactItemAdapter
    Private privateName As String
    Public Property Name() As String
        Get
            Return privateName
        End Get
        Private Set(ByVal value As String)
            privateName = value
        End Set
    End Property
    Private privateSecondaryText As String
    Public Property SecondaryText() As String
        Get
            Return privateSecondaryText
        End Get
        Private Set(ByVal value As String)
            privateSecondaryText = value
        End Set
    End Property
    Private privateThumbnail As BitmapImage
    Public Property Thumbnail() As BitmapImage
        Get
            Return privateThumbnail
        End Get
        Private Set(ByVal value As BitmapImage)
            privateThumbnail = value
        End Set
    End Property

    Public Sub New(ByVal contact As Contact)
        Name = contact.DisplayName
        If contact.Emails.Count > 0 Then
            SecondaryText = contact.Emails(0).Address
        ElseIf contact.Phones.Count > 0 Then
            SecondaryText = contact.Phones(0).Number
        ElseIf contact.Addresses.Count > 0 Then
            Dim addressParts As List(Of String) = (New List(Of String) From {contact.Addresses(0).StreetAddress, contact.Addresses(0).Locality, contact.Addresses(0).Region, contact.Addresses(0).PostalCode})
            Dim unstructuredAddress As String = String.Join(", ", addressParts.FindAll(Function(s) (Not String.IsNullOrEmpty(s))))
            SecondaryText = unstructuredAddress
        End If
        GetThumbnail(contact)
    End Sub

    Private Async Sub GetThumbnail(ByVal contact As Contact)
        If contact.Thumbnail IsNot Nothing Then
            Dim stream As IRandomAccessStreamWithContentType = Await contact.Thumbnail.OpenReadAsync()
            If stream IsNot Nothing AndAlso stream.Size > 0 Then
                Thumbnail = New BitmapImage()
                Thumbnail.SetSource(stream)
            End If
        End If
    End Sub
End Class


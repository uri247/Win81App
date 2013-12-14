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
Imports Windows.ApplicationModel.Contacts.Provider

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ContactPickerPage
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private contactPickerUI As ContactPickerUI = MainPagePicker.Current.contactPickerUI
    Private dispatch As CoreDispatcher = Window.Current.Dispatcher

    Public Sub New()
        Me.InitializeComponent()
        ContactList.ItemsSource = contactSet
        AddHandler ContactList.SelectionChanged, AddressOf ContactList_SelectionChanged
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        AddHandler contactPickerUI.ContactRemoved, AddressOf contactPickerUI_ContactRemoved
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler contactPickerUI.ContactRemoved, AddressOf contactPickerUI_ContactRemoved
    End Sub

    Private Async Sub contactPickerUI_ContactRemoved(ByVal sender As ContactPickerUI, ByVal args As ContactRemovedEventArgs)
        ' The event handler may be invoked on a background thread, so use the Dispatcher to run the UI-related code on the UI thread.
        Dim removedId As String = args.Id
        Await dispatch.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                   For Each contact As SampleContact In ContactList.SelectedItems
                                                                       If contact.Id = removedId Then
                                                                           ContactList.SelectedItems.Remove(contact)
                                                                           OutputText.Text += vbLf & contact.DisplayName & " was removed from the basket"
                                                                           Exit For
                                                                       End If
                                                                   Next contact
                                                               End Sub)
    End Sub

    Private Sub ContactList_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        For Each added As SampleContact In e.AddedItems
            AddSampleContact(added)
        Next added

        For Each removed As SampleContact In e.RemovedItems
            If contactPickerUI.ContainsContact(removed.Id) Then
                contactPickerUI.RemoveContact(removed.Id)
                OutputText.Text = removed.DisplayName & " was removed from the basket"
            End If
        Next removed
    End Sub

    Private Sub AddSampleContact(ByVal sampleContact As SampleContact)
        Dim contact As New Contact()
        contact.Id = sampleContact.Id
        contact.FirstName = sampleContact.FirstName
        contact.LastName = sampleContact.LastName

        If Not String.IsNullOrEmpty(sampleContact.HomeEmail) Then
            Dim homeEmail As New ContactEmail()
            homeEmail.Address = sampleContact.HomeEmail
            homeEmail.Kind = ContactEmailKind.Personal
            contact.Emails.Add(homeEmail)
        End If

        If Not String.IsNullOrEmpty(sampleContact.WorkEmail) Then
            Dim workEmail As New ContactEmail()
            workEmail.Address = sampleContact.WorkEmail
            workEmail.Kind = ContactEmailKind.Work
            contact.Emails.Add(workEmail)
        End If

        If Not String.IsNullOrEmpty(sampleContact.HomePhone) Then
            Dim homePhone As New ContactPhone()
            homePhone.Number = sampleContact.HomePhone
            homePhone.Kind = ContactPhoneKind.Home
            contact.Phones.Add(homePhone)
        End If

        If Not String.IsNullOrEmpty(sampleContact.MobilePhone) Then
            Dim mobilePhone As New ContactPhone()
            mobilePhone.Number = sampleContact.MobilePhone
            mobilePhone.Kind = ContactPhoneKind.Mobile
            contact.Phones.Add(mobilePhone)
        End If

        If Not String.IsNullOrEmpty(sampleContact.WorkPhone) Then
            Dim workPhone As New ContactPhone()
            workPhone.Number = sampleContact.WorkPhone
            workPhone.Kind = ContactPhoneKind.Work
            contact.Phones.Add(workPhone)
        End If

        If Not String.IsNullOrEmpty(sampleContact.Street) Then
            Dim homeAddress As New ContactAddress()
            homeAddress.StreetAddress = sampleContact.Street
            homeAddress.Locality = sampleContact.City
            homeAddress.Region = sampleContact.State
            homeAddress.PostalCode = sampleContact.ZipCode
            homeAddress.Kind = ContactAddressKind.Home
            contact.Addresses.Add(homeAddress)
        End If

        Select Case contactPickerUI.AddContact(contact)
            Case AddContactResult.Added
                ' Notify the user that the contact was added
                OutputText.Text = contact.DisplayName & " was added to the basket"
            Case AddContactResult.AlreadyAdded
                ' Notify the user that the contact is already added
                OutputText.Text = contact.DisplayName & " is already in the basket"
            Case Else
                ' Notify the user that the basket is unavailable
                OutputText.Text = contact.DisplayName & " could not be added to the basket"
        End Select
    End Sub

    ' Example contacts to pick from
    Private contactSet As New List(Of SampleContact)() From { _
        New SampleContact() With {.FirstName = "David", .LastName = "Jaffe", .HomeEmail = "david@contoso.com", .WorkEmail = "david@cpandl.com", .HomePhone = "248-555-0150", .Street = "", .City = "", .State = "", .ZipCode = ""}, _
        New SampleContact() With {.FirstName = "Kim", .LastName = "Abercrombie", .HomeEmail = "kim@contoso.com", .WorkEmail = "kim@adatum.com", .HomePhone = "444 555-0001", .WorkPhone = "245 555-0123", .MobilePhone = "921 555-0187", .Street = "123 Main St", .City = "Redmond", .State = "WA", .ZipCode = "23456"}, _
        New SampleContact() With {.FirstName = "Jeff", .LastName = "Phillips", .HomeEmail = "jeff@contoso.com", .WorkEmail = "jeff@fabrikam.com", .HomePhone = "987-555-0199", .MobilePhone = "543-555-0111", .Street = "456 2nd Ave", .City = "Dallas", .State = "TX", .ZipCode = "12345"}, _
        New SampleContact() With {.FirstName = "Arlene", .LastName = "Huff", .HomeEmail = "arlene@contoso.com", .MobilePhone = "234-555-0156"}, _
        New SampleContact() With {.FirstName = "Miles", .LastName = "Reid", .HomeEmail = "miles@contoso.com", .WorkEmail = "miles@proseware.com", .Street = "678 Elm St", .City = "New York", .State = "New York", .ZipCode = "95111"} _
    }

End Class

Friend Class SampleContact
    Private privateId As String
    Public Property Id() As String
        Get
            Return privateId
        End Get
        Private Set(ByVal value As String)
            privateId = value
        End Set
    End Property
    Public Property FirstName() As String
    Public Property LastName() As String
    Public Property HomeEmail() As String
    Public Property WorkEmail() As String
    Public Property HomePhone() As String
    Public Property WorkPhone() As String
    Public Property MobilePhone() As String

    Public Property Street() As String
    Public Property City() As String
    Public Property State() As String
    Public Property ZipCode() As String

    Public Sub New()
        Id = Guid.NewGuid().ToString()
    End Sub

    Public ReadOnly Property DisplayName() As String
        Get
            Return Me.FirstName & " " & Me.LastName
        End Get
    End Property
End Class


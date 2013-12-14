'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ReadBytes
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private Const bytesPerRow As Integer = 16
    Private Const bytesPerSegment As Integer = 2
    Private Const chunkSize As UInteger = 4096

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Hex Dump' button.  Open the image file we want to
    ''' perform a hex dump on.  Then open a sequential-access stream over the file and use
    ''' ReadBytes() to extract the binary data.  Finally, convert each byte to hexadecimal, and
    ''' display the formatted output in the HexDump textblock.
    ''' </summary>
    ''' <param name="sender">Contains information about the button that fired the event.</param>
    ''' <param name="e">Contains state information and event data associated with a routed event.</param>
    Private Async Sub HexDump(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Try
            ' Retrieve the uri of the image and use that to load the file.
            Dim uri As New Uri("ms-appx:///assets/110Strawberry.png")
            Dim file = Await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri)

            ' Open a sequential-access stream over the image file.
            Using inputStream = Await file.OpenSequentialReadAsync()
                ' Pass the input stream to the DataReader.
                Using dataReader = New Windows.Storage.Streams.DataReader(inputStream)
                    Dim currChunk As UInteger = 0
                    Dim numBytes As UInteger
                    ReadBytesOutput.Text = ""

                    ' Create a byte array which can hold enough bytes to populate a row of the hex dump.
                    Dim bytes = New Byte(bytesPerRow - 1) {}

                    Do
                        ' Load the next chunk into the DataReader buffer.
                        numBytes = Await dataReader.LoadAsync(chunkSize)

                        ' Read and print one row at a time.
                        Dim numBytesRemaining = numBytes
                        Do While numBytesRemaining >= bytesPerRow
                            ' Use the DataReader and ReadBytes() to fill the byte array with one row worth of bytes.
                            dataReader.ReadBytes(bytes)

                            PrintRow(bytes, (numBytes - numBytesRemaining) + (currChunk * chunkSize))

                            numBytesRemaining -= CUInt(bytesPerRow)
                        Loop

                        ' If there are any bytes remaining to be read, allocate a new array that will hold
                        ' the remaining bytes read from the DataReader and print the final row.
                        ' Note: ReadBytes() fills the entire array so if the array being passed in is larger
                        ' than what's remaining in the DataReader buffer, an exception will be thrown.
                        If numBytesRemaining > 0 Then
                            bytes = New Byte(CInt(numBytesRemaining) - 1) {}

                            ' Use the DataReader and ReadBytes() to fill the byte array with the last row worth of bytes.
                            dataReader.ReadBytes(bytes)

                            PrintRow(bytes, (numBytes - numBytesRemaining) + (currChunk * chunkSize))
                        End If

                        currChunk += CUInt(1)
                        ' If the number of bytes read is anything but the chunk size, then we've just retrieved the last
                        ' chunk of data from the stream.  Otherwise, keep loading data into the DataReader buffer.
                    Loop While numBytes = chunkSize
                End Using
            End Using
        Catch ex As Exception
            ReadBytesOutput.Text = ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' Format and print a row of the hex dump using the data retrieved by ReadBytes().
    ''' </summary>
    ''' <param name="bytes">An array that holds the data we want to format and output.</param>
    ''' <param name="currByte">Value to be formatted into an address.</param>
    Private Sub PrintRow(ByVal bytes() As Byte, ByVal currByte As UInteger)
        Dim rowStr = ""

        ' Format the address of byte i to have 8 hexadecimal digits and add the address
        ' of the current byte to the output string.
        rowStr &= currByte.ToString("x8")

        ' Format the output:
        For i As Integer = 0 To bytes.Length - 1
            ' If finished with a segment, add a space.
            If i Mod bytesPerSegment = 0 Then
                rowStr &= " "
            End If

            ' Convert the current byte value to hex and add it to the output string.
            rowStr &= bytes(i).ToString("x2")
        Next i

        ' Append the current row to the HexDump textblock.
        ReadBytesOutput.Text += rowStr & vbLf
    End Sub
End Class


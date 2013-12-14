'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports MediaExtensions
Imports Windows.Media

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage

        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Media Extensions"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Private scenariosList As New List(Of Scenario)() From { _
            New Scenario() With {.Title = "Install a local decoder", .ClassType = GetType(CustomDecoder)}, _
            New Scenario() With {.Title = "Install a local scheme handler", .ClassType = GetType(SchemeHandler)}, _
            New Scenario() With {.Title = "Install the built in Video Stabilization Effect", .ClassType = GetType(VideoStabilization)}, _
            New Scenario() With {.Title = "Install a custom Video Effect", .ClassType = GetType(VideoEffect)} _
        }

        Private _extensionManager As New MediaExtensionManager()

        Public ReadOnly Property ExtensionManager() As MediaExtensionManager
            Get
                Return _extensionManager
            End Get
        End Property

        '
        '  Open a single file picker [with fileTypeFilter].
        '  And then, call media.SetSource(picked file).
        '  If the file is successfully opened, VideoMediaOpened() will be called and call media.Play().
        '
        Public Async Sub PickSingleFileAndSet(ByVal fileTypeFilter() As String, ByVal ParamArray mediaElements() As MediaElement)
            Dim dispatcher As CoreDispatcher = Window.Current.Dispatcher

            Dim picker = New Windows.Storage.Pickers.FileOpenPicker()
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
            For Each filter As String In fileTypeFilter
                picker.FileTypeFilter.Add(filter)
            Next filter
            Dim file As StorageFile = Await picker.PickSingleFileAsync()

            If file IsNot Nothing Then
                Try
                    Dim stream = Await file.OpenAsync(FileAccessMode.Read)

                    For i As Integer = 0 To mediaElements.Length - 1
                        Dim [me] As MediaElement = mediaElements(i)
                        [me].Stop()
                        If i + 1 < mediaElements.Length Then
                            [me].SetSource(stream.CloneStream(), file.ContentType)
                        Else
                            [me].SetSource(stream, file.ContentType)
                        End If
                    Next i
                Catch ex As Exception
                    NotifyUser("Cannot open video file - error: " & ex.Message, NotifyType.ErrorMessage)
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Common video failed error handler.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="args"></param>
        Public Sub VideoOnError(ByVal obj As Object, ByVal args As ExceptionRoutedEventArgs)
            NotifyUser("Cannot open video file - error: " & args.ErrorMessage, NotifyType.ErrorMessage)
        End Sub
    End Class

    Public Class Scenario
        Public Property Title() As String

        Public Property ClassType() As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

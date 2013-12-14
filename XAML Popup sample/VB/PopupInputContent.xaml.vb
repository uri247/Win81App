Namespace Global.XAMLPopup
    Partial Public NotInheritable Class PopupInputContent
        Inherits UserControl

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        ' Handles the Click event of the 'Save' button simulating a save and close
        Private Sub SimulateSaveClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
            ' in this example we assume the parent of the UserControl is a Popup
            Dim p As Popup = TryCast(Me.Parent, Popup)
            p.IsOpen = False ' close the Popup
        End Sub
    End Class
End Namespace

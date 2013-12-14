Imports System

' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Partial Public NotInheritable Class BasicUserControl
    Inherits UserControl

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub ClickMeButtonClicked(ByVal sender As Object, ByVal e As RoutedEventArgs)
        OutputText.Text = String.Format("Hello {0}", NameInput.Text)
    End Sub
End Class

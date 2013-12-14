Imports System
Imports Windows.UI.Xaml.Media

' The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

Public NotInheritable Class ImageWithLabelControl
    Inherits Control

    Public Sub New()
        Me.DefaultStyleKey = GetType(ImageWithLabelControl)
    End Sub

    ' you can get help for these properties using the propdp code snippet in C# and Visual Basic
    Public Property ImagePath() As ImageSource
        Get
            Return CType(GetValue(ImagePathProperty), ImageSource)
        End Get
        Set(ByVal value As ImageSource)
            SetValue(ImagePathProperty, value)
        End Set
    End Property

    ' Using a DependencyProperty as the backing store for ImagePath.  This enables animation, styling, binding, etc...
    Public Shared ReadOnly ImagePathProperty As DependencyProperty = DependencyProperty.Register("ImagePath", GetType(ImageSource), GetType(ImageWithLabelControl), New PropertyMetadata(Nothing))

    Public Property Label() As String
        Get
            Return CStr(GetValue(LabelProperty))
        End Get
        Set(ByVal value As String)
            SetValue(LabelProperty, value)
        End Set
    End Property

    ' Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
    Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register("Label", GetType(String), GetType(ImageWithLabelControl), New PropertyMetadata(Nothing))


End Class

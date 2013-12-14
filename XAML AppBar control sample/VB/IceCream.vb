Public Class IceCream
    Private _name As String

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Private _type As String

    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Private _image As String

    Public Property Image() As String
        Get
            Return _image
        End Get
        Set(ByVal value As String)
            _image = value
        End Set
    End Property

    Public Sub New()
    End Sub


End Class


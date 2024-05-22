Public Class ThemedRenderer
    Inherits ToolStripProfessionalRenderer

    Private col As Color

    Public Sub New(col As Color)
        MyBase.New(New ThemedToolStripColorTable(col))
        Me.col = col

    End Sub

    Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)
        Dim arrowBounds = e.ArrowRectangle
        Dim graphics = e.Graphics
        Dim arrowBrush = New SolidBrush(Me.col)
        Dim midY As Integer = arrowBounds.Top + arrowBounds.Height \ 2
        Dim arrowHeight As Integer = arrowBounds.Height \ 3
        Dim arrowWidth As Integer = arrowBounds.Width \ 3
        ' Draw the arrow with the specified color
        graphics.FillPolygon(arrowBrush, New Point() {
             New Point(arrowBounds.Right - arrowWidth, midY - arrowHeight \ 2),
            New Point(arrowBounds.Right, midY),
            New Point(arrowBounds.Right - arrowWidth, midY + arrowHeight \ 2)
        })
    End Sub
End Class

Public Class ThemedToolStripColorTable : Inherits ProfessionalColorTable
    Private col As Color
    Public Sub New(col As Color)
        MyBase.New()
        Me.col = Color.FromArgb(col.A, col.R \ 2, col.G \ 2, col.B \ 2)
    End Sub
    Public Overrides ReadOnly Property ToolStripBorder As Color
        Get
            Return Color.Gray
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripGradientBegin As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripGradientEnd As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ToolStripGradientMiddle As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
        Get
            Return col
        End Get
    End Property
    Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
        Get
            Return Color.Black
        End Get
    End Property

    Public Overrides ReadOnly Property MenuItemSelected As Color
        Get
            Return Color.DarkBlue
        End Get
    End Property

End Class


Public Class ThemedRenderer
    Inherits ToolStripProfessionalRenderer

    Private col As Color

    Public Sub New(col As Color)
        MyBase.New(New ThemedToolStripColorTable(col))
        Me.col = col
    End Sub

    Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)
        Dim arrowBounds = e.ArrowRectangle
        ' Calculate arrow
        Dim midY As Integer = arrowBounds.Top + arrowBounds.Height \ 2
        Dim arrowHeight As Integer = arrowBounds.Height \ 3
        Dim arrowWidth As Integer = arrowBounds.Width \ 3
        ' Draw an arrow with the specified color
        e.Graphics.FillPolygon(New SolidBrush(Me.col), New Point() {
             New Point(arrowBounds.Right - arrowWidth, midY - arrowHeight \ 2),
             New Point(arrowBounds.Right, midY),
             New Point(arrowBounds.Right - arrowWidth, midY + arrowHeight \ 2)
        })
    End Sub

    Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)
        ' Prevent default checkbox from rendering
    End Sub
    Protected Overrides Sub OnRenderItemImage(e As ToolStripItemImageRenderEventArgs)
        ' Draw the image with correct aspect ratio
        If e.Image IsNot Nothing Then

            Dim g As Graphics = e.Graphics
            Dim rect As Rectangle = e.ImageRectangle

            ' Calculate aspect ratio
            Dim aspect As Single = e.Image.Width / e.Image.Height

            ' Calculate new dimensions while maintaining aspect ratio
            Dim w As Integer = rect.Width
            Dim h As Integer = rect.Height

            ' Apply aspect correction
            If aspect <= 1 Then
                w *= aspect
            Else
                h /= aspect
            End If

            ' Center the image in the rectangle
            Dim x As Integer = rect.X + (rect.Width - w) \ 2
            Dim y As Integer = rect.Y + (rect.Height - h) \ 2

            ' Draw the image
            g.DrawImage(If(e.Item.Enabled, e.Image, CreateDisabledImage(e.Image)), x, y, w, h)
        End If
    End Sub

End Class

Public Class ThemedToolStripColorTable : Inherits ProfessionalColorTable
    Private col As Color
    Public Sub New(col As Color)
        MyBase.New()
        Me.col = Color.FromArgb(col.A, col.R \ 2, col.G \ 2, col.B \ 2)
    End Sub

    Public Overrides ReadOnly Property MenuBorder As Color
        Get
            Return Me.col
        End Get
    End Property

    Public Overrides ReadOnly Property SeparatorDark As Color
        Get
            Return Me.col
        End Get
    End Property
   

    Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
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
            Return col
        End Get
    End Property

    Public Overrides ReadOnly Property MenuItemBorder As Color
        Get
            Return MyBase.MenuItemBorder
        End Get
    End Property


End Class


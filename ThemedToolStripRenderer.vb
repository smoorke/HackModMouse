Imports System.Drawing.Drawing2D

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



    Protected Overrides Sub OnRenderItemCheck(e As ToolStripItemImageRenderEventArgs)
        'Debug.Print("Rendering checkbox for: " & e.Item.Text)
        ' Your custom rendering code
    End Sub
    Protected Overrides Sub OnRenderItemImage(e As ToolStripItemImageRenderEventArgs)

        If Not DirectCast(e.Item, ToolStripMenuItem).Checked Then
            MyBase.OnRenderItemImage(e)
            Return
        End If

        '' Set the background color (e.g., LightCoral)
        'Using backgroundBrush As New SolidBrush(Color.Black)
        '    g.FillRectangle(backgroundBrush, rect)
        'End Using

        ' Draw the image over the custom background
        If e.Image IsNot Nothing Then

            Dim g As Graphics = e.Graphics
            Dim rect As Rectangle = e.ImageRectangle

            ' Set graphics modes for higher quality image rendering
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            ' Calculate aspect ratio
            Dim imgWidth As Integer = e.Image.Width
            Dim imgHeight As Integer = e.Image.Height
            Dim aspectRatio As Single = CSng(imgWidth) / imgHeight

            ' Calculate new dimensions while maintaining aspect ratio
            Dim drawWidth As Integer
            Dim drawHeight As Integer

            If aspectRatio > 1 Then
                ' Wider than tall
                drawWidth = rect.Width
                drawHeight = CInt(rect.Width / aspectRatio)
            Else
                ' Taller than wide
                drawHeight = rect.Height
                drawWidth = CInt(rect.Height * aspectRatio)
            End If

            ' Center the image in the rectangle
            Dim drawX As Integer = rect.X + (rect.Width - drawWidth) \ 2
            Dim drawY As Integer = rect.Y + (rect.Height - drawHeight) \ 2

            ' Draw the image
            g.DrawImage(e.Image, drawX, drawY, drawWidth, drawHeight)
        End If
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


Public Class ThemedRenderer
    Inherits ToolStripProfessionalRenderer

    Private col As Color

    Public Sub New(col As Color)
        MyBase.New(New ThemedToolStripColorTable(col))
        Me.col = col
    End Sub

    Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
        If Not e.Text.StartsWith(">>") OrElse Not e.Item.Enabled OrElse Not e.Item.Selected Then
            MyBase.OnRenderItemText(e)
            Return
        End If

        Dim item As ToolStripItem = e.Item
        Dim graphics As Graphics = e.Graphics
        Dim textColor As Color = e.TextColor
        Dim textFont As Font = e.TextFont
        'Dim italicFont As New Font(textFont, FontStyle.Italic)
        Dim text As String = e.Text
        Dim textRectangle As Rectangle = e.TextRectangle
        Dim textFormat As TextFormatFlags = e.TextFormat

        ' Define colors for different groups
        Dim colors As Color() = {Color.White,
                                 Color.FromArgb(&HFFFF8000),
                                 Color.FromArgb(&HFF1EFF00),
                                 Color.FromArgb(&HFF00FFFF),
                                 Color.FromArgb(&HFFFF00EC)}

        ' Draw each character individually with different colors
        Dim charHeight As Integer = 12 ' Approximate character height
        Dim charWidth As Integer = 9 ' Approximate character width
        Dim xPos As Integer = textRectangle.Left
        Dim yPos As Integer = (textRectangle.Top + textRectangle.Height) / 2 - charHeight / 2
        Dim colorIndex As Integer = -1

        For i As Integer = 0 To text.Length - 1
            Dim currentChar As Char = text(i)
            Dim drawColor As Color

            ' Determine color based on character type
            If Char.IsLetterOrDigit(currentChar) Then
                drawColor = colors(colorIndex)
            Else
                drawColor = colors(0)
                colorIndex += 1
            End If

            TextRenderer.DrawText(graphics, currentChar.ToString(), textFont, New Rectangle(xPos, yPos, charWidth, charHeight), drawColor, textFormat)

            ' Update xPos for the next character
            xPos += 7
        Next
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

            Dim img As Image = e.Image

            If Not e.Item.Enabled Then
                img = CreateDisabledImage(e.Image)
            Else
                If e.Item.Text.StartsWith(">>") AndAlso Not e.Item.Selected Then
                    img = img.AsGrayscale
                End If
            End If
            ' Draw the image
            g.DrawImage(img, x, y, w, h)
        End If
    End Sub

End Class

Module ImageExtension
    <Runtime.CompilerServices.Extension()>
    Public Function AsGrayscale(ByVal img As Image) As Image
        Try
            Dim bmp = New Bitmap(img.Width, img.Height)
            Using gfx = Graphics.FromImage(bmp), attr As New Imaging.ImageAttributes()
                ' Set grayscale matrix with hardcoded 75% brightness
                Dim grayscaleMatrix As New Imaging.ColorMatrix(
                    {
                        New Single() {0.3F, 0.3F, 0.3F, 0, 0}, ' Red channel
                        New Single() {0.59F, 0.59F, 0.59F, 0, 0}, ' Green channel
                        New Single() {0.11F, 0.11F, 0.11F, 0, 0}, ' Blue channel 
                        New Single() {0, 0, 0, 1, 0},   ' Alpha channel (unchanged)
                        New Single() {0, 0, 0, 0, 1}    ' No translation changes
                    })
                attr.SetColorMatrix(grayscaleMatrix)
                gfx.DrawImage(img, New Rectangle(0, 0, bmp.Width, bmp.Height),
                              0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attr)
            End Using
            Return bmp
        Catch ex As Exception
            Debug.Print($"Exception in AsGrayscaleWith75Brightness {ex.Message}")
            Return Nothing
        End Try
    End Function
End Module

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


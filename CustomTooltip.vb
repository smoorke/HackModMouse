Public Class CustomToolTip
    Inherits ToolTip

    Public font As Font
    Public text As String
    Public lines As Integer = 1
    Public currentowner As ToolStripDropDown
    Public currentitem As ToolStripItem
    Public Sub New()
        MyBase.New()
        Me.OwnerDraw = True
        Me.ShowAlways = True
        AddHandler Me.Popup, AddressOf Me.OnPopup
        AddHandler Me.Draw, AddressOf Me.OnDraw
    End Sub
    Dim delaysw As Stopwatch = Stopwatch.StartNew

    Public Shadows Sub Hide()
        If currentowner IsNot Nothing Then MyBase.Hide(currentowner)
    End Sub

    Public Sub ShowToolTip(item As ToolStripItem)

        Me.Hide()

        Dim tsdd As ToolStripDropDown = item.GetCurrentParent

        If tsdd Is Nothing OrElse String.IsNullOrEmpty(item.ToolTipText) Then
            delaysw.Restart()
            Exit Sub
        End If

        If delaysw.ElapsedMilliseconds < 250 Then
            Exit Sub
        End If
        delaysw.Restart()

        currentowner = tsdd  ' remember the control
        currentitem = item

        Dim offset = New Point(5, 5)

        ' Use the drop-down font
        Me.font = tsdd.Font
        Me.text = item.ToolTipText
        Me.lines = text.Count(Function(c) c = vbCr) + 1

        ' Get menu bounds in screen coordinates
        Dim menuBounds As Rectangle = tsdd.Bounds
        Dim screenArea As Rectangle = Screen.FromControl(tsdd).WorkingArea
        Dim screenBounds As Rectangle = Screen.FromControl(tsdd).Bounds

        ' Estimate tooltip size (can be adjusted later or made dynamic)
        If String.IsNullOrEmpty(text) Then
            Exit Sub
        End If

        ' Measure the width of each line
        Dim maxWidth As Integer = 0
        Using g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
            For Each line As String In text.Split({vbCrLf, vbLf}, StringSplitOptions.None)
                Dim size As SizeF = g.MeasureString(line, Me.font)
                If size.Width > maxWidth Then maxWidth = CInt(Math.Ceiling(size.Width))
            Next
        End Using

        Dim tooltipWidth As Integer = maxWidth + 10
        Dim tooltipHeight As Integer = (12 + 6 * lines) * scaling

        ' Start positioning to the right and below the menu
        Dim tooltipX As Integer = menuBounds.Right + offset.X
        Dim tooltipY As Integer = menuBounds.Top + offset.Y

        ' Adjust if going off the right edge
        If tooltipX + tooltipWidth > screenArea.Right Then
            tooltipX = menuBounds.Left - tooltipWidth - offset.X
        End If

        ' Adjust if going off bottom edge
        If tooltipY + tooltipHeight > screenArea.Bottom Then
            tooltipY = menuBounds.Bottom - tooltipHeight - offset.Y
        End If

        ' Adjust if going above top
        If tooltipY < screenArea.Top Then
            tooltipY = screenArea.Top + offset.Y
        End If

        ' Adjust for taskbar on top
        If screenArea.Top > screenBounds.Top Then
            tooltipY += screenArea.Top - screenBounds.Top
        End If

        ' Adjust for taskbar on right
        If screenArea.Right < screenBounds.Right Then
            tooltipX -= screenBounds.Right - screenArea.Right
        End If

        Dim pt = tsdd.PointToClient(New Point(tooltipX, tooltipY))

        ' Show tooltip at computed absolute position
        Me.Show(text, tsdd, pt.X, pt.Y)
    End Sub
    Private Sub OnPopup(ByVal sender As Object, ByVal e As PopupEventArgs)

        If String.IsNullOrEmpty(text) Then
            e.ToolTipSize = New Size(0, 0)
            e.Cancel = True
            Exit Sub
        End If
        ' Measure the width of each line
        Dim maxWidth As Integer = 0
        Using g As Graphics = Graphics.FromHwnd(e.AssociatedWindow.Handle)
            For Each line As String In text.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                Dim size As SizeF = g.MeasureString(line, Me.font)
                If size.Width > maxWidth Then maxWidth = CInt(Math.Ceiling(size.Width))
            Next
        End Using

        e.ToolTipSize = New Size(maxWidth + 10, (12 + 8 * lines) * scaling)
    End Sub

    Private Sub OnDraw(ByVal sender As Object, ByVal e As DrawToolTipEventArgs)

        Using g As Graphics = e.Graphics

            Using b As New Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 60, 60, 60), Color.Black, 0.0F)
                g.FillRectangle(b, e.Bounds)
            End Using

            g.DrawRectangle(New Pen(Brushes.Red, 1), New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1))
            Using bloombrush As New SolidBrush(Color.FromArgb(60, Color.HotPink))
                Dim sf As Integer = 1 * scaling
                g.DrawString(e.ToolTipText, Me.font, bloombrush, New PointF(e.Bounds.X + 5 - sf, e.Bounds.Y + 5 * scaling)) ' shadow layer
                g.DrawString(e.ToolTipText, Me.font, bloombrush, New PointF(e.Bounds.X + 5 + sf, e.Bounds.Y + 5 * scaling)) ' shadow layer
                g.DrawString(e.ToolTipText, Me.font, bloombrush, New PointF(e.Bounds.X + 5, e.Bounds.Y + 5 * scaling - sf)) ' shadow layer
                g.DrawString(e.ToolTipText, Me.font, bloombrush, New PointF(e.Bounds.X + 5, e.Bounds.Y + 5 * scaling + sf)) ' shadow layer
            End Using

            g.DrawString(e.ToolTipText, Me.font, If(currentitem.Enabled, Brushes.White, Brushes.DarkGray), New PointF(e.Bounds.X + 5, e.Bounds.Y + 5 * scaling)) ' top layer

        End Using
    End Sub
End Class
Module Globals

    Public mudproc As Process = Process.GetProcessesByName("hackmud_win").FirstOrDefault
    Public hackMudHandle As IntPtr = If(mudproc?.MainWindowHandle, IntPtr.Zero)
    Public ReadOnly mePid As Integer = Process.GetCurrentProcess.Id
    Public mH As MouseHook = New MouseHook
    Public ReadOnly Property cmsTray As ContextMenuStrip
        Get
            Return frmMain.cmsTray
        End Get
    End Property

    <Runtime.CompilerServices.Extension()>
    Public Sub Closer(menu As ContextMenuStrip)
        'this gets called in mousehook to close menu when clicking on hackmud
        If menu.Visible AndAlso Not menu.Bounds.Contains(Cursor.Position) AndAlso Not hasMouseRecurse(menu.Items) Then
            frmMain.LastClickedItem = Nothing
            menu.Close()
            If Not (My.Settings.xmbclick OrElse My.Settings.scrollActivate OrElse My.Settings.lcCompat) Then mH.UnhookMouse()
            frmMain.SetCursorVisibility(My.Settings.showcursor)
        End If
    End Sub
    Private Function hasMouseRecurse(collection As ToolStripItemCollection) As Boolean
        For Each menuitem As ToolStripMenuItem In collection.OfType(Of ToolStripMenuItem). 'skip separators
                Where(Function(m) m.HasDropDownItems AndAlso m.DropDown.Visible) 'only get items with a submenu that is open
            If menuitem.DropDown.Bounds.Contains(Cursor.Position) OrElse hasMouseRecurse(menuitem.DropDownItems) Then
                Return True
            End If
        Next
        Return False
    End Function

End Module

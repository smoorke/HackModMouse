Imports System.ComponentModel
Imports System.Drawing.Text

Public Class frmMain
    Private mH As MouseHook = New MouseHook
    Private appdataHMod As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "hackmod\")
    Private pfc As New PrivateFontCollection
#Region "StartupSequence"
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        SetMenuTheme(Color.FromArgb(&HFF7AB2F4))

        setFont()

        CursorMagic()

        SetCursorVisibility(My.Settings.showcursor)

        If My.Settings.xmbclick OrElse My.Settings.scrollActivate OrElse My.Settings.lcCompat Then mH.HookMouse()
        'these are off by default to have less false positives on viruscanners
        'note: if the mouse is hooked debugging lags the mouse a few seconds when hackmod enters break mode

    End Sub
    Private Sub frmMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        SetForegroundWindow(hackMudHandle)
    End Sub
#End Region

#Region "Cursor Magic"
    Private Sub tmrTick_Tick(sender As Object, e As EventArgs) Handles tmrTick.Tick 'interval 5077 ms
        If mudproc IsNot Nothing AndAlso mudproc.HasExited Then
            SysbootToolStripMenuItem.Enabled = True
            setGuiVfxBendToolStripMenuItem.Enabled = False
        End If
        CursorMagic()
        'Debug.Print($"{mudproc?.MainWindowTitle}:{mudproc?.Id}:{hackMudHandle}")
    End Sub

    Private Sub CursorMagic()

        Dim pp As Process = Process.GetProcessesByName("hackmud_win").FirstOrDefault

        If pp IsNot Nothing AndAlso String.IsNullOrWhiteSpace(pp.MainWindowTitle) Then pp = Nothing

        mudproc = pp
        hackMudHandle = If(mudproc?.MainWindowHandle, IntPtr.Zero)

        'set the parent to hackmud, note docs state you should use SetParent,
        '         however we don't do that to make ShowCursor work, why this works is a mystery to me.
        'to be honest i never managed to get SetParent to do the things i want in my other projects either.
        SetWindowLong(Me.Handle, GWL_HWNDPARENT, hackMudHandle)

    End Sub

    Private ShowValue As Integer
    'this only works because of the GWL_HWNDPARENT
    Private Sub SetCursorVisibility(visible As Boolean)
        ' ensure the cursor is visible if the handle is not valid
        If hackMudHandle = IntPtr.Zero Then visible = True

        ' set visibility and get the current state of the cursor
        ShowValue = ShowCursor(visible)
        Debug.Print($"visible:{visible} ShowValue:{ShowValue}")

        'this is a mess
        If visible Then
            ' ensure exact value
            Do While ShowValue <= 0
                ShowValue = ShowCursor(True)
                Debug.Print($"ensuring cursor visibility: {ShowValue}")
            Loop
            Do While ShowValue > 2
                ShowValue = ShowCursor(False)
                Debug.Print($"ensuring exact value: {ShowValue}")
            Loop
        Else
            ' ensure exact value
            Do While ShowValue > If(My.Settings.showcursor, 1, -1)
                ShowValue = ShowCursor(False)
                Debug.Print($"ensuring cursor invisibility: {ShowValue}")
            Loop
            Do While ShowValue < -2
                ShowValue = ShowCursor(True)
                Debug.Print($"ensuring exact value: {ShowValue}")
            Loop
        End If
    End Sub
#End Region

#Region "Hide from alt tab and Taskbar"
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or WindowStylesEx.WS_EX_TOOLWINDOW ' showintaskbar fails due to the GWL_HWNDPARENT
            Return cp
        End Get
    End Property
#End Region

#Region "Menu Theming"
    Private Sub SetMenuTheme(col As Color)
        cmsTray.Renderer = New ThemedRenderer(col)
        SetThemeRecurse(cmsTray.Items, col)
    End Sub
    Private Sub SetThemeRecurse(collection As ToolStripItemCollection, col As Color)
        For Each item As ToolStripMenuItem In collection.OfType(Of ToolStripMenuItem) ' skip separators
            item.ForeColor = col
            AddHandler item.MouseEnter, Sub(s As ToolStripMenuItem, e As EventArgs) s.ForeColor = Color.White
            AddHandler item.MouseLeave, Sub(s As ToolStripMenuItem, e As EventArgs) s.ForeColor = col
            If item.HasDropDown Then SetThemeRecurse(item.DropDownItems, col)
        Next
    End Sub
#End Region

#Region "setFont"
    Private Sub setFont()
        Try

            Dim fontpath = IO.Path.Combine(appdataHMod, "whitrabt.ttf")

            If Not IO.Directory.Exists(appdataHMod) OrElse Not IO.File.Exists(fontpath) Then
                'create directory. this can fail silently, we don't care.
                IO.Directory.CreateDirectory(appdataHMod)
                'write font to disk
                FileIO.FileSystem.WriteAllBytes(fontpath, My.Resources.whitrabt, False)
            End If

            pfc.AddFontFile(fontpath)
            Dim fnt As Font = New Font(pfc.Families(0), 9)

            'Set each menuitem to use font
            setFontRecurse(cmsTray.Items, fnt)

        Catch ex As Exception
            Debug.Print($"bab0 setting font {ex.Message}")
        End Try
    End Sub
    Private Sub setFontRecurse(collection As ToolStripItemCollection, fnt As Font)
        For Each item As ToolStripMenuItem In collection.OfType(Of ToolStripMenuItem) ' skip separators
            item.Font = fnt
            If item.HasDropDown Then setFontRecurse(item.DropDownItems, fnt)
        Next
    End Sub
#End Region

#Region "trayIcon and Contextmenu"
    Private Sub ExitToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles ExitToolStripMenuItem.Click
        mH.UnhookMouse()
        Me.Close()
    End Sub

    Private Sub cmsTray_Opening(sender As ContextMenuStrip, e As CancelEventArgs) Handles cmsTray.Opening
        SysbootToolStripMenuItem.Enabled = mudproc Is Nothing
        SetCursorVisibility(True)

        ' this is for when OOP crackers send esc followed by WM_ACTIVATE, SetForegroundWindow() or AppActivate()
        SendMessage(hackMudHandle, WM_ACTIVATE, 1, 0) 'prevents the menu from closing when the above happens
        ' Note: you need to unminimize hackmud before sending esc or it will fail
        '   in your OOG WM_ACTIVATE is preferred as it doesn't make hackmud pop to front nor steal focus
        ' Note: the closing is a sideffect of setting GWL_HWNDPARENT

        If IsIconic(hackMudHandle) Then SendMessage(hackMudHandle, WM_SYSCOMMAND, SC_RESTORE, 0)

        If mH.HookHandle = IntPtr.Zero Then mH.HookMouse() ' additional logic in mousehook to close menu when appropriate
    End Sub

    Private Sub cmsTray_Closed(sender As ContextMenuStrip, e As ToolStripDropDownClosedEventArgs) Handles cmsTray.Closed
        Debug.Print("systray closed")
        SetCursorVisibility(My.Settings.showcursor)
    End Sub
    Private Sub SysconfigureToolStripMenuItem_DropDownOpening(sender As ToolStripMenuItem, e As EventArgs) Handles SysconfigureToolStripMenuItem.DropDownOpening
        CursorshowToolStripMenuItem.Checked = My.Settings.showcursor
        LeftclickcompatToolStripMenuItem.Checked = My.Settings.lcCompat
        XmbclickToolStripMenuItem.Checked = My.Settings.xmbclick
        WheelScrollActivateToolStripMenuItem.Checked = My.Settings.scrollActivate

        setGuiVfxBendToolStripMenuItem.Enabled = mudproc IsNot Nothing

        For Each item As ToolStripItem In SysconfigureToolStripMenuItem.DropDownItems
            If TypeOf item Is ToolStripSeparator Then
                Exit For
            Else
                Dim mitem As ToolStripMenuItem = item
                mitem.Image = If(mitem.Checked, My.Resources.goodmud, Nothing)
            End If
        Next

    End Sub

    Private Sub SysconfigureItemToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles CursorshowToolStripMenuItem.Click, LeftclickcompatToolStripMenuItem.Click, XmbclickToolStripMenuItem.Click, WheelScrollActivateToolStripMenuItem.Click
        'toggle checkmark
        sender.Checked = Not sender.Checked
        'set settings and handle doing the stuff
        Select Case sender.Name
            Case CursorshowToolStripMenuItem.Name
                My.Settings.showcursor = sender.Checked
                SetCursorVisibility(sender.Checked)
            Case LeftclickcompatToolStripMenuItem.Name
                My.Settings.lcCompat = sender.Checked
            Case XmbclickToolStripMenuItem.Name
                My.Settings.xmbclick = sender.Checked
                If sender.Checked AndAlso mH.HookHandle = IntPtr.Zero Then mH.HookMouse()
            Case WheelScrollActivateToolStripMenuItem.Name
                My.Settings.scrollActivate = sender.Checked
                If sender.Checked AndAlso mH.HookHandle = IntPtr.Zero Then mH.HookMouse()
        End Select

        'counter-intuitively the click event fires after the closed event
        If Not (My.Settings.xmbclick OrElse My.Settings.scrollActivate OrElse My.Settings.lcCompat) Then mH.UnhookMouse()
    End Sub

    Private Sub SysbootToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles SysbootToolStripMenuItem.Click
        Dim pp As Process = Nothing
        Try
            pp = Process.Start("explorer", "steam://rungameid/469920")
        Catch ex As Exception
            Debug.Print($"bab0 starting hackmud {ex.Message}")
        Finally
            pp?.Dispose()
        End Try
    End Sub

    Private Sub TrayIcon_DoubleClick(sender As NotifyIcon, e As MouseEventArgs) Handles trayIcon.MouseDoubleClick
        If e.Button <> MouseButtons.Left Then Exit Sub

        If IsIconic(hackMudHandle) Then SendMessage(hackMudHandle, WM_SYSCOMMAND, SC_RESTORE, 0)

        SetForegroundWindow(hackMudHandle)
    End Sub

    Private Sub setGuiVfxBendToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles setGuiVfxBendToolStripMenuItem.Click

        'send esc
        SendMessage(hackMudHandle, WM_KEYDOWN, Keys.Escape, 1)  ' esc down
        SendMessage(hackMudHandle, WM_KEYUP, Keys.Escape, 1 << 31)    ' esc up
        SendMessage(hackMudHandle, WM_ACTIVATE, 1, 0)    ' needed for esc to take

        'send text
        Dim text = "gui.vfx{bend:0}"
        For i = 0 To text.Count() - 1
            PostMessage(hackMudHandle, WM_CHAR, Asc(text.Chars(i)), 0)
        Next

        'send enter
        SendMessage(hackMudHandle, WM_CHAR, Keys.Return, 0)

    End Sub



#End Region

End Class

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
    Private Sub tmrTick_Tick(sender As Object, e As EventArgs) Handles tmrTick.Tick 'interval 1337 ms
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
        SendMessage(hackMudHandle, WM_KEYDOWN, Keys.Escape, 1) ' esc down
        SendMessage(hackMudHandle, WM_KEYUP, Keys.Escape, 1 << 31)   ' esc up
#If DEBUG Then
        'redundant as the menu.opening already set hackmud as active but left in for completeness sake as OOG needs it for esc to take (does not work with hm minimized)
        ' see SendEsc for example code to handle unminimizing 
        SendMessage(hackMudHandle, WM_ACTIVATE, WA_ACTIVE, 0) ' needed for esc to take
#End If
        'send text
        Dim text = "gui.vfx{bend:0}"
        For i = 0 To text.Count() - 1
            PostMessage(hackMudHandle, WM_CHAR, Asc(text.Chars(i)), 0)
        Next

        'send enter
        SendMessage(hackMudHandle, WM_CHAR, Keys.Return, 0)

    End Sub

#End Region

#If DEBUG Then

#Region "SendEsc Example Code"
    Private Async Function SendEsc() As Task
        'Todo: find a way to send esc with hackmud minimized 'maybe a client bug?
        If IsIconic(hackMudHandle) Then
            'make sure window doesn't pop to front when doing showwindow
            SetWindowPos(hackMudHandle, SWP_HWND.BOTTOM, -1, -1, -1, -1,
                         SetWindowPosFlags.IgnoreMove Or
                         SetWindowPosFlags.IgnoreResize Or
                         SetWindowPosFlags.DoNotActivate)
            'restore the window w/o activating it
            ShowWindow(hackMudHandle, SW_SHOWNOACTIVATE)
        End If
        Await Task.Delay(33) 'there is a timing issue when following a PostMessage WM_CHAR
        SendMessage(hackMudHandle, WM_KEYDOWN, Keys.Escape, 1) ' esc down
        SendMessage(hackMudHandle, WM_KEYUP, Keys.Escape, 1 << 31)   ' esc up
        SendMessage(hackMudHandle, WM_ACTIVATE, WA_ACTIVE, 0) ' needed for esc to take. note: contrairy to other methods this doesn't bring hackmud to front
    End Function

#Region "NativeMethods"
    Enum SWP_HWND As Integer
        ''' <summary>
        ''' 1 Places the window at the bottom Of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status And Is placed at the bottom Of all other windows.
        ''' </summary>
        BOTTOM = 1
        ''' <summary> 
        ''' -2 Places the window above all non-topmost windows (that Is, behind all topmost windows). This flag has no effect If the window Is already a non-topmost window.
        ''' </summary>
        NOTOPMOST = -2
        ''' <summary>
        ''' 0 Places the window at the top Of the Z order. 
        ''' </summary>
        TOP = 0
        ''' <summary>
        ''' -1 Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated. 
        ''' </summary>
        TOPMOST = -1
    End Enum

    <Flags>
    Public Enum SetWindowPosFlags As UInteger
        ''' <summary>If the calling thread and the thread that owns the window are attached to different input queues,
        ''' the system posts the request to the thread that owns the window. This prevents the calling thread from
        ''' blocking its execution while other threads process the request.</summary>
        ''' <remarks>SWP_ASYNCWINDOWPOS</remarks>
        ASyncWindowPosition = &H4000
        ''' <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
        ''' <remarks>SWP_DEFERERASE</remarks>
        DeferErase = &H2000
        ''' <summary>Draws a frame (defined in the window's class description) around the window.</summary>
        ''' <remarks>SWP_DRAWFRAME</remarks>
        DrawFrame = &H20
        ''' <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
        ''' the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
        ''' is sent only when the window's size is being changed.</summary>
        ''' <remarks>SWP_FRAMECHANGED</remarks>
        FrameChanged = &H20
        ''' <summary>Hides the window.</summary>
        ''' <remarks>SWP_HIDEWINDOW</remarks>
        HideWindow = &H80
        ''' <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
        ''' top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        ''' parameter).</summary>
        ''' <remarks>SWP_NOACTIVATE</remarks>
        DoNotActivate = &H10
        ''' <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
        ''' contents of the client area are saved and copied back into the client area after the window is sized or
        ''' repositioned.</summary>
        ''' <remarks>SWP_NOCOPYBITS</remarks>
        DoNotCopyBits = &H100
        ''' <summary>Retains the current position (ignores X and Y parameters).</summary>
        ''' <remarks>SWP_NOMOVE</remarks>
        IgnoreMove = &H2
        ''' <summary>Does not change the owner window's position in the Z order.</summary>
        ''' <remarks>SWP_NOOWNERZORDER</remarks>
        DoNotChangeOwnerZOrder = &H200
        ''' <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
        ''' the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
        ''' window uncovered as a result of the window being moved. When this flag is set, the application must
        ''' explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
        ''' <remarks>SWP_NOREDRAW</remarks>
        DoNotRedraw = &H8
        ''' <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
        ''' <remarks>SWP_NOREPOSITION</remarks>
        DoNotReposition = &H200
        ''' <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
        ''' <remarks>SWP_NOSENDCHANGING</remarks>
        DoNotSendChangingEvent = &H400
        ''' <summary>Retains the current size (ignores the cx and cy parameters).</summary>
        ''' <remarks>SWP_NOSIZE</remarks>
        IgnoreResize = &H1
        ''' <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
        ''' <remarks>SWP_NOZORDER</remarks>
        IgnoreZOrder = &H4
        ''' <summary>Displays the window.</summary>
        ''' <remarks>SWP_SHOWWINDOW</remarks>
        ShowWindow = &H40
        ''' <summary>Undocumented</summary>
        ''' <remarks>SWP_NOCLIENTSIZE</remarks>
        NoClientSize = &H800
        ''' <summary>Undocumented</summary>
        ''' <remarks>SWP_NOCLIENTMOVE</remarks>
        NoClientMove = &H1000
        ''' <summary>Undocumented</summary>
        ''' <remarks>SWP_STATECHANGED</remarks>
        StateChanged = &H8000
    End Enum

    <Runtime.InteropServices.DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr,
                                 ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer,
                                 ByVal uFlags As SetWindowPosFlags) As Boolean
    End Function

    Public Const SW_HIDE = 0
    Public Const SW_SHOWNORMAL = 1
    Public Const SW_NORMAL = 1
    Public Const SW_SHOWMINIMIZED = 2
    Public Const SW_SHOWMAXIMIZED = 3
    Public Const SW_MAXIMIZE = 3
    Public Const SW_SHOWNOACTIVATE = 4
    Public Const SW_SHOW = 5
    Public Const SW_MINIMIZE = 6
    Public Const SW_SHOWMINNOACTIVE = 7
    Public Const SW_SHOWNA = 8
    Public Const SW_RESTORE = 9
    Public Const SW_SHOWDEFAULT = 10
    Public Const SW_FORCEMINIMIZE = 11
    <Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function ShowWindow(Hwnd As IntPtr, iCmdShow As Integer) As Integer : End Function

#End Region

#End Region

#End If

End Class

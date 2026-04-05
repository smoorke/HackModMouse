Imports System.ComponentModel
Imports System.Drawing.Text
Imports System.Runtime.InteropServices

Public Class frmMain

    Private MeTiD As UInteger = GetWindowThreadProcessId(Me.Handle, Nothing)

#Region "StartupSequence"
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        SetMenuTheme(Color.FromArgb(&HFF7AB2F4))
        AddHandler SysconfigureToolStripMenuItem.DropDown.Closing, AddressOf cmsTray_Closing

        ApplyScaling()

        CursorMagic()

        SetCursorVisibility(My.Settings.showcursor)

        If My.Settings.xmbclick OrElse My.Settings.scrollActivate OrElse My.Settings.lcCompat Then mH.HookMouse()
        ' these are off by default to have less false positives on viruscanners
        ' note: if the mouse is hooked debugging lags the mouse a few seconds when entering break mode

        If My.Settings.AutoBoot AndAlso mudproc Is Nothing AndAlso Not (My.Computer.Keyboard.ShiftKeyDown OrElse My.Computer.Keyboard.CtrlKeyDown) Then SysbootToolStripMenuItem.PerformClick()

    End Sub

    Protected Overloads Overrides ReadOnly Property ShowWithoutActivation() As Boolean
        Get
            Return True
        End Get
    End Property

#End Region

#Region "Scaling"
    Private Sub ApplyScaling(Optional pt As Point = Nothing)
        Dim dpi As UInteger = 96
        GetDpiForMonitor(MonitorFromPoint(New Point, 2), 0, dpi, dpi)
        scaling = CSng(dpi) / 96.0!
        setFont()
        cmsTray.ImageScalingSize = New Size(16 * scaling, 16 * scaling)
    End Sub
#End Region

#Region "Cursor Magic"
    Private Sub tmrTick_Tick(sender As Object, e As EventArgs) Handles tmrTick.Tick 'interval 1337 ms
        If mudproc IsNot Nothing AndAlso mudproc.HasExited Then
            SysbootToolStripMenuItem.Enabled = True
            GuiVfxBendToolStripMenuItem.Enabled = False
        End If
        CursorMagic()
        'Debug.Print($"{mudproc?.MainWindowTitle}:{mudproc?.Id}:{hackMudHandle}")
    End Sub

    Private Sub CursorMagic()

        Dim pp As Process = Process.GetProcessesByName("hackmud_win").FirstOrDefault

        If pp IsNot Nothing AndAlso String.IsNullOrWhiteSpace(pp.MainWindowTitle) Then pp = Nothing

        mudproc = pp
        hackMudHandle = If(mudproc?.MainWindowHandle, IntPtr.Zero)

        If hackMudHandle <> IntPtr.Zero Then
            AttachThreadInput(GetWindowThreadProcessId(hackMudHandle, Nothing), MeTiD, True)
        End If

    End Sub

    Private ShowValue As Integer
    Public Sub SetCursorVisibility(visible As Boolean)

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
            Do While ShowValue < -1
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
            cp.ExStyle = cp.ExStyle Or WindowStylesEx.WS_EX_TOOLWINDOW
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
    Private pfc As PrivateFontCollection

    Private Sub setFont()
        Try
            If pfc Is Nothing Then
                pfc = New PrivateFontCollection
                Dim data As IntPtr = Marshal.AllocCoTaskMem(My.Resources.whitrabt.Length)
                Marshal.Copy(My.Resources.whitrabt, 0, data, My.Resources.whitrabt.Length)
                AddFontMemResourceEx(data, My.Resources.whitrabt.Length, FR_PRIVATE, Nothing)
                pfc.AddMemoryFont(data, My.Resources.whitrabt.Length)
                Marshal.FreeCoTaskMem(data)
            End If

            'todo: set fontsize according to mainscreen scaling
            cmsTray.Font = New Font(pfc.Families(0), 12 * scaling, GraphicsUnit.Pixel)

        Catch ex As Exception
            Debug.Print($"bab0 setting font {ex.Message}")
        End Try
    End Sub
#End Region

#Region "trayIcon and Contextmenu"
    Private Sub ExitToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles ExitToolStripMenuItem.Click
        mH.UnhookMouse()
        Me.Close()
    End Sub

    Private Sub cmsTray_Opening(sender As ContextMenuStrip, e As CancelEventArgs) Handles cmsTray.Opening

        'this is needed to make DPI change message fire
        SetWindowPos(Me.Handle, SWP_HWND.TOPMOST, -1, -1, -1, -1, SetWindowPosFlags.IgnoreResize Or SetWindowPosFlags.IgnoreMove Or SetWindowPosFlags.DoNotActivate)

        SysbootToolStripMenuItem.Enabled = mudproc Is Nothing
        SetCursorVisibility(True)

        ' this is for when OOG crackers send esc followed by WM_ACTIVATE, SetForegroundWindow() or AppActivate()
        SendMessage(hackMudHandle, WM_ACTIVATE, 1, 0) 'prevents the menu from closing when the above happens
        ' Note: you need to unminimize hackmud before sending esc or it will fail
        '   in your OOG WM_ACTIVATE is preferred as it doesn't make hackmud pop to front nor steal focus
        '   note: some applications still don't play nice with this so reults may vary.

        If IsIconic(hackMudHandle) Then SendMessage(hackMudHandle, WM_SYSCOMMAND, SC_RESTORE, 0)

        If mH.HookHandle = IntPtr.Zero Then mH.HookMouse() ' additional logic in mousehook to close menu when appropriate
    End Sub

    Public LastClickedItem As ToolStripItem
    Private Sub CmsTray_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles cmsTray.ItemClicked, SysconfigureToolStripMenuItem.DropDownItemClicked
        LastClickedItem = e.ClickedItem
    End Sub

    Private Sub cmsTray_Closing(sender As Object, e As ToolStripDropDownClosingEventArgs) Handles cmsTray.Closing
        If TypeOf LastClickedItem Is ToolStripSeparator OrElse SysconfigureToolStripMenuItem.DropDownItems.Contains(LastClickedItem) Then
            e.Cancel = True
            Debug.Print($"Prevented closing due to {If(TypeOf LastClickedItem Is ToolStripSeparator, "seperator", "configItem")} click")
            LastClickedItem = Nothing
        End If
    End Sub

    Private Sub cmsTray_Closed(sender As ContextMenuStrip, e As ToolStripDropDownClosedEventArgs) Handles cmsTray.Closed
        Debug.Print($"systray closed {e.CloseReason}")
        SetCursorVisibility(My.Settings.showcursor)
    End Sub
    Private Sub SysconfigureToolStripMenuItem_DropDownOpening(sender As ToolStripMenuItem, e As EventArgs) Handles SysconfigureToolStripMenuItem.DropDownOpening

        AutoBootToolStripMenuItem.Checked = My.Settings.AutoBoot
        '-
        CursorshowToolStripMenuItem.Checked = My.Settings.showcursor
        LeftclickcompatToolStripMenuItem.Checked = My.Settings.lcCompat
        XmbclickToolStripMenuItem.Checked = My.Settings.xmbclick
        WheelScrollActivateToolStripMenuItem.Checked = My.Settings.scrollActivate
        '-
        GuiVfxBendToolStripMenuItem.Enabled = mudproc IsNot Nothing

        'scaling fix
        SetWindowPos(Me.Handle, SWP_HWND.TOPMOST, -1, -1, -1, -1, SetWindowPosFlags.IgnoreResize Or SetWindowPosFlags.IgnoreMove Or SetWindowPosFlags.DoNotActivate)
    End Sub

    Private Sub SysconfigureItemToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles AutoBootToolStripMenuItem.Click, CursorshowToolStripMenuItem.Click,
                                                            LeftclickcompatToolStripMenuItem.Click, XmbclickToolStripMenuItem.Click, WheelScrollActivateToolStripMenuItem.Click
        'toggle checkmark
        sender.Checked = Not sender.Checked
        'set settings 
        Select Case sender.Name
            Case AutoBootToolStripMenuItem.Name
                My.Settings.AutoBoot = sender.Checked
            Case CursorshowToolStripMenuItem.Name
                My.Settings.showcursor = sender.Checked
            Case LeftclickcompatToolStripMenuItem.Name
                My.Settings.lcCompat = sender.Checked
                'If sender.Checked AndAlso mH.HookHandle = IntPtr.Zero Then mH.HookMouse()
            Case XmbclickToolStripMenuItem.Name
                My.Settings.xmbclick = sender.Checked
                'If sender.Checked AndAlso mH.HookHandle = IntPtr.Zero Then mH.HookMouse()
            Case WheelScrollActivateToolStripMenuItem.Name
                My.Settings.scrollActivate = sender.Checked
                'If sender.Checked AndAlso mH.HookHandle = IntPtr.Zero Then mH.HookMouse()
        End Select

        'hook mouse if applicable 
        If mH.HookHandle = IntPtr.Zero AndAlso (My.Settings.lcCompat OrElse My.Settings.xmbclick OrElse My.Settings.scrollActivate) Then mH.HookMouse()
        'counter-intuitively the click event fires after the closed event
        Debug.Print("SysconfigureItem_Click")

        My.Settings.Save()

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

    Private Sub setGuiVfxBendToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles GuiVfxBendToolStripMenuItem.Click

        'todo: find a cleaner way to select shell input
        Dim curPos As Point = Cursor.Position
        Dim pt As New Point(100, 100) 'need to account for invis border and gui.size which alters it's size
        ClientToScreen(hackMudHandle, pt)
        Cursor.Position = pt
        SendMessage(hackMudHandle, WM_LBUTTONDOWN, 0, 0)
        Threading.Thread.Sleep(1) 'this is needed or we might get a dragbox on slower hardware
        SendMessage(hackMudHandle, WM_LBUTTONUP, 0, 0)
        Cursor.Position = curPos

        'send esc
        SendMessage(hackMudHandle, WM_KEYDOWN, Keys.Escape, 1) ' esc down
        SendMessage(hackMudHandle, WM_KEYUP, Keys.Escape, 1 << 31) ' esc up

        SendMessage(hackMudHandle, WM_ACTIVATE, WA_ACTIVE, 0) ' needed for esc to take, note: hm should aready be active so this isn't necessary but left in just in case

        'send text
        Dim text = "gui.vfx{bend:0}"
        For i = 0 To text.Count() - 1
            PostMessage(hackMudHandle, WM_CHAR, Asc(text.Chars(i)), 0)
        Next

        'send enter
        SendMessage(hackMudHandle, WM_CHAR, Keys.Return, 0)

    End Sub

#End Region

#Region "WndProc"
    Dim DisplayChangeBusy As Boolean = False
    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)
        If m.Msg = WM_DISPLAYCHANGE Then
            Debug.Print("WM_DISPLAYCHANGE")
            If Not DisplayChangeBusy Then
                DisplayChangeBusy = True
                Task.Run(Sub()
                             ApplyScaling()

                             'nudge blurry trayicon so it is sharp again
                             Threading.Thread.Sleep(4000)
                             trayIcon.Visible = False
                             trayIcon.Visible = True
                             DisplayChangeBusy = False
                         End Sub)
            End If
        End If
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

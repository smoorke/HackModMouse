Imports System.ComponentModel
Imports System.Drawing.Text

Public Class frmMain
    Private mH As MouseHook = New MouseHook
    Private appdataHMod As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "hackmod\")
    Private pfc As New PrivateFontCollection
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        'handle theming of the menu
        cmsTray.Renderer = New ToolStripProfessionalRenderer(New ThemedColorTable)
        cmsTray.ForeColor = Color.White

        setFont()

        Attach()

        If My.Settings.xmbclick Then mH.HookMouse() 'send left mousebutton instead of xmb
        'xmbclick is off by default to have less false positives on viruscanners
        'note: when this is enabled debugging lags the mouse a few seconds when hackmod is in break mode

        Try
            AppActivate(mudproc.Id)
        Catch ex As Exception
            Debug.Print("bab0")
        End Try

    End Sub

    Private Sub tmrTick_Tick(sender As Object, e As EventArgs) Handles tmrTick.Tick
        'Attach()
        Debug.Print($"{mudproc?.MainWindowTitle}:{mudproc?.Id}")
    End Sub

#Region "Magic Attach"
    Private Sub Attach()

        mudproc = Process.GetProcessesByName("hackmud_win").FirstOrDefault

        hackMudHandle = If(mudproc?.MainWindowHandle, IntPtr.Zero)

        'set the parent to hackmud, note docs state you should use SetParent
        '         however we don't do that to make ShowCursor work
        'if we don't check for the menu being open this bugs it
        If Not cmsTray.Visible Then SetWindowLong(Me.Handle, GWL_HWNDPARENT, hackMudHandle)

        'this only works because of the GWL_HWNDPARENT
        If My.Settings.showcursor AndAlso ShowCursor(True) > 1 Then
            Do
            Loop Until ShowCursor(False) <= 1 'we don't want to overflow when we call this func more than once
        End If
    End Sub
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or WindowStylesEx.WS_EX_TOOLWINDOW 'Do not show in taskbar. me.showintaskbar fails due to the GWL_HWNDPARENT
            Return cp
        End Get
    End Property
#End Region

#Region "Font Setters"
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
            Debug.Print($"Error setting font: {ex.Message}")
        End Try
    End Sub

    Private Sub setFontRecurse(collection As ToolStripItemCollection, fnt As Font)
        For Each menuitem As ToolStripMenuItem In collection.OfType(Of ToolStripMenuItem) 'skip separators
            menuitem.Font = fnt
            If menuitem.HasDropDown Then setFontRecurse(menuitem.DropDownItems, fnt)
        Next
    End Sub
#End Region

#Region "trayIcon and Contextmenu"
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        mH.UnhookMouse()
        Me.Close()
    End Sub

    Private Sub cmsTray_Opening(sender As Object, e As CancelEventArgs) Handles cmsTray.Opening
        SysbootToolStripMenuItem.Enabled = mudproc Is Nothing
    End Sub

    Private Sub SysconfigureToolStripMenuItem_DropDownOpening(sender As Object, e As EventArgs) Handles SysconfigureToolStripMenuItem.DropDownOpening
        CursorshowToolStripMenuItem.Checked = My.Settings.showcursor
        XmbclickToolStripMenuItem.Checked = My.Settings.xmbclick
    End Sub

    Private Sub SysconfigureItemToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles CursorshowToolStripMenuItem.Click, XmbclickToolStripMenuItem.Click
        'toggle checkmark
        sender.Checked = Not sender.Checked
        'set settings and handle doing the stuff
        Select Case sender.Name
            Case CursorshowToolStripMenuItem.Name
                My.Settings.showcursor = sender.Checked
                If Not sender.Checked Then
                    Do
                    Loop Until ShowCursor(False) <= 1 ' set to 0, ShowCursor returns the previous showcursor value
                Else
                    Do
                    Loop Until ShowCursor(True) >= 0 ' set to 1
                End If
            Case XmbclickToolStripMenuItem.Name
                My.Settings.xmbclick = sender.Checked
                If sender.Checked AndAlso mH.HookHandle = IntPtr.Zero Then
                    mH.HookMouse()
                ElseIf Not sender.Checked AndAlso mH.HookHandle <> IntPtr.zero Then
                    mH.UnhookMouse()
                End If
        End Select
    End Sub

    Private Sub SysbootToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SysbootToolStripMenuItem.Click
        Try
            Process.Start(New ProcessStartInfo("steam://rungameid/469920") With {.UseShellExecute = True})
        Catch ex As Exception
            Debug.Print($"Error starting hackmud {ex.Message}")
        End Try
    End Sub

    Private Sub cmsTray_Click(sender As Object, e As MouseEventArgs) Handles trayIcon.MouseDown
        If e.Button <> MouseButtons.Left Then Exit Sub
        Try
            If IsIconic(hackMudHandle) Then SendMessage(hackMudHandle, WM_SYSCOMMAND, SC_RESTORE, 0)
            AppActivate(mudproc.Id)
        Catch ex As Exception

        End Try
    End Sub
#End Region

End Class

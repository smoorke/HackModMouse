<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tmrTick = New System.Windows.Forms.Timer(Me.components)
        Me.trayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.cmsTray = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SysbootToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SysconfigureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CursorshowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LeftclickcompatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.XmbclickToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WheelScrollActivateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.setGuiVfxBendToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.cmsTray.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripSeparator1
        '
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New System.Drawing.Size(177, 6)
        ToolStripSeparator1.Tag = "DefRender"
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New System.Drawing.Size(177, 6)
        ToolStripSeparator2.Tag = "DefRender"
        '
        'ToolStripSeparator3
        '
        ToolStripSeparator3.Name = "ToolStripSeparator3"
        ToolStripSeparator3.Size = New System.Drawing.Size(186, 6)
        '
        'tmrTick
        '
        Me.tmrTick.Enabled = True
        Me.tmrTick.Interval = 5077
        '
        'trayIcon
        '
        Me.trayIcon.ContextMenuStrip = Me.cmsTray
        Me.trayIcon.Icon = CType(resources.GetObject("trayIcon.Icon"), System.Drawing.Icon)
        Me.trayIcon.Text = "hackmod"
        Me.trayIcon.Visible = True
        '
        'cmsTray
        '
        Me.cmsTray.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SysbootToolStripMenuItem, ToolStripSeparator1, Me.SysconfigureToolStripMenuItem, ToolStripSeparator2, Me.ExitToolStripMenuItem})
        Me.cmsTray.Name = "cmsTray"
        Me.cmsTray.Size = New System.Drawing.Size(181, 104)
        '
        'SysbootToolStripMenuItem
        '
        Me.SysbootToolStripMenuItem.Image = Global.HackMod.My.Resources.Resources.HackMod
        Me.SysbootToolStripMenuItem.Name = "SysbootToolStripMenuItem"
        Me.SysbootToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SysbootToolStripMenuItem.Tag = "DefRender"
        Me.SysbootToolStripMenuItem.Text = "sys.boot"
        '
        'SysconfigureToolStripMenuItem
        '
        Me.SysconfigureToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CursorshowToolStripMenuItem, Me.LeftclickcompatToolStripMenuItem, Me.XmbclickToolStripMenuItem, Me.WheelScrollActivateToolStripMenuItem, ToolStripSeparator3, Me.setGuiVfxBendToolStripMenuItem})
        Me.SysconfigureToolStripMenuItem.Image = Global.HackMod.My.Resources.Resources.mud
        Me.SysconfigureToolStripMenuItem.Name = "SysconfigureToolStripMenuItem"
        Me.SysconfigureToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SysconfigureToolStripMenuItem.Text = "sys.configure"
        '
        'CursorshowToolStripMenuItem
        '
        Me.CursorshowToolStripMenuItem.Name = "CursorshowToolStripMenuItem"
        Me.CursorshowToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.CursorshowToolStripMenuItem.Text = "cursor.manage{show}"
        '
        'LeftclickcompatToolStripMenuItem
        '
        Me.LeftclickcompatToolStripMenuItem.Name = "LeftclickcompatToolStripMenuItem"
        Me.LeftclickcompatToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.LeftclickcompatToolStripMenuItem.Text = "left.click{compat}"
        '
        'XmbclickToolStripMenuItem
        '
        Me.XmbclickToolStripMenuItem.Name = "XmbclickToolStripMenuItem"
        Me.XmbclickToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.XmbclickToolStripMenuItem.Text = "xmbutton.click{left}"
        '
        'WheelScrollActivateToolStripMenuItem
        '
        Me.WheelScrollActivateToolStripMenuItem.Name = "WheelScrollActivateToolStripMenuItem"
        Me.WheelScrollActivateToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.WheelScrollActivateToolStripMenuItem.Text = "wheel.scroll{activate}"
        '
        'setGuiVfxBendToolStripMenuItem
        '
        Me.setGuiVfxBendToolStripMenuItem.Image = Global.HackMod.My.Resources.Resources.HackMod
        Me.setGuiVfxBendToolStripMenuItem.Name = "setGuiVfxBendToolStripMenuItem"
        Me.setGuiVfxBendToolStripMenuItem.Size = New System.Drawing.Size(189, 22)
        Me.setGuiVfxBendToolStripMenuItem.Tag = "DefRender"
        Me.setGuiVfxBendToolStripMenuItem.Text = ">>gui.vfx{bend:0}"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ExitToolStripMenuItem.Image = Global.HackMod.My.Resources.Resources.badmud
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ExitToolStripMenuItem.Tag = ""
        Me.ExitToolStripMenuItem.Text = "hackmod.shutdown"
        Me.ExitToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(34, 32)
        Me.ControlBox = False
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMain"
        Me.Opacity = 0R
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "HackMod"
        Me.cmsTray.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tmrTick As Timer
    Friend WithEvents trayIcon As NotifyIcon
    Friend WithEvents cmsTray As ContextMenuStrip
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SysconfigureToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CursorshowToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents XmbclickToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SysbootToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents WheelScrollActivateToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LeftclickcompatToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents setGuiVfxBendToolStripMenuItem As ToolStripMenuItem
End Class

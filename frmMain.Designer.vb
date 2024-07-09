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
        Dim ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tmrTick = New System.Windows.Forms.Timer(Me.components)
        Me.trayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.cmsTray = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SysbootToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SysconfigureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CursorshowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.XmbclickToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WheelScrollActivateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LeftclickcompatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.cmsTray.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripMenuItem1
        '
        ToolStripMenuItem1.ForeColor = System.Drawing.SystemColors.ControlText
        ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        ToolStripMenuItem1.Size = New System.Drawing.Size(177, 6)
        '
        'ToolStripMenuItem2
        '
        ToolStripMenuItem2.ForeColor = System.Drawing.SystemColors.ControlText
        ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        ToolStripMenuItem2.Size = New System.Drawing.Size(177, 6)
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
        Me.cmsTray.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SysbootToolStripMenuItem, ToolStripMenuItem2, Me.SysconfigureToolStripMenuItem, ToolStripMenuItem1, Me.ExitToolStripMenuItem})
        Me.cmsTray.Name = "cmsTray"
        Me.cmsTray.Size = New System.Drawing.Size(181, 104)
        '
        'SysbootToolStripMenuItem
        '
        Me.SysbootToolStripMenuItem.Image = Global.HackMod.My.Resources.Resources.HackMod
        Me.SysbootToolStripMenuItem.Name = "SysbootToolStripMenuItem"
        Me.SysbootToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SysbootToolStripMenuItem.Text = "sys.boot"
        '
        'SysconfigureToolStripMenuItem
        '
        Me.SysconfigureToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CursorshowToolStripMenuItem, Me.LeftclickcompatToolStripMenuItem, Me.XmbclickToolStripMenuItem, Me.WheelScrollActivateToolStripMenuItem})
        Me.SysconfigureToolStripMenuItem.Image = Global.HackMod.My.Resources.Resources.gear_wheel
        Me.SysconfigureToolStripMenuItem.Name = "SysconfigureToolStripMenuItem"
        Me.SysconfigureToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SysconfigureToolStripMenuItem.Text = "sys.configure"
        '
        'CursorshowToolStripMenuItem
        '
        Me.CursorshowToolStripMenuItem.Name = "CursorshowToolStripMenuItem"
        Me.CursorshowToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.CursorshowToolStripMenuItem.Text = "cursor.manage{show:true}"
        '
        'XmbclickToolStripMenuItem
        '
        Me.XmbclickToolStripMenuItem.Name = "XmbclickToolStripMenuItem"
        Me.XmbclickToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.XmbclickToolStripMenuItem.Text = "xmbutton.click{left:true}"
        '
        'WheelScrollActivateToolStripMenuItem
        '
        Me.WheelScrollActivateToolStripMenuItem.Name = "WheelScrollActivateToolStripMenuItem"
        Me.WheelScrollActivateToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.WheelScrollActivateToolStripMenuItem.Text = "wheel.scroll{activate:true}"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Image = Global.HackMod.My.Resources.Resources.Close
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ExitToolStripMenuItem.Text = "hackmod.shutdown"
        '
        'LeftclickcompatToolStripMenuItem
        '
        Me.LeftclickcompatToolStripMenuItem.Name = "LeftclickcompatToolStripMenuItem"
        Me.LeftclickcompatToolStripMenuItem.Size = New System.Drawing.Size(213, 22)
        Me.LeftclickcompatToolStripMenuItem.Text = "left.click{compat:true}"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(114, 43)
        Me.ControlBox = False
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
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
End Class

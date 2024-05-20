Module Globals
    Public WithEvents mudproc As Process = Process.GetProcessesByName("hackmud_win").FirstOrDefault

    Public hackMudHandle As IntPtr = If(mudproc?.MainWindowHandle, IntPtr.Zero)

    Public Sub mudproc_exit(sender As Process, e As EventArgs) Handles mudproc.Exited
        hackMudHandle = Nothing
        mudproc.Dispose()
        mudproc = Nothing
    End Sub

End Module

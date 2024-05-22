Module Globals

    Public mudproc As Process = Process.GetProcessesByName("hackmud_win").FirstOrDefault
    Public hackMudHandle As IntPtr = If(mudproc?.MainWindowHandle, IntPtr.Zero)

End Module

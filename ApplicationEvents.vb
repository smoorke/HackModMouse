Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private Sub MyApplication_StartupNextInstance(sender As Object, e As StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
            If hackMudHandle <> IntPtr.Zero Then
                If IsIconic(hackMudHandle) Then SendMessage(hackMudHandle, WM_SYSCOMMAND, SC_RESTORE, 0)

                Dim foreThread As Integer = GetWindowThreadProcessId(GetForegroundWindow(), Nothing)
                Dim currentThread As Integer = GetCurrentThreadId()

                Try
                    AttachThreadInput(currentThread, foreThread, True)
                    SetForegroundWindow(hackMudHandle) 'w/o attaching to the fgw (explorer) this may fail.
                    SetActiveWindow(hackMudHandle)
                Finally
                    AttachThreadInput(currentThread, foreThread, False)
                End Try

            End If

            e.BringToForeground = False

            cmsTray.Tag = ToolStripDropDownDirection.Default
            cmsTray.Show(Cursor.Position)

        End Sub
        <DllImport("kernel32.dll")>
        Private Shared Function GetCurrentThreadId() As Integer : End Function
        <DllImport("user32.dll")>
        Public Shared Function SetActiveWindow(hWnd As IntPtr) As IntPtr : End Function
    End Class
End Namespace

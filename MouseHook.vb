
Imports System.Runtime.InteropServices

Public Class MouseHook : Implements IDisposable

    Private Const HC_ACTION As Integer = 0
    Private Const WH_MOUSE_LL As Integer = 14
    Private Const WM_MOUSEMOVE As Integer = &H200

    Public Structure MSLLHOOKSTRUCT
        Public pt As Point
        Public mousedata As Integer
        Public flags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure


    Public Delegate Function MouseHookCallBack(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As Integer

    <DllImport("Kernel32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function GetModuleHandle(ByVal ModuleName As String) As IntPtr : End Function

    <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function SetWindowsHookEx(idHook As Integer, HookProc As MouseHookCallBack,
           hInstance As IntPtr, ThreadId As Integer) As IntPtr : End Function

    <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function CallNextHookEx(hHook As IntPtr, nCode As Integer,
           wParam As IntPtr, lParam As IntPtr) As Integer : End Function

    <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function UnhookWindowsHookEx(hHook As IntPtr) As Boolean : End Function

    Public HookHandle As IntPtr = IntPtr.Zero

    Private Function MouseProc(
        ByVal nCode As Integer,
        ByVal wParam As IntPtr,
        ByVal lParam As IntPtr) As Integer
        If nCode <> HC_ACTION Then Return CallNextHookEx(HookHandle, nCode, wParam, lParam)

        If nCode = HC_ACTION AndAlso (My.Settings.xmbclick OrElse My.Settings.scrollActivate OrElse My.Settings.lcCompat OrElse cmsTray.Visible) Then

            Dim mhs As MSLLHOOKSTRUCT = Marshal.PtrToStructure(Of MSLLHOOKSTRUCT)(lParam)

            Select Case wParam.ToInt32()
                Case WM_LBUTTONDOWN, WM_RBUTTONDOWN, WM_MBUTTONDOWN
                    cmsTray.Closer() ' close the tricked menu properly when clicking hackmud
                Case WM_LBUTTONUP
                    If My.Settings.lcCompat AndAlso GetForegroundWindow() = hackMudHandle Then Threading.Thread.Sleep(16)
                Case WM_XBUTTONDOWN
                    If My.Settings.xmbclick Then ' send a left click
                        If (mhs.mousedata And &HFFFF0000) AndAlso WindowFromPoint(mhs.pt) = hackMudHandle AndAlso
                            mhs.flags = 0 Then 'don't click on injected event
                            Debug.Print($"xmbclick {mhs.flags} {mhs.mousedata}")
                            SendMessage(hackMudHandle, WM_LBUTTONDOWN, 0, 0)
                            Threading.Thread.Sleep(1) ' this is needed or we get a dragbox
                            SendMessage(hackMudHandle, WM_LBUTTONUP, 0, 0)
                        End If
                    End If

                    ' rarely other applications do not close their traymenu when using xmb on their respective window
                    ' i'm opting to follow the majority here and close it, the same applies for MBUTTONDOWN
                    cmsTray.Closer() ' close the tricked menu properly when clicking hackmud

                Case WM_MOUSEWHEEL
                    If My.Settings.scrollActivate AndAlso
                                GetForegroundWindow() <> hackMudHandle AndAlso
                                WindowFromPoint(mhs.pt) = hackMudHandle Then

                        'activate the window
                        'needs Task.Run or there is lag
                        Task.Run(Sub() SendMouseInput(MouseEventF.XDown Or MouseEventF.XUp, 2)) 'inject xmb to activate window

                        'SendMessage(hackMudHandle, WM_ACTIVATE, 1, 0) 'doesn't work

                        'SetForegroundWindow(hackMudHandle) 'doesn't work if not debugging

                        'Todo: find a way to scroll w/o activating if possible
#If debug Then
                        Dim delta As Integer = (mhs.mousedata And &HFFFF0000) >> 16
                        Debug.Print($"inactive scroll {delta}")
#End If
                    End If
            End Select

        End If

        Return CallNextHookEx(HookHandle, nCode, wParam, lParam)
    End Function

    Private mhCallBack As MouseHookCallBack = New MouseHookCallBack(AddressOf MouseProc)
    Private disposedValue As Boolean

    Public Sub HookMouse()
        HookHandle = SetWindowsHookEx(WH_MOUSE_LL, mhCallBack,
            GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0)
        If HookHandle = IntPtr.Zero Then Throw New System.Exception("Mouse hook bab0")
    End Sub

    Public Sub UnhookMouse()
        If HookHandle <> IntPtr.Zero Then
            UnhookWindowsHookEx(HookHandle)
            HookHandle = IntPtr.Zero
        End If
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            UnhookMouse()
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    Protected Overrides Sub Finalize()
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

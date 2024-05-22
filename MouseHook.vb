
Imports System.Runtime.InteropServices

Public Class MouseHook : Implements IDisposable

    Private Const HC_ACTION As Integer = 0
    Private Const WH_MOUSE_LL As Integer = 14
    Private Const WM_MOUSEMOVE As Integer = &H200

    Public Structure MSLLHOOKSTRUCT
        Public pt As Point
        Public mousedata As Long
        Public flags As Long
        Public time As Long
        Public dwExtraInfo As Long
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

    'Public hwnd As IntPtr = IntPtr.Zero

    Public HookHandle As IntPtr = IntPtr.Zero

    Private Function MouseProc(
        ByVal nCode As Integer,
        ByVal wParam As IntPtr,
        ByVal lParam As IntPtr) As Integer
        If nCode <> HC_ACTION Then Return CallNextHookEx(HookHandle, nCode, wParam, lParam)
        Try
            If nCode = HC_ACTION Then
                Select Case wParam.ToInt32()

                    Case WM_XBUTTONDOWN
                        If My.Settings.xmbclick Then
                            Dim mhs As MSLLHOOKSTRUCT = Marshal.PtrToStructure(Of MSLLHOOKSTRUCT)(lParam)
                            If (mhs.mousedata And &HFFFF0000) AndAlso WindowFromPoint(mhs.pt) = hackMudHandle Then
                                SendMessage(hackMudHandle, WM_LBUTTONDOWN, 0, 0)
                                Threading.Thread.Sleep(1) ' this is needed or we get a dragbox
                                SendMessage(hackMudHandle, WM_LBUTTONUP, 0, 0)
                            End If
                        End If

                    Case WM_MOUSEWHEEL
                        If My.Settings.scrollActivate AndAlso
                                GetForegroundWindow() <> hackMudHandle AndAlso
                                WindowFromPoint(Control.MousePosition) = hackMudHandle Then

                            Debug.Print($"inactive scroll {wParam}")
                            ' for some reason this always returns 552, i can't get the delta

                            SetForegroundWindow(hackMudHandle) ' this works but brings window to front

                            'SendMessage(hackMudHandle, WM_ACTIVATE, 1, 0) ' does not work
                            'SendMessage(hackMudHandle, WM_MOUSEWHEEL, wParam, lParam) ' does nothing

                            ' in wndproc the messages look like this
                            'WM_MOUSEWHEEL 0x0000020A &H0000020A w7864320 10618611 
                            'WM_MOUSEWHEEL 0x0000020A &H0000020A w-7864320 10553075

                        End If
                End Select
            End If
        Catch
            Debug.Print("error in mousehook")
        End Try
        Return CallNextHookEx(HookHandle, nCode, wParam, lParam)
    End Function

    Private mhCallBack As MouseHookCallBack = New MouseHookCallBack(AddressOf MouseProc)
    Private disposedValue As Boolean

    Public Sub HookMouse()
        HookHandle = SetWindowsHookEx(WH_MOUSE_LL, mhCallBack,
            GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0)
        If HookHandle = IntPtr.Zero Then Throw New System.Exception("Mouse hook failed")
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

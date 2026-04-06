Module DWMThumb
    <Flags()> Public Enum DwmThumbnailFlags As UInteger
        DWM_TNP_RECTDESTINATION = &H1
        DWM_TNP_RECTSOURCE = &H2
        DWM_TNP_OPACITY = &H4
        DWM_TNP_VISIBLE = &H8
        DWM_TNP_SOURCECLIENTAREAONLY = &H10
    End Enum

    <System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure DWM_THUMBNAIL_PROPERTIES
        Public dwFlags As DwmThumbnailFlags
        Public rcDestination As RECT
        Public rcSource As RECT
        Public opacity As Byte
        Public fVisible As Boolean
        Public fSourceClientAreaOnly As Boolean
    End Structure

    Public Declare Function DwmRegisterThumbnail Lib "dwmapi.dll" (ByVal Dest As IntPtr, ByVal Src As IntPtr, ByRef Thumb As IntPtr) As Integer
    Public Declare Function DwmUpdateThumbnailProperties Lib "dwmapi.dll" (ByVal hThumbnail As IntPtr, ByRef props As DWM_THUMBNAIL_PROPERTIES) As Integer
    Public Declare Function DwmUnregisterThumbnail Lib "dwmapi.dll" (ByVal Thumb As IntPtr) As Integer
    Public Declare Function DwmQueryThumbnailSourceSize Lib "dwmapi.dll" (hThumbnail As IntPtr, ByRef pSize As Size) As Integer
    <Runtime.InteropServices.DllImport("dwmapi.dll")>
    Public Function DwmFlush() As Integer : End Function
End Module

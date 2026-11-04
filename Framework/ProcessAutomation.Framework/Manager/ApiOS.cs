using ProcessAutomation.Framework.DTO.Constants;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessAutomation.Framework.Manager
{
    public class ApiOS
    {
        public const int WM_CLOSE = 0x10;
        public const int WM_SETTEXT = 0x000C;
        public const int WM_GETTEXT = 0x000D;
        public const int WM_GETTEXTLENGTH = 0x000E;
        public const int BM_CLICK = 0x00F5;

        public const uint CF_BITMAP = 2U;
        public const uint CF_TEXT = 1U;
        public const uint CF_UNICODETEXT = 13U;
        public const uint CF_HDROP = 15U;

        public const int SW_HIDE = 0;
        public const int SW_NORMAL = 1;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        public struct GdiplusStartupInput
        {
            public GdiplusStartupInput(int version)
            {
                GdiplusVersion = version;
                SuppressBackgroundThread = false;
                SuppressExternalCodecs = false;
            }
            public int GdiplusVersion;
            public bool SuppressBackgroundThread;
            public bool SuppressExternalCodecs;
        }

        public enum RasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public enum GetWindowType : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct DROPFILES
        {
            public uint pFiles;
            public int x;
            public int y;
            public int fNC;
            public int fWide;
        };
    
    #if WINDOWS

        [DllImport(FrameworkConstants.USER32_DLL, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(FrameworkConstants.USER32_DLL, EntryPoint = "FindWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(FrameworkConstants.USER32_DLL, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);

        [DllImport(FrameworkConstants.USER32_DLL, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

        [DllImport(FrameworkConstants.USER32_DLL, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Int32 GetWindowTextLength(IntPtr hWnd);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
        
        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern int GetDlgCtrlID(IntPtr hWnd);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        //[DllImport(Constants.USER32_DLL)]
        //public static extern string GetWindowText(IntPtr hWnd, LPSTR lpString, int nMaxCount);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(FrameworkConstants.USER32_DLL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(FrameworkConstants.USER32_DLL, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(FrameworkConstants.USER32_DLL, SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool SetWindowText(IntPtr hWnd, string text);

        [DllImport(FrameworkConstants.USER32_DLL, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDlgItem(IntPtr hWnd, IntPtr childhWnd);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern int SetDlgItemText(IntPtr hwnd, int id, string title);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr GetFocus();

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool IsIconic(IntPtr handle);

        // second overload of SendMessage
        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, out int wParam, out int lParam);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport(FrameworkConstants.USER32_DLL, SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport(FrameworkConstants.USER32_DLL, SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool SetClipboardData(uint uFormat, IntPtr data);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool EmptyClipboard();
        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport(FrameworkConstants.USER32_DLL)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport(FrameworkConstants.USER32_DLL, EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(FrameworkConstants.SHELL32_DLL)]
        public static extern int DragQueryFile(IntPtr hDrop, uint iFile, [Out] StringBuilder lpszFile, uint cch);

        [DllImport(FrameworkConstants.KERNEL32_DLL, SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport(FrameworkConstants.KERNEL32_DLL, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport(FrameworkConstants.KERNEL32_DLL, SetLastError = true)]
        public static extern int GlobalSize(IntPtr hMem);

        [DllImport(FrameworkConstants.KERNEL32_DLL)]
        static extern IntPtr GetCurrentThreadId();

        [DllImport(FrameworkConstants.GDIL32_DLL, ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport(FrameworkConstants.GDIL32_DLL)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport(FrameworkConstants.GDIL32_DLL, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(FrameworkConstants.GDIL32_DLL)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(FrameworkConstants.GDIL32_DLL)]
        public static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, IntPtr lpvBits);

        [DllImport(FrameworkConstants.GDIL32_DLL)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, RasterOperations dwRop);

        [DllImport(FrameworkConstants.GDILPLUS_DLL, CharSet = CharSet.Unicode)]
        public static extern int GdipCreateBitmapFromHBITMAP(IntPtr hbitmap, IntPtr hpalette, out IntPtr bitmap);

        [DllImport(FrameworkConstants.GDILPLUS_DLL, CharSet = CharSet.Unicode)]
        public static extern int GdipSaveImageToFile(IntPtr image, string filename, ref Guid classId, IntPtr encoderParams);

        [DllImport(FrameworkConstants.GDILPLUS_DLL, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern long GdiplusStartup(out IntPtr token, ref GdiplusStartupInput gdiplusStartupInput, out IntPtr gdiplusStartupOutput);

        [DllImport(FrameworkConstants.GDILPLUS_DLL)]
        public static extern void GdiplusShutdown(IntPtr token);

        [DllImport(FrameworkConstants.USER32_DLL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport(FrameworkConstants.USER32_DLL, CharSet = CharSet.Auto)]
        static public extern IntPtr GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        public static void Click(IntPtr hWnd)
        {
            SendMessage(hWnd, BM_CLICK, 0, null);
        }

        public static void Click(IntPtr hWnd, int objectId, bool asynchronous = false)
        {
            IntPtr boxHwnd = GetDlgItem(hWnd, objectId);

            if (!asynchronous)
            {
                SendMessage(boxHwnd, BM_CLICK, 0, null);
            }
            else
            {
                PostMessage(boxHwnd, BM_CLICK, 0, null);
            }
        }

        public static void SetText(IntPtr hWnd, int objectId, string text)
        {
            IntPtr boxHwnd = GetDlgItem(hWnd, objectId);
            SendMessage(boxHwnd, WM_SETTEXT, 0, text);
        }

        public static void SetText(IntPtr hWnd, string text)
        {
            SendMessage(hWnd, WM_SETTEXT, 0, text);
        }

        public static string GetText(IntPtr hWnd)
        {
            int titleSize = SendMessage(hWnd, WM_GETTEXTLENGTH, 0, null) + 1;
            StringBuilder title = new StringBuilder(titleSize + 1);
            SendMessage(hWnd, WM_GETTEXT, titleSize, title);
            return title.ToString();
        }

        public static List<IntPtr> GetAllChildrenWindowHandles(IntPtr hParent, int maxCount)
        {
            List<IntPtr> result = new List<IntPtr>();
            int ct = 0;
            IntPtr prevChild = IntPtr.Zero;
            IntPtr currChild = IntPtr.Zero;
            while (ct < maxCount)
            {
                currChild = FindWindowEx(hParent, prevChild, null, null);
                if (currChild == IntPtr.Zero)
                {
                    //prevChild = IntPtr.Add(prevChild, 1);
                    //++ct;
                    //continue;
                    break;
                }
                result.Add(currChild);
                prevChild = currChild;
                ++ct;
            }
            return result;
        }

        public static bool FileBrowserSetFile(string value)
        {
            bool result = false;
            //const int maxCount = 100;
            //const int textBoxId = 1148;
            //const int openButtonId = 1;
            //int countFind = 0;
            //string titleWindow = null;
            //IntPtr handleBrowseWindow = IntPtr.Zero;                        

            //while (handleBrowseWindow == IntPtr.Zero && countFind <= maxCount)
            //{
            //    handleBrowseWindow = ApiOS.FindWindow(null, "Abrir");
            //    if (handleBrowseWindow == IntPtr.Zero)
            //    {
            //        handleBrowseWindow = ApiOS.FindWindow(null, "Open");
            //    }

            //    Utilities.Sleep(0.5);
            //    countFind++;
            //}

            //titleWindow = ApiOS.GetText(handleBrowseWindow);

            //Utilities.Sleep(0.5);
            //ApiOS.SetText(handleBrowseWindow, textBoxId, value);

            //Utilities.Sleep(0.5);
            //ApiOS.Click(handleBrowseWindow, openButtonId, true);
            //Utilities.Sleep(0.5);

            //countFind = 0;
            //FILE_BROWSER:
            //handleBrowseWindow = ApiOS.FindWindow(null, titleWindow);
            //if (handleBrowseWindow != IntPtr.Zero || countFind <= maxCount)
            //{   
            //    ApiOS.SetForegroundWindow(handleBrowseWindow);

            //    SendKeys.SendWait("{ESC}");
            //    result = false;
            //    countFind++;
            //    Utilities.Sleep(0.5);
            //    goto FILE_BROWSER;
            //}

            //handleBrowseWindow = IntPtr.Zero;

            return result;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);
            return true;
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Callback childProc = new Win32Callback(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        public static string GetWinClass(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return null;
            StringBuilder classname = new StringBuilder(100);
            IntPtr result = GetClassName(hwnd, classname, classname.Capacity);
            if (result != IntPtr.Zero)
                return classname.ToString();
            return null;
        }

        public static IEnumerable<IntPtr> EnumAllWindows(IntPtr hwnd, string childClassName)
        {
            List<IntPtr> children = GetChildWindows(hwnd);
            if (children == null)
                yield break;
            foreach (IntPtr child in children)
            {
                if (GetWinClass(child) == childClassName)
                    yield return child;
                foreach (var childchild in EnumAllWindows(child, childClassName))
                    yield return childchild;
            }
        }

        public static IntPtr FocusedControlInActiveWindow(IntPtr activeWindowHandle)
        {
            IntPtr currentWindow = GetForegroundWindow();
            IntPtr activeWindowThread = GetWindowThreadProcessId(activeWindowHandle, IntPtr.Zero);
            IntPtr thisWindowThread = GetCurrentThreadId();
            IntPtr focusedControlHandle = IntPtr.Zero;

            if (AttachThreadInput(activeWindowThread, thisWindowThread, true) != IntPtr.Zero)
            {
                SetForegroundWindow(activeWindowHandle);
                focusedControlHandle = GetFocus();
                AttachThreadInput(activeWindowThread, thisWindowThread, false);
                SetForegroundWindow(currentWindow);
            }

            return focusedControlHandle;
        }
    #else
        public static IntPtr FindWindow(string a, string b) => IntPtr.Zero;
        public static IntPtr FindWindowEx(IntPtr a, IntPtr b, string c, string d) => IntPtr.Zero;
        public static bool SetForegroundWindow(IntPtr hWnd) => false;
        public static int GetWindowText(IntPtr a, StringBuilder b, int c) => 0;
        public static int GetWindowThreadProcessId(IntPtr a, out int b) { b = 0; return 0; }
        public static int GetWindowTextLength(IntPtr a) => 0;
        public static IntPtr GetDlgItem(IntPtr a, int b) => IntPtr.Zero;
        public static int PostMessage(IntPtr a, int b, int c, string d) => 0;
        public static int SendMessage(IntPtr a, int b, int c, string d) => 0;
        public static int SendMessage(IntPtr a, int b, int c, int d) => 0;
        public static bool SendMessage(IntPtr a, uint b, int c, StringBuilder d) => false;
        public static IntPtr SendMessage(int a, int b, int c, int d) => IntPtr.Zero;
        public static int GetDlgCtrlID(IntPtr a) => 0;
        public static IntPtr GetParent(IntPtr a) => IntPtr.Zero;
        public static IntPtr GetDC(IntPtr a) => IntPtr.Zero;
        public static int ReleaseDC(IntPtr a, IntPtr b) => 0;
        public static bool GetWindowRect(IntPtr a, out RECT b) { b = default; return false; }
        public static IntPtr GetForegroundWindow() => IntPtr.Zero;
        public static IntPtr GetWindow(IntPtr a, GetWindowType b) => IntPtr.Zero;
        public static IntPtr SetActiveWindow(IntPtr a) => IntPtr.Zero;
        public static bool SetWindowText(IntPtr a, string b) => false;
        public static IntPtr GetDlgItem(IntPtr a, IntPtr b) => IntPtr.Zero;
        public static int SetDlgItemText(IntPtr a, int b, string c) => 0;
        public static IntPtr GetFocus() => IntPtr.Zero;
        public static IntPtr SetFocus(IntPtr a) => IntPtr.Zero;
        public static IntPtr GetWindowThreadProcessId(IntPtr a, IntPtr b) => IntPtr.Zero;
        public static IntPtr AttachThreadInput(IntPtr a, IntPtr b, bool c) => IntPtr.Zero;
        public static bool ShowWindow(IntPtr a, int b) => false;
        public static bool IsIconic(IntPtr a) => false;
        public static int SendMessage(IntPtr a, uint b, out int c, out int d) { c = 0; d = 0; return 0; }
        public static IntPtr GetClipboardData(uint a) => IntPtr.Zero;
        public static bool IsClipboardFormatAvailable(uint a) => false;
        public static bool OpenClipboard(IntPtr a) => false;
        public static bool CloseClipboard() => false;
        public static bool SetClipboardData(uint a, IntPtr b) => false;
        public static bool EmptyClipboard() => false;
        public static bool PrintWindow(IntPtr a, IntPtr b, uint c) => false;
        public static IntPtr GetWindowDC(IntPtr a) => IntPtr.Zero;
        public static IntPtr GetDesktopWindow() => IntPtr.Zero;
        public static int DragQueryFile(IntPtr a, uint b, StringBuilder c, uint d) => 0;
        public static IntPtr GlobalLock(IntPtr a) => IntPtr.Zero;
        public static bool GlobalUnlock(IntPtr a) => false;
        public static int GlobalSize(IntPtr a) => 0;
        static IntPtr GetCurrentThreadId() => IntPtr.Zero;
        public static IntPtr SelectObject(IntPtr a, IntPtr b) => IntPtr.Zero;
        public static IntPtr CreateCompatibleBitmap(IntPtr a, int b, int c) => IntPtr.Zero;
        public static IntPtr CreateCompatibleDC(IntPtr a) => IntPtr.Zero;
        public static bool DeleteObject(IntPtr a) => false;
        public static IntPtr CreateBitmap(int a, int b, uint c, uint d, IntPtr e) => IntPtr.Zero;
        public static bool BitBlt(IntPtr a, int b, int c, int d, int e, IntPtr f, int g, int h, RasterOperations i) => false;
        public static int GdipCreateBitmapFromHBITMAP(IntPtr a, IntPtr b, out IntPtr c) { c = IntPtr.Zero; return 0; }
        public static int GdipSaveImageToFile(IntPtr a, string b, ref Guid c, IntPtr d) => 0;
        public static long GdiplusStartup(out IntPtr a, ref GdiplusStartupInput b, out IntPtr c) { a = IntPtr.Zero; c = IntPtr.Zero; return 0; }
        public static void GdiplusShutdown(IntPtr a) { }
        public static bool EnumChildWindows(IntPtr a, Win32Callback b, IntPtr c) => false;
        public static IntPtr GetClassName(IntPtr a, StringBuilder b, int c) => IntPtr.Zero;
        public static void Click(IntPtr hWnd) { }
        public static void Click(IntPtr hWnd, int objectId, bool asynchronous = false) { }
        public static void SetText(IntPtr hWnd, int objectId, string text) { }
        public static void SetText(IntPtr hWnd, string text) { }
        public static string GetText(IntPtr hWnd) => string.Empty;
        public static List<IntPtr> GetAllChildrenWindowHandles(IntPtr hParent, int maxCount) => null;
        public static bool FileBrowserSetFile(string value) => false;
        private static bool EnumWindow(IntPtr handle, IntPtr pointer) => false;
        public static List<IntPtr> GetChildWindows(IntPtr parent) => null;
        public static string GetWinClass(IntPtr hwnd) => string.Empty;
        public static IEnumerable<IntPtr> EnumAllWindows(IntPtr hwnd, string childClassName) => null;
        public static IntPtr FocusedControlInActiveWindow(IntPtr activeWindowHandle) => IntPtr.Zero;
    #endif
    }
}
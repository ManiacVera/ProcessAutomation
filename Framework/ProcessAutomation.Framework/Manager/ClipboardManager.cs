using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ProcessAutomation.Framework.Manager
{
    public class ClipboardManager
    {
        public static byte[] StructureToByte<T>(T structure)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, bufferIntPtr, true);
                Marshal.Copy(bufferIntPtr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferIntPtr);
            }
            return buffer;
        }

        public static List<string> GetFiles()
        {
            List<string> resultList = null;
            if (ApiOS.IsClipboardFormatAvailable(ApiOS.CF_HDROP) && ApiOS.OpenClipboard(IntPtr.Zero))
            {
                IntPtr handle = ApiOS.GetClipboardData(ApiOS.CF_HDROP);
                if (handle == IntPtr.Zero)
                    return null;

                IntPtr pointer = IntPtr.Zero;
                try
                {
                    pointer = ApiOS.GlobalLock(handle);
                    if (pointer == IntPtr.Zero)
                        return null;

                    int size = 0;
                    int filesCount = ApiOS.DragQueryFile(handle, uint.MaxValue, null, 0);
                    StringBuilder queryFileBuilder = new StringBuilder();
                    if (filesCount > 0)
                    {
                        resultList = new List<string>();
                        for (uint count = 0; count < filesCount; count++)
                        {
                            size = ApiOS.DragQueryFile(handle, count, null, 0) + 1;
                            ApiOS.DragQueryFile(handle, count, queryFileBuilder, (uint)size);
                            resultList.Add(queryFileBuilder.ToString());
                        }
                    }
                }
                finally
                {
                    if (pointer != IntPtr.Zero)
                        ApiOS.GlobalUnlock(handle);
                }
                ApiOS.CloseClipboard();
            }

            return resultList;
        }

        public static void SetFiles(List<string> pathList)
        {
            if (pathList != null && pathList.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < pathList.Count; i++)
                {
                    builder.Append(pathList[i]);
                    builder.Append('\0');
                }
                builder.Append('\0');
                string path = builder.ToString();
                
                if (ApiOS.OpenClipboard(IntPtr.Zero))
                {
                    int length = Marshal.SizeOf(typeof(ApiOS.DROPFILES));
                    IntPtr bufferPtr = Marshal.AllocHGlobal(length + path.Length * sizeof(char) + 8);
                    try
                    {
                        ApiOS.GlobalLock(bufferPtr);
                        ApiOS.DROPFILES config = new ApiOS.DROPFILES();
                        config.pFiles = (uint)length;
                        config.fNC = 1;
                        int seek = 0;
                        byte[] configData = StructureToByte(config);
                        for (int i = 0; i < configData.Length; i++)
                        {
                            Marshal.WriteByte(bufferPtr, seek, configData[i]);
                            seek++;
                        }
                        for (int i = 0; i < path.Length; i++)
                        {
                            Marshal.WriteInt16(bufferPtr, seek, path[i]);
                            seek++;
                        }
                        ApiOS.GlobalUnlock(bufferPtr);
                        ApiOS.EmptyClipboard();
                        ApiOS.SetClipboardData(ApiOS.CF_HDROP, bufferPtr);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        if (bufferPtr != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(bufferPtr);
                        }

                        ApiOS.CloseClipboard();
                    }
                }
            }
        }

        public static byte[] GetScreenshot()
        {
            byte[] data = null;
            IntPtr gdipToken = IntPtr.Zero; ;
            string fileName = string.Empty;

            try
            {
                if (ApiOS.IsClipboardFormatAvailable(ApiOS.CF_BITMAP) && ApiOS.OpenClipboard(IntPtr.Zero))
                {
                    IntPtr hGlobal = ApiOS.GetClipboardData(ApiOS.CF_BITMAP);
                    if (hGlobal != IntPtr.Zero)
                    {
                        IntPtr gdiplusStartupOutput;
                        ApiOS.GdiplusStartupInput input = new ApiOS.GdiplusStartupInput(1);
                        long num0 = ApiOS.GdiplusStartup(out gdipToken, ref input, out gdiplusStartupOutput);

                        IntPtr zero = IntPtr.Zero;
                        IntPtr palette = IntPtr.Zero;

                        if (ApiOS.GdipCreateBitmapFromHBITMAP(hGlobal, palette, out zero) == 0)
                        {
                            Guid classId = Guid.Parse("{557CF401-1A04-11D3-9A73-0000F81EF32E}");
                            fileName = System.IO.Path.GetTempFileName();

                            if (ApiOS.GdipSaveImageToFile(zero, fileName, ref classId, palette) == 0)
                            {
                                FileStream fs = File.OpenRead(fileName);
                                try
                                {
                                    data = new byte[fs.Length];
                                    fs.ReadExactly(data, 0, Convert.ToInt32(fs.Length));
                                }
                                finally
                                {
                                    fs.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(exp.ToString());
            }
            finally
            {
                if (gdipToken != IntPtr.Zero)
                {
                    ApiOS.GdiplusShutdown(gdipToken);
                    ApiOS.CloseClipboard();

                    if (!string.IsNullOrEmpty(fileName) && System.IO.File.Exists(fileName))
                        System.IO.File.Delete(fileName);
                }
            }

            return data;
        }

        public static void SetScreenshot(bool currentWindow = false)
        {
            if (ApiOS.OpenClipboard(IntPtr.Zero))
            {
                try
                {
                    IntPtr hWnd = IntPtr.Zero;

                    if (currentWindow)
                    {
                        hWnd = ApiOS.GetForegroundWindow();
                    }
                    else
                    {
                        hWnd = ApiOS.GetDesktopWindow();
                    }

                    IntPtr hWndDc = ApiOS.GetDC(hWnd);
                    IntPtr hMemDc = ApiOS.CreateCompatibleDC(hWndDc);

                    ApiOS.RECT rc;
                    ApiOS.GetWindowRect(hWnd, out rc);

                    int width = rc.Right - rc.Left;
                    int height = rc.Bottom - rc.Top;
                    IntPtr hBitmap = ApiOS.CreateCompatibleBitmap(hWndDc, width, height);
                    ApiOS.SelectObject(hMemDc, hBitmap);

                    ApiOS.BitBlt(hMemDc, 0, 0, width, height, hWndDc, 0, 0, ApiOS.RasterOperations.SRCCOPY);

                    ApiOS.EmptyClipboard();
                    ApiOS.SetClipboardData(ApiOS.CF_BITMAP, hBitmap);

                    ApiOS.DeleteObject(hBitmap);
                    ApiOS.ReleaseDC(hWnd, hWndDc);
                    ApiOS.ReleaseDC(IntPtr.Zero, hMemDc);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    ApiOS.CloseClipboard();
                }
            }
        }

        public static string GetText(bool unicode = true)
        {
            string result = null;

            uint cfTextType = unicode ? ApiOS.CF_UNICODETEXT : ApiOS.CF_TEXT;
            if (ApiOS.IsClipboardFormatAvailable(cfTextType) && ApiOS.OpenClipboard(IntPtr.Zero))
            {
                IntPtr handle = ApiOS.GetClipboardData(cfTextType);
                if (handle == IntPtr.Zero)
                    return null;

                IntPtr pointer = IntPtr.Zero;

                try
                {
                    pointer = ApiOS.GlobalLock(handle);
                    if (pointer == IntPtr.Zero)
                        return null;

                    int size = ApiOS.GlobalSize(handle);
                    byte[] buff = new byte[size];

                    Marshal.Copy(pointer, buff, 0, size);

                    result = Encoding.Unicode.GetString(buff).TrimEnd('\0');
                }
                finally
                {
                    if (pointer != IntPtr.Zero)
                    {
                        ApiOS.GlobalUnlock(handle);
                    }
                }
                ApiOS.CloseClipboard();
            }

            return result;
        }

        public static void SetText(string value, bool unicode = true)
        {
            if (value != null && ApiOS.OpenClipboard(IntPtr.Zero))
            {
                uint cfTextType = unicode ? ApiOS.CF_UNICODETEXT : ApiOS.CF_TEXT;

                IntPtr ptr = Marshal.StringToHGlobalUni(value);
                //EmptyClipboard();
                ApiOS.SetClipboardData(cfTextType, ptr);
                ApiOS.CloseClipboard();
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}

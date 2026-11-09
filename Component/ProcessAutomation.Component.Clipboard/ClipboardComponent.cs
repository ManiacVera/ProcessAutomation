using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProcessAutomation.Component
{
    public class ClipboardComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.FINISHED;
        private enum ClipboardActionEnum
        {
            COPY = 1,
            PASTE,
            GET_DATA
            //COPY_PASTE
        }

        private enum ClipboardTypeEnum
        {
            VALUE = 1,
            TEXT,
            //ALL_TEXT,
            FILE,
            //ALL_FILES,
            SCREENSHOT,
            SHORT_SCREENSHOT
        }

        private void CopyValue(string value)
        {
            ClipboardManager.SetText(value);
        }

        private void CopyText(string text)
        {
            // LUIS VERA PODRIA IR A ALGUNA VENTANA PARA COPIAR EL TEXTO SELECCIONADO AL PORTA PAPELES
            // DUDO SEA NO ES UTIL
            ClipboardManager.SetText(text);
        }

        private void CopyFile(string value)
        {
            //keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            //keybd_event(KEY_C, 0, KEYEVENTF_KEYDOWN, 0);
            //keybd_event(KEY_C, 0, KEYEVENTF_KEYUP, 0);
            //keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);

            if (!string.IsNullOrWhiteSpace(value))
            {
                string[] valueSplit = null;

                if (value.Contains(","))
                {
                    valueSplit = value.Split(new char[] { ',' });
                }
                else if (value.Contains("|"))
                {
                    valueSplit = value.Split(new char[] { '|' });
                }
                else if (value.Contains("\r\n"))
                {
                    valueSplit = value.Split("\r\n");
                }
                else if (value.Contains("\n"))
                {
                    valueSplit = value.Split("\n");
                }
                else if (value.Contains(";"))
                {
                    valueSplit = value.Split(new char[] { ';' });
                }
                else
                {
                    valueSplit = new string[] { value };
                }

                ClipboardManager.SetFiles(new List<string>(valueSplit));
                value = string.Join(";", valueSplit);
            }
            else
            {
                value = null;
            }
        }

        private void PasteText(string windowName)
        {
            //keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            //keybd_event(KEY_V, 0, KEYEVENTF_KEYDOWN, 0);
            //keybd_event(KEY_V, 0, KEYEVENTF_KEYUP, 0);
            //keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);

            string text = ClipboardManager.GetText();
            IntPtr hWndFinded = IntPtr.Zero;
            if (!string.IsNullOrEmpty(text))
            {
                IntPtr hWnd = IntPtr.Zero;
                bool childControlFocused = false;
                string className = null; //"Notepad";

                if (!string.IsNullOrWhiteSpace(className) || !string.IsNullOrWhiteSpace(windowName))
                {
                    if (className != null && className.Trim().Length == 0)
                    {
                        className = null;
                    }

                    if (windowName != null && windowName.Trim().Length == 0)
                    {
                        windowName = null;
                    }

                    hWndFinded = ApiOS.FindWindow(className, windowName);
                    if (hWndFinded != IntPtr.Zero)
                    {
                        hWnd = ApiOS.FocusedControlInActiveWindow(hWndFinded);
                        childControlFocused = true;
                    }
                }
                else
                {
                    hWnd = ApiOS.GetForegroundWindow();
                }

                bool isMinimized = false;
                IntPtr hWndMain = IntPtr.Zero;

                if (hWndFinded != IntPtr.Zero)
                {
                    hWndMain = hWndFinded;
                }
                else
                {
                    hWndMain = hWnd;
                }

                // LUIS VERA CUANDO ESTA MINIMIZADA LA VENTANA NO FUNCIONA
                if (ApiOS.IsIconic(hWndMain))
                {
                    isMinimized = true;
                    ApiOS.ShowWindow(hWndMain, ApiOS.SW_RESTORE);
                    ApiOS.ShowWindow(hWndMain, ApiOS.SW_SHOW);
                }

                if (childControlFocused)
                {
                    ApiOS.SendMessage(hWnd, ApiOS.WM_SETTEXT, text.Length, new StringBuilder(text));
                }
                else
                {
                    ApiOS.SetWindowText(hWnd, text);
                }

                if (isMinimized)
                {
                    ApiOS.ShowWindow(hWndMain, ApiOS.SW_MINIMIZE);
                }
            }
        }

        private void PasteFile(string path)
        {
            List<string> fileList = ClipboardManager.GetFiles();

            if (fileList != null && fileList.Count > 0)
            {
                bool withoutPath = false;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    path = path.Trim();
                }
                else
                {
                    withoutPath = true;
                }

                string newFile = string.Empty;
                int countFile = 0;
                FileInfo fileInfo = null;
                foreach (string file in fileList)
                {
                    if (File.Exists(file))
                    {
                        if (withoutPath)
                        {
                            fileInfo = new FileInfo(file);
                            path = fileInfo.DirectoryName;
                            fileInfo = null;
                        }

                        newFile = Path.Combine(path, Path.GetFileName(file));
                        countFile = 0;

                        while (true)
                        {
                            countFile++;
                            if (File.Exists(newFile))
                            {
                                newFile = string.Format("{0}{1}{2}",
                                    Path.Combine(path, Path.GetFileNameWithoutExtension(newFile)),
                                    countFile,
                                    Path.GetExtension(newFile));
                            }
                            else
                            {
                                break;
                            }                            
                        }

                        try
                        {
                            File.Copy(file, newFile);
                        }
                        catch 
                        {
                            throw;
                        }
                    }
                }
            }
        }

        private void PasteScreenshot()
        {
            //ClipboardManager.GetScreenshot();
            // LUIS VERA ESTA SOLO FUNCIONARIA CON PAINT U OTRA APLICACIÓN DE DISEÑO ¿NO CREO QUE SE USE?
        }
        
        private void Screenshot()
        {
            ClipboardManager.SetScreenshot();
        }

        private void ShortScreenshot()
        {
            ClipboardManager.SetScreenshot(true);
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            return true;
        }

        public void EndComponent()
        {

        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            ObjectType result = null;
            ClipboardDTO clipboardDTO = processBC.GetProcessedInputValue<ClipboardDTO>();

            if (clipboardDTO != null)
            {
                if (!string.IsNullOrWhiteSpace(clipboardDTO.Action))
                {
                    switch (Enum.Parse(typeof(ClipboardActionEnum), clipboardDTO.Action))
                    {
                        case ClipboardActionEnum.GET_DATA:
                            {
                                switch (Enum.Parse(typeof(ClipboardTypeEnum), clipboardDTO.Type))
                                {
                                    case ClipboardTypeEnum.TEXT:
                                        result = ClipboardManager.GetText();
                                        break;
                                    case ClipboardTypeEnum.FILE:
                                        {
                                            List<string> fileList = ClipboardManager.GetFiles();
                                            if (fileList != null && fileList.Count > 0)
                                            {
                                                result = new ListObjectType(fileList);
                                            }
                                        }
                                        break;
                                    case ClipboardTypeEnum.SCREENSHOT:
                                    case ClipboardTypeEnum.SHORT_SCREENSHOT:
                                        {
                                            byte[] screenshotByte = ClipboardManager.GetScreenshot();
                                            if (screenshotByte != null && screenshotByte.Length > 0)
                                            {
                                                result = new ObjectType(screenshotByte);
                                            }
                                        }
                                        break;
                                }
                            }
                            break;

                        case ClipboardActionEnum.COPY:
                            {
                                switch (Enum.Parse(typeof(ClipboardTypeEnum), clipboardDTO.Type))
                                {
                                    case ClipboardTypeEnum.VALUE:
                                        CopyValue(clipboardDTO.Value);
                                        break;
                                    case ClipboardTypeEnum.TEXT:
                                        CopyText(clipboardDTO.Value);
                                        break;
                                    case ClipboardTypeEnum.FILE:
                                        CopyFile(clipboardDTO.Value);
                                        break;
                                    case ClipboardTypeEnum.SCREENSHOT:
                                        Screenshot();
                                        break;
                                    case ClipboardTypeEnum.SHORT_SCREENSHOT:
                                        ShortScreenshot();
                                        break;
                                }
                            }
                            break;

                        case ClipboardActionEnum.PASTE:
                            {
                                switch (Enum.Parse(typeof(ClipboardTypeEnum), clipboardDTO.Type))
                                {
                                    case ClipboardTypeEnum.TEXT:
                                        PasteText(clipboardDTO.Value);
                                        break;
                                    case ClipboardTypeEnum.FILE:
                                        PasteFile(clipboardDTO.Value);
                                        break;
                                    case ClipboardTypeEnum.SCREENSHOT:
                                    case ClipboardTypeEnum.SHORT_SCREENSHOT:
                                        //ShortPrintScreen();
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                StepFail("Undefined value");
            }

            return result;
        }

        public bool InitComponent()
        {
            return true;
        }
    }
}

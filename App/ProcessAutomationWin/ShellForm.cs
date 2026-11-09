using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.Manager;
using ProcessAutomation.Process;
using ProcessAutomationWin.App.Windows.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAccessBridgeInterop;
using HWND = System.IntPtr;

namespace ProcessAutomationNetCore
{
    public partial class ShellForm : Form
    {
        AccessBridge accessBridge = new AccessBridge();

        public ShellForm()
        {
            InitializeComponent();
            this.Load += ShellForm_Load;
            this.Shown += ShellForm_Shown;
        }

        private void ShellForm_Load(object sender, EventArgs e)
        {
            //Windows_run();
            // ESTE METODO ES NECESARIO CORRERLO EN EL FORM LOAD

            try
            {
                accessBridge.Initialize();
            }
            catch { }
        }

        private string GetText(int JvmId, JavaObjectHandle context)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder();

            int caretIndex = 0;

            while (true)
            {
                AccessibleTextItemsInfo ti = new AccessibleTextItemsInfo();
                if (!accessBridge.Functions.GetAccessibleTextItems(JvmId, context, out ti, caretIndex))
                {
                    //throw new AccessibilityException("Error getting accessible text item information");
                    // Error
                }

                if (!string.IsNullOrEmpty(ti.sentence))
                    sbText.Append(ti.sentence);
                else
                    break;

                caretIndex = sbText.Length;

            }

            return sbText.ToString().TrimEnd();
        }

        private void initialTreeRefreshTimer_Tick(object sender, EventArgs e)
        {
            _initialTreeRefreshTimer.Stop();
            HWND window = ApiOS.FindWindow(null, "HDL Dump Helper GUI v2");

            return;

            //// DISABLED

            //var actionCall = new Action(() =>
            //{
            //    var ps2DumpWindow = accessBridge.CreateAccessibleWindow(window);
            //    if (ps2DumpWindow != null)
            //    {
            //        VisibleChildrenInfo visibleChildren;
            //        accessBridge.Functions.GetVisibleChildren(ps2DumpWindow.JvmId, ps2DumpWindow.AccessibleContextHandle, 0, out visibleChildren);
            //        {
            //            AccessibleContextInfo infoChild = null;
            //            foreach (var javaObjectHandleChild in visibleChildren.children)
            //            {
            //                accessBridge.Functions.GetAccessibleContextInfo(ps2DumpWindow.JvmId, javaObjectHandleChild, out infoChild);
            //                if (infoChild.description.Contains("Escribe aquí la Ip de la Ps2"))
            //                {
            //                    // .name es el label

            //                    string texto = GetText(ps2DumpWindow.JvmId, javaObjectHandleChild);
            //                    accessBridge.Functions.SetTextContents(ps2DumpWindow.JvmId, javaObjectHandleChild, "SI PUDE!!");
            //                }
            //                else if (infoChild.description.Contains("para buscar una Imagen-ISO"))
            //                {
            //                    AccessibleActions accesibleActions = null;
            //                    accessBridge.Functions.GetAccessibleActions(ps2DumpWindow.JvmId, javaObjectHandleChild, out accesibleActions);

            //                    AccessibleActionsToDo actionsToDo = new AccessibleActionsToDo();
            //                    actionsToDo.actions = new AccessibleActionInfo[256];
            //                    actionsToDo.actionsCount = accesibleActions.actionsCount;
            //                    actionsToDo.actions[0].name = "hacer clic";

            //                    try
            //                    {
            //                        accessBridge.Functions.DoAccessibleActions(ps2DumpWindow.JvmId, javaObjectHandleChild, ref actionsToDo, out int error);
            //                    }
            //                    catch (Exception ex)
            //                    {

            //                    }
            //                }
            //                else if (infoChild.description.Contains("Pulsa aquí para añadir el juego"))
            //                {

            //                }
            //                else if (infoChild.description.Contains("local con formato"))
            //                {
            //                    AccessibleActions accesibleActions = null;
            //                    accessBridge.Functions.GetAccessibleActions(ps2DumpWindow.JvmId, javaObjectHandleChild, out accesibleActions);

            //                    AccessibleActionsToDo actionsToDo = new AccessibleActionsToDo();
            //                    actionsToDo.actions = new AccessibleActionInfo[256];
            //                    actionsToDo.actionsCount = accesibleActions.actionsCount;
            //                    actionsToDo.actions[0].name = "hacer clic";

            //                    try
            //                    {
            //                        accessBridge.Functions.DoAccessibleActions(ps2DumpWindow.JvmId, javaObjectHandleChild, ref actionsToDo, out int error);
            //                    }
            //                    catch (Exception ex)
            //                    {

            //                    }
            //                }
            //                else if (infoChild.description.Contains("Por favor selecciona aquí un disco duro"))
            //                {
            //                    accessBridge.Functions.AddAccessibleSelectionFromContext(ps2DumpWindow.JvmId, javaObjectHandleChild, 1);
            //                }
            //            }
            //        }

            //        //AccessibleActions actions = null;
            //        //accessBridge.Functions.GetAccessibleActions(ps2DumpWindow.JvmId, ps2DumpWindow.AccessibleContextHandle, out actions);
            //        //{
            //        //    if (infoChild.description.Contains("Disco duro local"))
            //        //    {
            //        //        AccessibleActionsToDo actionsToDo = new AccessibleActionsToDo();
            //        //        actionsToDo.actions = new AccessibleActionInfo[1];
            //        //        actionsToDo.actions[0].name = "click";

            //        //        accessBridge.Functions.DoAccessibleActions(ps2DumpWindow.JvmId, javaObjectHandleChild, ref actionsToDo, out int error);
            //        //    }
            //        //}
            //    }
            //});

            //if (InvokeRequired)
            //{
            //    base.Invoke(actionCall);
            //}
            //else
            //{
            //    actionCall();
            //}
        }

        private void ShellForm_Shown(object sender, EventArgs e)
        {
            
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {

        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            ProcessForm newProcessForm = new ProcessForm();
            newProcessForm.ShowDialog(this);
        }

        private void functionMenuItem_Click(object sender, EventArgs e)
        {
            FunctionFormatForm newProcessForm = new FunctionFormatForm();
            newProcessForm.ShowDialog(this);
        }
    }
}

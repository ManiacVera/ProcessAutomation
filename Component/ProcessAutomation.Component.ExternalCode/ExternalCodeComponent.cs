using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProcessAutomation.Component
{
    public class ExternalCodeComponent : TaskComponentBaseBC, ITaskComponent
    {
        private enum LibraryTypeEnum 
        {
            NET = 1,
            NATIVE = 2
        }

        public ExternalCodeComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            if (componentInfo != null)
            {
                try
                {
                }
                catch (Exception)
                {
                }
            }

            return true;
        }


        //[DllImport("kernel32.dll")]
        //internal static extern IntPtr LoadLibrary(String dllname);
        //[DllImport("kernel32.dll")]

        //internal static extern IntPtr GetProcAddress(IntPtr hModule, String procName);

        //internal delegate bool DelegBeep(uint iFreq, uint iDuration);
        //static void ExecuteMethod(string library, string method, params object[] arguments)
        //{
        //    IntPtr kernel32 = LoadLibrary(library);
        //    IntPtr procBeep = GetProcAddress(kernel32, method);
        //    DelegBeep delegBeep = Marshal.GetDelegateForFunctionPointer(procBeep, typeof(DelegBeep)) as DelegBeep;
        //    delegBeep(100, 100);
        //}

        object ExecuteMethod(string library, string className, string method, params object[] arguments)
        {
            object result = null;
            Assembly dllAssembly = Assembly.LoadFile(library);

            if (dllAssembly != null)
            {
                Type type = dllAssembly.GetExportedTypes().FirstOrDefault(f => f.FullName == className || f.Name == className);
                if (type != null)
                {
                    var methodInstance = Activator.CreateInstance(type);
                    result = type.InvokeMember(method, BindingFlags.InvokeMethod, null, methodInstance, arguments);
                }
            }

            return result;
        }

        private IObjectType ProcessMethod()
        {
            IObjectType result = null;
            ExternalCodeDTO externalCodeDTO = processBC.GetProcessedInputValue<ExternalCodeDTO>();

            if (externalCodeDTO != null)
            {
                if (externalCodeDTO.LibraryType == LibraryTypeEnum.NET.ToString())
                {
                    List<object> fieldList = new List<object>();
                    if (externalCodeDTO.FieldList != null)
                    {
                        foreach (Dictionary<string, object> currentField in externalCodeDTO.FieldList)
                        {
                            fieldList.Add(currentField["Value"].ToString());
                        }
                    }

                    if (externalCodeDTO.Library != null && externalCodeDTO.Class != null && externalCodeDTO.Method != null)
                    {
                        object resultObject = ExecuteMethod(externalCodeDTO.Library, externalCodeDTO.Class, externalCodeDTO.Method, fieldList?.ToArray());

                        // LUIS VERA CUANDO LLEGA UNA LISTA, CONVERTIRLA A LIST<OBJECT> PERO SI ES UN DICCIONARIO, CONVERTIRLO A DICTIONARY<STRING, OBJECT>
                        //resultObject.GetType().Namespace != CommonConstants.SYSYTEM_COLLECTIONS_GENERIC)
                        if (resultObject.GetType() == typeof(string))
                        {
                            result = new TextType(resultObject.ToString());
                        }
                        else
                        {
                            result = new ObjectType(resultObject);
                        }

                        StepSuccess();
                    }
                    else
                    {
                        StepFail("No se encontró el metodo o librería a cargar");
                    }
                }
                else
                {
                    StepFail("Falta seleccionar el tipo de librería");
                }                
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            return ProcessMethod();
        }

        public void EndComponent()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Framework.DTO.Info
{
    public static class Constants
    {
        // Luis Vera este namespace debe de estar configurado en la base de datos
        public const string COMPONENT_NAMESPACE = "ProcessAutomation.Component";
        public const string GET_FORMAT_CONFIG_METHOD = "GetFormatConfig";
        public const string CONVERT_TO_FORMAT_CONFIG_METHOD = "ConvertToFormatConfigResult";
        public const string FORMAT_CONFIG_RESULT = "FORMAT_CONFIG_RESULT";

        public const string USER32_DLL = "user32.dll";
        public const string PATH_LOG_FILE = "ProcessAutomation.log";

        public const string CLICK = "CLICK";
        public const string ENTER = "ENTER";

        public const string ID = "ID";
        public const string NAME = "NAME";
        public const string TAG_NAME = "TAG_NAME";
        public const string CLASS_NAME = "CLASS_NAME";
        public const string CSS_SELECTOR = "CSS_SELECTOR";
        public const string TEXT = "TEXT";
        public const string CONTAINS = "CONTAINS";
        public const string XPATH = "XPATH";

        public const string FIND_BY_TEXT_FORMAT = "//*[text() = '{0}']";
        public const string FIND_BY_CONTAINS_FORMAT = "//*[contains(text(),'{0}')]";
        public const string GET_STEP_VALUE_FORMAT = "task{0}-step{1}";

        public const string GET_STEP_VALUE = "GET_STEP_VALUE";
        public const string GET_STEP_VALUE_SHORT = "GET_STEP_VALUE_SHORT";
        public const string GET_DATE = "GET_DATE";
        public const string GET_VARIABLE_VALUE = "GET_VARIABLE_VALUE";

        public const string GET_STEP_VALUE_SHORT_VARIABLE = "g";
        public const string GET_STEP_VALUE_VARIABLE = "getStepValue";
        public const string GET_DATE_VARIABLE = "getDate";

        // Luis Vera faltan formatos para tomar el valor a nivel tarea->proceso->paso
        public const string GET_STEP_VALUE_REGULAR_EXPRESSION_FORMAT = @"[@{{getStepValue:{{task:{0},step:{1}}}}}]";
        public const string GET_STEP_VALUE_SHORT_REGULAR_EXPRESSION_FORMAT = @"[@{{g:{{t:{0},s:{1}}}}}]";

        public const string GET_STEP_VALUE_REGULAR_EXPRESSION = @"\[@{getStepValue:{task:[0-9]+,step:[0-9]+}}\]";
        public const string GET_STEP_VALUE_SHORT_REGULAR_EXPRESSION = @"\[@{g:{t:[0-9]+,s:[0-9]+}}\]";        
        public const string GET_DATE_REGULAR_EXPRESSION = @"\[@{getDate:[a-zA-Z\/\""]+}\]";
        public const string GET_JSON_PROPERTY_EXPRESSION = @"\[@(.*?)\](\.[a-zA-Z\.\[(0-9)\]]+)";
        public const string GET_JSON_ARRAY_PROPERTY_EXPRESSION = @"[a-zA-Z]+(\[[0-9]+\])";
        public const string GET_JSON_ARRAY_VALUE_PROPERTY_EXPRESSION = @"(\[([0-9]+)\])";
        public const string GET_VARIABLE_VALUE_REGULAR_EXPRESSION = @"\[@(.*?)\]";
    }
}

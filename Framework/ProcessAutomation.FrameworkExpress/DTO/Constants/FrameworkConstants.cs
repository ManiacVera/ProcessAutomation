using System.ComponentModel;

namespace ProcessAutomation.Framework.DTO.Constants
{
    public static class FrameworkConstants
    {

        public const string SQLSERVER_DATABASE_CONNECTION_FORMAT = "Server={0},{1};DataBase={2};MultipleActiveResultSets=true;User ID={3};Password={4};TrustServerCertificate=true";
        public const string MYSQL_DATABASE_CONNECTION_FORMAT = "server={0};port={1};database={2};user={3};password={4};persistsecurityinfo=True;SslMode=none";
        public const string SQLITE_DATABASE_CONNECTION_FORMAT = "Data Source={0}";

        public const string PROCESS_AUTOMATION_TOKEN = "X-PA-TOKEN";
        public const string LOG_INFO_READ_DEPENDENCY = "Read dependency";
        public const string LOG_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string LOG_MESSAGE_FORMAT = "{{\n\t\"module\":\"{0}\",\n\t\"messageType\":\"{1}\",\n\t\"messsage\":\"{2}\",\n\t\"dateTime\":\"{3}\"\n}},\n";
        public const string KEY_ATTRIBUTE = "KeyAttribute";
        public const string IDENTIFIER_FIELD_ATTRIBUTE = "IdentifierFieldAttribute";
        public const string NAME_FIELD_ATTRIBUTE = "NameFieldAttribute";
        public const string CODE_FIELD_ATTRIBUTE = "CodeFieldAttribute";
        public const string NUMBER_FIELD_ATTRIBUTE = "NumberFieldAttribute";
        public const string DESCRIPTION_FIELD_ATTRIBUTE = "DescriptionFieldAttribute";

        public const string EQUAL_PASCAL = "Equal";
        public const string CONTAINS_PASCAL = "Contains";
        public const string NOT_EQUAL_PASCAL = "NotEqual";

        public const string ALTERATION_COUNTER = "Alteration_Counter";
        public const string CREATION_DATE = "Creation_Date";
        public const string MODIFICATION_DATE = "Modification_Date";

        // Luis Vera este namespace debe de estar configurado en la base de datos
        public const string COMPONENT_NAMESPACE = "ProcessAutomation.Component";
        //public const string GET_FORMAT_CONFIG_METHOD = "GetFormatConfig";
        //public const string CONVERT_TO_FORMAT_CONFIG_METHOD = "ConvertToFormatConfigResult";
        //public const string FORMAT_CONFIG_RESULT = "FORMAT_CONFIG_RESULT";

        public const string RUNTIMES_FOLDER = "runtimes";
        public const string LIB_FOLDER = "lib";
        public const string MAC_CATALYST_ARM64_FOLDER = "maccatalyst-arm64";
        public const string MAC_CATALYST_X64_FOLDER = "maccatalyst-x64";
        public const string OSX_ARM64_FOLDER = "osx-arm64";
        public const string OSX_X64_FOLDER = "osx-x64";
        public const string UNIX_FOLDER = "unix";
        public const string LINUX_FOLDER = "linux";
        public const string WIN_FOLDER = "win";
        public const string WIN_X64_FOLDER = "win-x64";
        public const string WIN_ARM64_FOLDER = "win-arm64";
        public const string NATIVE_FOLDER = "native";
        public const string PACKAGE_FOLDER = "Package";
        public const string USER32_DLL = "user32.dll";
        public const string SHELL32_DLL = "shell32.dll";
        public const string KERNEL32_DLL = "Kernel32.dll";
        public const string GDIL32_DLL = "gdi32.dll";
        public const string GDILPLUS_DLL = "gdiplus.dll";
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

        public const string SCRIPT_RETURN_RESULT = "scriptReturnResult";
    }
}

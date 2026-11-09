namespace ProcessAutomation.Common.DTO.Constants
{
    public static class CommonConstants
    {
        public const string APPLICATION_NAME = "ProcessAutomation";
        public const string DATABASES_NAME = "Databases";
        public const string SERVER_CONNECTION_FOLDER = "Config";
        public const string SERVER_CONNECTION_FILE = "ServerConnection.json";
        public const string USER_SETTINGS_FILE = "UserSettings.json";

        public const string PROCESS_AUTOMATION_INTERNAL = "ProcessAutomation";
        public const string PROCESS_AUTOMATION_SITE_INTERNAL = "ProcessAutomationSite";
        public const string PROCESS_VIEWER_INTERNAL = "ProcessViewerWin";
        public const string GRID = "GRID";
        public const string PROCESS_DATABASE_NAME_FORMAT = "{0}_STORE";
        public const string RUN_PROCESS_INFO = "RunProcessInfo";

        public const string COMMON_NAMESPACE = "ProcessAutomation.Common";
        public const string TASK_COMPONENT_INTERFACE = "ITaskComponent";
        public const string COMPONENT_SERVICE_INTERFACE = "IComponentService";
        public const string COMPONENT_DTO_INTERFACE = "IComponentDTO";
        public const string COMPONENT_SNIPPET_SOURCE_INTERFACE = "IComponentSnippetSource";
        public const string DLL_EXTENSION = ".dll";
        public const string JSON_EXTENSION = ".json";
        public const string HTML_EXTENSION = ".html";
        public const string COMMONS = "COMMONS";

        // Luis Vera este namespace debe de estar configurado en la base de datos
        public const string CONFIG_FOLDER = "Config";
        public const string EXTERNAL_FOLDER = "External";
        public const string RESOURCES_PAGES_FOLDER = "Resources/Pages";
        public const string COMPONENT_FOLDER = "Component";
        public const string COMPONENT = "Component";
        public const string PACKAGE_FOLDER = "Package";
        public const string BASE_RUNTIMES_FOLDER = "BaseRuntimes";
        public const string RUNTIMES_FOLDER = "runtimes";
        public const string DATABASE_FOLDER = "Database";
        public const string VIEWER_FOLDER = "Viewer";
        public const string SCRIPTS_FOLDER = "Scripts";
        public const string FORM_FOLDER = "Form";
        public const string PROGRAM_FILE = "Program.cs";
        public const string NEWTONSOFT_PACKAGE = "Microsoft.AspNetCore.Mvc.NewtonsoftJson";
        public const string NEWTONSOFT_PACKAGE_VERSION = "6.0.16";
        public const string COMPONENT_NAMESPACE = "ProcessAutomation.Component";
        public const string INTERFACE_DLL = "ProcessAutomation.Interface.dll";
        public const string COMMONS_API_ADAPTER_DLL = "ProcessAutomation.Api.Adapter.dll";
        public const string COMMONS_DLL = "ProcessAutomation.Common.dll";
        public const string COMMONS_DTO_DLL = "ProcessAutomation.Common.DTO.dll";
        public const string COMMONS_EXPRESS_DLL = "ProcessAutomation.CommonExpress.dll";
        public const string FRAMEWORK_DLL = "ProcessAutomation.Framework.dll";
        public const string FRAMEWORK_DTO_DLL = "ProcessAutomation.Framework.DTO.dll";
        public const string FRAMEWORK_EXPRESS_DLL = "ProcessAutomation.FrameworkExpress.dll";
        public const string BUSINESS_DLL = "ProcessAutomation.Business.dll";
        public const string BUSINESS_DTO_DLL = "ProcessAutomation.Business.DTO.dll";
        public const string DEPENDENCY_MODEL_DLL = "Microsoft.Extensions.DependencyModel.dll";
        public const string COMPONENT_STATUS_FIELD = "componentStatus";
        public const string COMPONENT_STATEMENT_FIELD = "componentStatement";        
        public const string WAIT_ASYNC_COMPONENT = "WaitAsyncComponent";
        public const string ASYNC_COMPONENT = "AsyncComponent";
        public const string FUNCTION_COMPONENT = "FunctionComponent";        
        public const string ENCAPSULATED_BEGIN = "BEGIN";
        public const string ENCAPSULATED_PARTITION = "PARTITION";
        public const string ENCAPSULATED_END = "END";
        public const string PIPE_HTML = "OPEN_DIALOG::";
        public const string PIPE_STRING = "SEND_STRING::";
        public const string PIPE_LOG_STRING = "SEND_LOG_STRING::";

        public const string USER32_DLL = "user32.dll";
        public const string PATH_LOG_FILE = "ProcessAutomation.log";
        public const string PATH_LOG_DATA_FILE = "ProcessAutomationData.log";
        //public const string STEP_LOG_FORMAT = "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}";
        //public const string STEP_HEADER_LOG_FORMAT = "Fecha|Process|Task|Number|Step|Variable|Message|ItsOk|StepValue|ProcessedValue";

        /// <summary>
        /// -n = Name Project
        /// -o = Folder Output
        /// </summary>
        public const string DOTNET_WEBAPI_PROJECT_FORMAT = "new webapi -n \"{0}\" -o \"{1}\"";
        public const string DOTNET_RESTORE_PROJECT_FORMAT = "restore \"{0}\"";
        public const string DOTNET_BUILD_PROJECT_FORMAT =  "build \"{0}\" -c {1}";
        public const string API_REST = "ApiRest";
        public const string API_REST_PROJECT = "ApiRest.csproj";

        public const string STEP_LOG_LINE_FORMAT = "Date: {0}, Task: {1}, StepNumber: {2}, StepName: {3}, Variable: {4}, ItsOk: {5}, KeyLog: {6}";
        public const string STEP_LOG_FORMAT = "{{\"Process\": \"{0}\", \"Task\": \"{1}\", \"StepNumber\": {2}, \"Step\": \"{3}\", \"Variable\": \"{4}\", \"Component\": \"{5}\", \"Message\": \"{6}\", \"ItsOk\": {7}, \r\n\"StepValue\": {8}, \r\n\"ProcessedValue\": {9}, \"OutputType\": \"{10}\", \"Required\": {11}, \r\n\"Fecha\": \"{12}\"}}";

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
        //public const string GET_STEP_VALUE_FORMAT = "task{0}-step{1}";
        public const string GET_STEP_VALUE_FORMAT = "T{0}V{1}";
        
        public const string TURN_ON = "TURN_ON";
        public const string TURN_OFF = "TURN_OFF";
        public const string GO_TO_STEP = "GO_TO_STEP";
        public const string INCREASE_COUNTER = "INCREASE_COUNTER";
        public const string DECREASE_COUNTER = "DECREASE_COUNTER";
        public const string PROCESS_COUNT = "PROCESS_COUNT";

        public const string DEFAULT_LANGUAGE = "es-MX";
        public const string I18N_FORMAT_JSON_FILE = "i18n_{0}.json";
        public const string INPUT_FORMAT_JSON_FILE = "InputFormat.json";
        public const string EXTERNAL_DEPENDENCY_JSON_FILE = "ExternalDependency.json";
        public const string STATEMENT_FORMAT_JSON_FILE = "StatementList.json";
        public const string INPUT_TEXT_FIELD_FILE = "InputTextField.html";
        public const string INPUT_FIELDS_FILE = "InputFields.html";
        public const string INPUT_FIELDS_VISOR_FILE = "InputFieldsVisor.html";
        public const string OUTPUT_VALUE_OPTION_FILE = "OutputValueOption.html";
        public const string COMMON_FORM_NAMESPACE = "ProcessAutomation.Common.Form";
        public const string PATH_FORM_TMP = "Form/tmp";
        public const string INPUT_FORMAT_VAR = "[@inputFormat]";
        public const string COMPONENT_NAME_VAR = "[@componentName]";
        public const string FIELD_NAME_VAR = "[@fieldName]";
        public const string ACTION_VAR = "[@action]";
        public const string VARIABLE_FORMAT = "[@{0}]";

        public const string SYSYTEM_COLLECTIONS_GENERIC = "System.Collections.Generic";

        public const string EXE_RUN_APP_FOLDER = "ExeRun";
        public const string OUTPUT_FOLDER = "Output";
        public const string PROCESS_FILE_FOLDER = "ProcessFiles";
        //public const string PROCESS_FILE_PUB_FOLDER = "ProcessFilesPub";
        public const string PUBLISHING_FOLDER = "Publishing";
        public const string PUBLISH_FOLDER = "Publish";
        public const string PROCESS_EXE_WIN= "ProcessExeWin.exe";
        public const string DEFAULT_FOLDER = "Default";
        public const string PROCESS_VIEWER_JSON_FILE = "ProcessViewer.json"; 
        public const string TEMPLATE_LAUNCHER_APPLICATION_NAME = "template_launcher";
        public const string RUN_PROCESS_APPLICATION_NAME = "RunProcessApp";
        public const string SHORTCUTS_FOLDER = "Shortcuts";
        public const string TEMPLATES_FOLDER = "Templates";
        public const string DEFAULT_USER_FOLDER = "DEFAULT_USER";

        public const string RESOURCES_FOLDER = "Resources";
        public const string CONFIG_JSON_FILE = "Config.json";
        public const string JOBJECT = "JOBJECT";
        public const string JARRAY = "JARRAY";

        public const string VARCHAR_20 = "VARCHAR(20)";
        public const string VARCHAR_30 = "VARCHAR(30)";
        public const string VARCHAR_50 = "VARCHAR(50)";
        public const string VARCHAR_100 = "VARCHAR(100)";
        public const string VARCHAR_1000 = "VARCHAR(1000)";
        public const string DATETIME = "DATETIME";
        public const string INT = "INT";

        #region KEY FIELD
        public const string ID_Token_Type = "ID_Token_Type";
        public const string Token_Type_Code = "Token_Type_Code";
        public const string ID_Status = "ID_Status";
        public const string Status_Code = "Status_Code";
        public const string Notification_Type_Code = "Notification_Type_Code";
        public const string ID_Language = "ID_Language";
        public const string Language_Code = "Language_Code";
        public const string ID_Device = "ID_Device";
        public const string Device_Code = "Device_Code";
        public const string ID_Module = "ID_Module";
        public const string Module_Code = "Module_Code";
        public const string ID_Page = "ID_Page";
        public const string Page_Code = "Page_Code";
        public const string ID_Page_View = "ID_Page_View";
        public const string Page_View_Code = "Page_View_Code";
        public const string ID_Email_Template = "ID_Email_Template";
        public const string Email_Template_Code = "Email_Template_Code";
        #endregion KEY FIELD
    }
}

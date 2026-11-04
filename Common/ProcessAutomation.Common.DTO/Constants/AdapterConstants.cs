namespace ProcessAutomation.Common.DTO.Constants
{
    public static class AdapterConstants
    {
        public const string PROCESS_CONTROLLER = "Process";
        public const string PROCESS_CLIENT_CONTROLLER = "ProcessClient";
        
        public const string INIT_PROCESS_METHOD = "InitProcess";
        public const string SET_DEFAULT_SETTINGS_METHOD = "SetDefaultSettings";        
        public const string GET_STEP_DESCRIPTION_PROCESS_METHOD = "GetStepDescription";
        public const string GET_PROGRESS_PROCESS_METHOD = "GetProgress";
        public const string GET_MAXIMUM_PROGRESS_PROCESS_METHOD = "GetMaximumProgress";
        public const string IS_RUNNING_PROCESS_METHOD = "IsRunning";
        public const string ABORT_PROCESS_METHOD = "AbortProcess";
        public const string SET_INPUT_VALUE_METHOD = "SetInputValue";
        public const string RUN_PROCESS_METHOD = "RunProcess";

        public const string GET_WORK_SPACE_DIRECTORY_PROCESS_VIEWER_METHOD = "GetWorkSpaceDirectory";
        public const string SET_WORK_SPACE_DIRECTORY_PROCESS_VIEWER_METHOD = "SetWorkSpaceDirectory";
        public const string RETRIEVE_LANGUAGE_LIST_PROCESS_VIEWER_METHOD = "RetrieveLanguageList";
        public const string RELOAD_LIST_PROCESS_VIEWER_METHOD = "ReloadList";
        public const string RETRIEVE_PROCESS_VIEWER_LIST_PROCESS_VIEWER_METHOD = "RetrieveProcessViewerList";
        public const string REGENERATE_CONFIG_PROCESS_VIEWER_METHOD = "RegenerateConfigFile";
        public const string STOP_PROGRAM_PROCESS_VIEWER_METHOD = "StopProgram";
        public const string SAVE_CONFIG_PROCESS_VIEWER_METHOD = "SaveConfig";
        public const string SAVE_PROCESS_VIEWER_ITEM_METHOD = "SaveProcessItem";
        public const string COPY_PROCESS_VIEWER_BY_NUMBER_METHOD = "CopyProcessByNumber";
        public const string CREATE_PROCESS_SHORTCUT_BY_NUMBER_METHOD = "CreateProcessShortcutByNumber";
        public const string DELETE_PROCESS_VIEWER_BY_NUMBER_METHOD = "DeleteProcessByNumber";
        public const string GET_PROGRESS_PROCESS_VIEWER_METHOD = "GetProgress";
        public const string RUN_PROCESS_LIST_VIEWER_METHOD = "RunProcessList";
        public const string RUN_PROCESS_VIEWER_METHOD = "RunProcess";
        public const string RUN_PROCESS_VIEWER_BY_NUMBER_METHOD = "RunProcessByNumber";
        public const string IS_RUNNING_PROCESS_VIEWER_METHOD = "IsRunning";
        public const string GET_MAXIMUM_PROGRESS_PROCESS_VIEWER_METHOD = "GetMaximumProgress";
        public const string GET_LOG_PROCESS_VIEWER_METHOD = "GetLog";
        public const string READ_PROCESS_FILE_PROCESS_VIEWER_METHOD = "ReadProcessFile";
        public const string SAVE_PROCESS_CONFIG_PROCESS_VIEWER_METHOD = "SaveProcessConfig";
        public const string TOKEN_REGISTER_METHOD = "Register";
        public const string TEST_PING_METHOD = "Ping";

        public const string RETRIEVE_PROCESS_LIST_METHOD = "RetrieveProcessList";
        public const string OPEN_PROCESS_METHOD = "OpenProcess";
        public const string SAVE_PROCESS_METHOD = "SaveProcess";
        public const string BUILD_PROCESS_METHOD = "BuildProcess";
        public const string BUILD_INPUT_FORMAT_METHOD = "BuildInputFormat";
        public const string BUILD_RESULT_VALUE_FORMAT_METHOD = "BuildResultValueFormat";

        public const string RETRIEVE_INPUT_FORMAT_METHOD = "RetrieveInputFormat";
        public const string RETRIEVE_FORM_FORMAT_METHOD = "RetrieveFormFormat";
        public const string RETRIEVE_TRANSLATION_FILE_METHOD = "RetrieveTranslationFile";

        public const string RETRIEVE_PROCESS_FILE_LIST_METHOD = "RetrieveProcessFileList";
        public const string RETRIEVE_COMPONENT_METHOD = "RetrieveComponent";
        public const string RETRIEVE_DECLARATION_TYPE_METHOD = "RetrieveDeclarationType";
        public const string RETRIEVE_VALUE_ACCESS_METHOD = "RetrieveValueAccess";
        public const string RETRIEVE_VALUE_TYPE_METHOD = "RetrieveValueType";
        public const string RETRIEVE_OUTPUT_VALUE_OPTION_METHOD = "RetrieveOutputValueOption";
        public const string RETRIEVE_LANGUAGE_LIST_PROCESS_METHOD = "RetrieveLanguageList";
        public const string RETRIEVE_VALUE_OUTPUT_METHOD = "RetrieveValueOutputType";
        public const string RETRIEVE_ENCAPSULATED_PARTITIONTYPE_METHOD = "RetrieveEncapsulatedPartitionType";

        public const string LOAD_USER_SETTINGS_METHOD = "LoadUserSettings";
        public const string SAVE_USER_SETTINGS_METHOD = "SaveUserSettings";
        public const string GET_DATA_PATH_METHOD = "GetDataPath";

        public const string USER_LOGIN_METHOD = "UserLogin";
        public const string USER_REGISTER_METHOD = "UserRegister";
        public const string USER_CHANGE_PASSWORD_METHOD = "UserChangePassword";
        public const string RETRIEVE_USER_BY_EMAIL_METHOD = "RetrieveUserByEmail";
        public const string USER_LOGOUT_METHOD = "UserLogout";

        public const string SHOW_STATIC_PARAM = "showStatic";
        public const string PROCESS_FILE_PARAM = "processFile";
        public const string USER_CODE_PARAM = "userCode";
        public const string BASE_DIRECTORY_PARAM = "baseDirectory";
        public const string OUTPUT_FOLDER_PARAM = "outputFolder";
        public const string USERNAME_OR_EMAIL_PARAM = "userNameOrEmail";
        public const string PASSWORD_PARAM = "password";
        public const string DEVICE_PARAM = "device";
        public const string DEVICE_CODE_PARAM = "deviceCode";
        public const string EMAIL_PARAM = "email";
        public const string NAME_PARAM = "name";
        public const string LASTNAME_PARAM = "lastname";
        public const string OLD_PASSWORD_PARAM = "oldPassword";
        public const string NEW_PASSWORD_PARAM = "newPassword";
        public const string TOKEN_PARAM = "token";
        public const string ID_USER_PARAM = "idUser";
        public const string LATITUDE_PARAM = "latitude";
        public const string LONGITUDE_PARAM = "longitude";

        public const string VALUE_PARAM = "value";
        public const string VALUE_DECODED_PARAM = "valueDecoded";
        public const string VALUE_ACTION_PARAM = "valueAction";
        public const string COMPONENT_PARAM = "component";
        public const string DESCRIPTION_PARAM = "description";
        public const string VARIABLE_LIST_PARAM = "variableList";
        public const string LANGUAGE_PARAM = "language";

        public const string ROOT_COMPONENT_FOLDER_PARAM = "rootComponentFolder";
        public const string COMPONENT_TYPE_PARAM = "componentType";
        public const string FORMAT_FILE_PARAM = "formatFile";
 
        public const string USER_LOGIN_ROUTE = $"{USER_LOGIN_METHOD}/{{{USERNAME_OR_EMAIL_PARAM}}}/{{{PASSWORD_PARAM}}}/{{{DEVICE_PARAM}}}";
        public const string USER_REGISTER_ROUTE = $"{USER_REGISTER_METHOD}/{{{EMAIL_PARAM}}}/{{{PASSWORD_PARAM}}}/{{{NAME_PARAM}}}/{{{LASTNAME_PARAM}}}";
        public const string USER_CHANGE_PASSWORD_ROUTE = $"{USER_CHANGE_PASSWORD_METHOD}/{{{EMAIL_PARAM}}}/{{{OLD_PASSWORD_PARAM}}}/{{{NEW_PASSWORD_PARAM}}}";
        public const string RETRIEVE_USER_BY_EMAIL_ROUTE = $"{RETRIEVE_USER_BY_EMAIL_METHOD}/{{{EMAIL_PARAM}}}";
        public const string USER_LOGOUT_ROUTE = $"{USER_LOGOUT_METHOD}/{{{TOKEN_PARAM}}}/{{{ID_USER_PARAM}}}/{{{DEVICE_CODE_PARAM}}}/{{{LATITUDE_PARAM}}}/{{{LONGITUDE_PARAM}}}";

        public const string INIT_PROCESS_ROUTE = $"{INIT_PROCESS_METHOD}";
        public const string SET_DEFAULT_SETTINGS_PROCESS_ROUTE = $"{SET_DEFAULT_SETTINGS_METHOD}";
        public const string GET_STEP_DESCRIPTION_PROCESS_ROUTE = $"{GET_STEP_DESCRIPTION_PROCESS_METHOD}";
        public const string GET_PROGRESS_PROCESS_ROUTE = $"{GET_PROGRESS_PROCESS_METHOD}";
        public const string GET_MAXIMUM_PROGRESS_PROCESS_ROUTE = $"{GET_MAXIMUM_PROGRESS_PROCESS_METHOD}";
        public const string IS_RUNNING_PROCESS_ROUTE = $"{IS_RUNNING_PROCESS_METHOD}";
        public const string ABORT_PROCESS_ROUTE = $"{ABORT_PROCESS_METHOD}";
        public const string SET_INPUT_VALUE_ROUTE = $"{SET_INPUT_VALUE_METHOD}";
        public const string RUN_PROCESS_ROUTE = $"{RUN_PROCESS_METHOD}";

        public const string GET_WORK_SPACE_DIRECTORY_PROCESS_VIEWER_ROUTE = $"{GET_WORK_SPACE_DIRECTORY_PROCESS_VIEWER_METHOD}";
        public const string SET_WORK_SPACE_DIRECTORY_PROCESS_VIEWER_ROUTE = $"{SET_WORK_SPACE_DIRECTORY_PROCESS_VIEWER_METHOD}";
        public const string RETRIEVE_LANGUAGE_LIST_PROCESS_VIEWER_ROUTE = $"{RETRIEVE_LANGUAGE_LIST_PROCESS_VIEWER_METHOD}";
        public const string RELOAD_LIST_PROCESS_VIEWER_ROUTE = $"{RELOAD_LIST_PROCESS_VIEWER_METHOD}";
        public const string RETRIEVE_PROCESS_VIEWER_LIST_PROCESS_VIEWER_ROUTE = $"{RETRIEVE_PROCESS_VIEWER_LIST_PROCESS_VIEWER_METHOD}";
        public const string REGENERATE_CONFIG_PROCESS_VIEWER_ROUTE = $"{REGENERATE_CONFIG_PROCESS_VIEWER_METHOD}";
        public const string IS_RUNNING_PROCESS_VIEWER_ROUTE = $"{IS_RUNNING_PROCESS_VIEWER_METHOD}";
        public const string STOP_PROGRAM_PROCESS_VIEWER_ROUTE = $"{STOP_PROGRAM_PROCESS_VIEWER_METHOD}";
        public const string SAVE_CONFIG_PROCESS_VIEWER_ROUTE = $"{SAVE_CONFIG_PROCESS_VIEWER_METHOD}";
        public const string SAVE_PROCESS_VIEWER_ITEM_ROUTE = $"{SAVE_PROCESS_VIEWER_ITEM_METHOD}";
        public const string COPY_PROCESS_VIEWER_BY_NUMBER_ROUTE = $"{COPY_PROCESS_VIEWER_BY_NUMBER_METHOD}";
        public const string CREATE_PROCESS_SHORTCUT_BY_NUMBER_ROUTE = $"{CREATE_PROCESS_SHORTCUT_BY_NUMBER_METHOD}";
        public const string DELETE_PROCESS_VIEWER_BY_NUMBER_ROUTE = $"{DELETE_PROCESS_VIEWER_BY_NUMBER_METHOD}";
        public const string GET_PROGRESS_PROCESS_VIEWER_ROUTE = $"{GET_PROGRESS_PROCESS_VIEWER_METHOD}";
        public const string RUN_PROCESS_LIST_VIEWER_ROUTE = $"{RUN_PROCESS_LIST_VIEWER_METHOD}";
        public const string RUN_PROCESS_VIEWER_ROUTE = $"{RUN_PROCESS_VIEWER_METHOD}";
        public const string RUN_PROCESS_VIEWER_BY_NUMBER_ROUTE = $"{RUN_PROCESS_VIEWER_BY_NUMBER_METHOD}";
        public const string GET_MAXIMUM_PROGRESS_PROCESS_VIEWER_ROUTE = $"{GET_MAXIMUM_PROGRESS_PROCESS_VIEWER_METHOD}";
        public const string GET_LOG_PROCESS_VIEWER_ROUTE = $"{GET_LOG_PROCESS_VIEWER_METHOD}";
        public const string READ_PROCESS_FILE_PROCESS_VIEWER_ROUTE = $"{READ_PROCESS_FILE_PROCESS_VIEWER_METHOD}/{{{PROCESS_FILE_PARAM}}}";
        public const string SAVE_PROCESS_CONFIG_PROCESS_VIEWER_ROUTE = $"{SAVE_PROCESS_CONFIG_PROCESS_VIEWER_METHOD}";

        public const string TOKEN_REGISTER_ROUTE = $"{TOKEN_REGISTER_METHOD}";        
        public const string TEST_PING_ROUTER = $"{TEST_PING_METHOD}";
        public const string RETRIEVE_PROCESS_LIST_ROUTE = $"{RETRIEVE_PROCESS_LIST_METHOD}/{{{USER_CODE_PARAM}}}/{{{BASE_DIRECTORY_PARAM}}}";
        public const string OPEN_PROCESS_ROUTE = $"{OPEN_PROCESS_METHOD}/{{{PROCESS_FILE_PARAM}}}/{{{USER_CODE_PARAM}}}/{{{BASE_DIRECTORY_PARAM}}}";
        public const string SAVE_PROCESS_ROUTE = SAVE_PROCESS_METHOD;
        public const string BUILD_PROCESS_ROUTE = BUILD_PROCESS_METHOD;
        public const string BUILD_INPUT_FORMAT_ROUTE = BUILD_INPUT_FORMAT_METHOD;
        public const string BUILD_RESULT_VALUE_FORMAT_ROUTE = BUILD_RESULT_VALUE_FORMAT_METHOD;
        public const string OPEN_PROCESS_ROUTE_FORMAT = $"{OPEN_PROCESS_METHOD}/{{0}}/{{1}}/{{2}}";
        public const string SAVE_PROCESS_ROUTE_FORMAT = SAVE_PROCESS_METHOD;
        public const string BUILD_PROCESS_ROUTE_FORMAT = BUILD_PROCESS_METHOD;
       
        public const string RETRIEVE_INPUT_FORMAT_EXTENDED_ROUTE = $"{RETRIEVE_INPUT_FORMAT_METHOD}/{{{COMPONENT_TYPE_PARAM}}}/{{{ROOT_COMPONENT_FOLDER_PARAM}}}";
        public const string RETRIEVE_INPUT_FORMAT_ROUTE = $"{RETRIEVE_INPUT_FORMAT_METHOD}/{{{COMPONENT_TYPE_PARAM}}}";
        public const string RETRIEVE_INPUT_FORMAT_ROUTE_FORMAT = $"{RETRIEVE_INPUT_FORMAT_METHOD}/{{0}}/{{1}}";

        public const string RETRIEVE_FORM_FORMAT_EXTENDED_ROUTE = $"{RETRIEVE_FORM_FORMAT_METHOD}/{{{FORMAT_FILE_PARAM}}}/{{{ROOT_COMPONENT_FOLDER_PARAM}}}";
        public const string RETRIEVE_FORM_FORMAT_ROUTE = $"{RETRIEVE_FORM_FORMAT_METHOD}/{{{FORMAT_FILE_PARAM}}}";
        public const string RETRIEVE_FORM_FORMAT_ROUTE_FORMAT = $"{RETRIEVE_FORM_FORMAT_METHOD}/{{0}}/{{1}}";

        public const string RETRIEVE_TRANSLATION_FILE_EXTENDED_ROUTE = $"{RETRIEVE_TRANSLATION_FILE_METHOD}/{{{COMPONENT_TYPE_PARAM}}}/{{{LANGUAGE_PARAM}}}";
        public const string RETRIEVE_TRANSLATION_FILE_ROUTE = $"{RETRIEVE_TRANSLATION_FILE_METHOD}/{{{COMPONENT_TYPE_PARAM}}}/{{{LANGUAGE_PARAM}}}/{{{ROOT_COMPONENT_FOLDER_PARAM}}}";
        public const string RETRIEVE_TRANSLATION_FILE_ROUTE_FORMAT = $"{RETRIEVE_TRANSLATION_FILE_METHOD}/{{0}}/{{1}}/{{2}}";

        public const string RETRIEVE_COMPONENT_ROUTE = RETRIEVE_COMPONENT_METHOD;
        public const string RETRIEVE_DECLARATION_TYPE_ROUTE = RETRIEVE_DECLARATION_TYPE_METHOD;
        public const string RETRIEVE_VALUE_ACCESS_ROUTE = RETRIEVE_VALUE_ACCESS_METHOD;
        public const string RETRIEVE_VALUE_TYPE_ROUTE = RETRIEVE_VALUE_TYPE_METHOD;
        public const string RETRIEVE_VALUE_OUTPUT_ROUTE = RETRIEVE_VALUE_OUTPUT_METHOD;
        public const string RETRIEVE_ENCAPSULATED_PARTITIONTYPE_ROUTE= RETRIEVE_ENCAPSULATED_PARTITIONTYPE_METHOD;
        public const string RETRIEVE_OUTPUT_VALUE_OPTION_ROUTE = RETRIEVE_OUTPUT_VALUE_OPTION_METHOD;
        public const string RETRIEVE_LANGUAGE_LIST_PROCESS_ROUTE = $"{RETRIEVE_LANGUAGE_LIST_PROCESS_METHOD}";
        public const string LOAD_USER_SETTINGS_ROUTE = $"{LOAD_USER_SETTINGS_METHOD}";
        public const string SAVE_USER_SETTINGS_ROUTE = $"{SAVE_USER_SETTINGS_METHOD}";
        public const string GET_DATA_PATH_ROUTE = $"{GET_DATA_PATH_METHOD}";

        public const string RETRIEVE_PROCESS_FILE_LIST_ROUTE = $"{RETRIEVE_PROCESS_FILE_LIST_METHOD}/{{{BASE_DIRECTORY_PARAM}}}/{{{OUTPUT_FOLDER_PARAM}}}";
        public const string RETRIEVE_PROCESS_FILE_LIST_ROUTE_FORMAT = $"{RETRIEVE_PROCESS_FILE_LIST_METHOD}/{{0}}/{{1}}";
        public const string RETRIEVE_COMPONENT_ROUTE_FORMAT = RETRIEVE_COMPONENT_METHOD;
        public const string RETRIEVE_DECLARATION_TYPE_ROUTE_FORMAT = RETRIEVE_DECLARATION_TYPE_METHOD;
        public const string RETRIEVE_VALUE_ACCESS_ROUTE_FORMAT = RETRIEVE_VALUE_ACCESS_METHOD;
        public const string RETRIEVE_VALUE_TYPE_ROUTE_FORMAT = RETRIEVE_VALUE_TYPE_METHOD;
        public const string RETRIEVE_VALUE_OUTPUT_ROUTE_FORMAT = RETRIEVE_VALUE_OUTPUT_METHOD;
        public const string RETRIEVE_ENCAPSULATED_PARTITIONTYPE_ROUTE_FORMAT = RETRIEVE_ENCAPSULATED_PARTITIONTYPE_METHOD;
    }
}

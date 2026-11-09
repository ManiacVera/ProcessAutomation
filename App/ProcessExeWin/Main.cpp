#include <windows.h>
#include <string>

std::string GetFileName(const std::string& str)
{
    size_t found = str.find_last_of("/\\");
    std::string path = str.substr(found + 1);
    return path;
}

std::string RemoveExtensionFileName(const std::string& fullName)
{
    size_t lastindex = fullName.find_last_of(".");
    std::string rawname = fullName.substr(0, lastindex);
    return rawname;
}

//int WINAPI WinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPSTR lpCmdLine, _In_ int nCmdShow)
int main()
{
    char exeFullPath[MAX_PATH];
    GetModuleFileNameA(NULL, exeFullPath, MAX_PATH);
    std::string exeName = GetFileName(exeFullPath);
    std::string exeNameWithouthExtension = RemoveExtensionFileName(exeName);

    // GET COMMAND LINE PARAMETERS
    //int nArgs;
    //LPWSTR* szArglist = CommandLineToArgvW(GetCommandLineW(), &nArgs);
    //if (szArglist != NULL)
    //{
    //    int bufLen = nArgs * sizeof(LPSTR);
    //    LPSTR buffer = NULL; //((LPSTR)result) + numArgs * sizeof(LPSTR);

    //    BOOL lpUsedDefaultChar = FALSE;
    //    int retval = WideCharToMultiByte(CP_ACP, 0, szArglist[1], -1, buffer, 0, NULL, &lpUsedDefaultChar);

    //    LocalFree(szArglist);
    //}
    
    DWORD written = 0;
    const int bufferSize = 1000;
    char* buffer = (char*)malloc(bufferSize);
    memset(buffer, 0, bufferSize);
    wsprintfA(buffer, "/C dotnet \"%s.dll\"", exeNameWithouthExtension.c_str());
    ShellExecuteA(0, "open", "cmd.exe", buffer, 0, SW_SHOW);

    return 1;
}


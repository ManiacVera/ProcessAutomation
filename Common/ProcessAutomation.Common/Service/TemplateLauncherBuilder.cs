#nullable enable

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

class CommandInfo
{
    public int Command { get; set; } = 0;
    public string PackageFolderPath { get; set; } = string.Empty;
    public string PublishedFolderPath { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
}

public static class TemplateLauncherBuilder
{
    private static readonly byte[] magic = Encoding.ASCII.GetBytes("LCHRDATA");
    private static readonly byte[] endMagic = Encoding.ASCII.GetBytes("LCHREND!");
    private const uint version = 1;

    private const int binaryPathSize = 2048;
    private const int encryptedPayloadSize = 2048;
    private const int launcherDataSize = 4120;

    public static string Build(string templatesFolder, string binaryPath, int command, string packagePath, 
        string publishedPath, string processNumber, string outputPath, string? pfxCertificatePath = null, string? pfxPassword = null)
    {
        ValidateInput(templatesFolder, nameof(templatesFolder));
        ValidateInput(binaryPath, nameof(binaryPath));
        ValidateInput(packagePath, nameof(packagePath));
        ValidateInput(publishedPath, nameof(publishedPath));
        ValidateInput(processNumber, nameof(processNumber));
        ValidateInput(outputPath, nameof(outputPath));

        ValidateFieldSize(binaryPath, binaryPathSize, nameof(binaryPath));

        var commandInfo = new CommandInfo
        {
            Command = command,
            PackageFolderPath = packagePath,
            PublishedFolderPath = publishedPath,
            Process = processNumber
        };

        string jsonPayload = JsonSerializer.Serialize(commandInfo);
        string encryptedPayload = Encryption.Encrypt(jsonPayload);

        ValidateFieldSize(encryptedPayload, encryptedPayloadSize, nameof(encryptedPayload));

        string templatePath = Path.Combine(templatesFolder, GetTemplateName());

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template not found: {templatePath}", templatePath);

        byte[] binary = File.ReadAllBytes(templatePath);
        int dataOffset = FindLauncherData(binary);

        ValidateLauncherData(binary, dataOffset);

        long originalSize = binary.LongLength;

        int offset = dataOffset + 16;
        WriteFixedBytes(binary, offset, binaryPathSize, binaryPath);
        offset += binaryPathSize;

        WriteFixedBytes(binary, offset, encryptedPayloadSize, encryptedPayload);
        offset += encryptedPayloadSize;

        if (binary.LongLength != originalSize)
            throw new InvalidOperationException("The executable size changed during patching.");

        string? outputDirectory = Path.GetDirectoryName(Path.GetFullPath(outputPath));

        if (!string.IsNullOrEmpty(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        string temporaryPath = outputPath + ".tmp";

        try
        {
            File.WriteAllBytes(temporaryPath, binary);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                File.SetUnixFileMode(temporaryPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                                                     UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                                                     UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
            }

            File.Move(temporaryPath, outputPath, true);
        }
        catch
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);

            throw;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && 
            outputPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && 
            !string.IsNullOrEmpty(pfxCertificatePath))
        {
            SignExecutableWindows(outputPath, pfxCertificatePath, pfxPassword);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            SignExecutableMacOS(outputPath);
        }

        return outputPath;
    }

    private static void SignExecutableMacOS(string filePath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "codesign",
                Arguments = $"--force --deep -s - \"{filePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        try
        {
            process.Start();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Failed to ad-hoc sign macOS binary: {error}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"codesign utility failed or was not found: {ex.Message}");
        }
    }

    private static void SignExecutableWindows(string filePath, string certPath, string? password)
    {
        if (!File.Exists(certPath))
            throw new FileNotFoundException($"Signing certificate not found: {certPath}", certPath);

        string arguments = string.IsNullOrEmpty(password)
            ? $"sign /f \"{certPath}\" /tr http://timestamp.digicert.com /td sha256 /fd sha256 \"{filePath}\""
            : $"sign /f \"{certPath}\" /p \"{password}\" /tr http://timestamp.digicert.com /td sha256 /fd sha256 \"{filePath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "signtool.exe",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        try
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Failed to sign the executable. Exit code: {process.ExitCode}. Output: {output} Error: {error}");
            }
        }
        catch (System.ComponentModel.Win32Exception)
        {
            throw new InvalidOperationException("signtool.exe was not found. Ensure Windows SDK is installed and signtool is available in the system PATH.");
        }
    }

    private static string GetTemplateName()
    {
        if (OperatingSystem.IsWindows())
        {
            return "template_launcher.exe";
        }
        else
        {
            return "template_launcher";
        }
    }

    private static int FindLauncherData(byte[] binary)
    {
        if (binary.Length < launcherDataSize)
            throw new InvalidDataException("The template is too small.");

        for (int i = 0; i <= binary.Length - launcherDataSize; i++)
        {
            if (!Matches(binary, i, magic))
                continue;

            uint currentVersion = ReadUInt32(binary, i + 8);
            uint currentSize = ReadUInt32(binary, i + 12);

            if (currentVersion != version || currentSize != launcherDataSize)
                continue;

            int endMagicOffset = i + launcherDataSize - endMagic.Length;

            if (Matches(binary, endMagicOffset, endMagic))
                return i;
        }

        throw new InvalidDataException("A valid LauncherData block was not found in the template.");
    }

    private static void ValidateLauncherData(byte[] binary, int offset)
    {
        if (!Matches(binary, offset, magic))
            throw new InvalidDataException("Invalid launcher magic.");

        uint currentVersion = ReadUInt32(binary, offset + 8);

        if (currentVersion != version)
            throw new InvalidDataException($"Unsupported launcher version: {currentVersion}");

        uint currentSize = ReadUInt32(binary, offset + 12);

        if (currentSize != launcherDataSize)
            throw new InvalidDataException($"Invalid launcher data size: {currentSize}");

        int endMagicOffset = offset + launcherDataSize - endMagic.Length;

        if (!Matches(binary, endMagicOffset, endMagic))
            throw new InvalidDataException("Invalid launcher end magic.");
    }

    private static void WriteFixedBytes(byte[] binary, int offset, int fieldSize, string value)
    {
        byte[] data = Encoding.UTF8.GetBytes(value);

        if (data.Length > fieldSize)
            throw new ArgumentException($"Value exceeds the maximum size of {fieldSize} bytes.");

        Array.Clear(binary, offset, fieldSize);
        Buffer.BlockCopy(data, 0, binary, offset, data.Length);
    }

    private static void ValidateFieldSize(string value, int fieldSize, string fieldName)
    {
        int byteCount = Encoding.UTF8.GetByteCount(value);

        if (byteCount > fieldSize)
            throw new ArgumentException($"{fieldName} exceeds the maximum size of {fieldSize} bytes.");
    }

    private static void ValidateInput(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty.", parameterName);
    }

    private static uint ReadUInt32(byte[] binary, int offset)
    {
        return BitConverter.ToUInt32(binary, offset);
    }

    private static bool Matches(byte[] binary, int offset, byte[] expected)
    {
        if (offset < 0 || offset + expected.Length > binary.Length)
            return false;

        for (int i = 0; i < expected.Length; i++)
        {
            if (binary[offset + i] != expected[i])
                return false;
        }

        return true;
    }
}
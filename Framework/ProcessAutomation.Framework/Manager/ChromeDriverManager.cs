using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace ProcessAutomation.Framework.Manager
{
    public class ChromeDriverManager
    {
        private const string WIN64_PLATFORM = "win64";
        private const string WIN32_PLATFORM = "win32";
        private const string LINUX64_PLATFORM = "linux64";
        private const string LINUXARM64_PLATFORM = "linux-arm64";
        private const string MAC_ARM64_PLATFORM = "mac-arm64";
        private const string MAC_X64_PLATFORM = "mac-x64";
        private const string STABLE = "STABLE";

        private const string ZIP_EXTENSION = ".zip";
        private const string CHROMEDRIVER_NAME = "chromedriver";
        private const string CHROMEDRIVER_GOOD_VERSION_FILE = "GoodVersions.json";
        private const string CHROMEDRIVER_LAST_GOOD_VERSION_FILE = "LastGoodVersions.json";
        private const string CHROMEDRIVER_URL = "https://chromedriver.chromium.org/downloads";
        private const string CHROMEDRIVER_URL_DOWNLOAD_FORMAT = "https://chromedriver.storage.googleapis.com/{0}/chromedriver_win32.zip";
        private const string CHROMEDRIVER_URL_GOOD_VERSIONS = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json";
        private const string CHROMEDRIVER_URL_LAST_GOOD_VERSIONS = "https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json";

        private const string CHROME_VERSION_FROM_ERROR_REGEX = @"\d+\.\d+\.\d+\.\d+";
        private const string DEFAULT_CHROME_DRIVER_ARGUMENTS = "--incognito --disable-infobars --kiosk";
        private const string CHROME_DRIVER_PATH_REXEG = @"(?:[a-zA-Z]:{1}([a-zA-Z0-9\\\s]+)(?<=\\)chrome.exe)";
        private IWebDriver driver = null;
        private string chromeDriverFolder = null;

        public ChromeDriverManager(string chromeDriverFolder)
        {
            if (string.IsNullOrWhiteSpace(chromeDriverFolder))
            {
                throw new Exception("You need chrome driver folder");
            }

            this.chromeDriverFolder = chromeDriverFolder;
        }

        private string GetChromeNameApp()
        {
            string name = null;

            if (OperatingSystem.IsWindows())
            {
                name = string.Format("{0}.exe", CHROMEDRIVER_NAME);
            }
            else if (OperatingSystem.IsLinux())
            {
                name = CHROMEDRIVER_NAME;
            }
            else if (OperatingSystem.IsMacOS())
            {
                name = CHROMEDRIVER_NAME;
            }
            else
            {
                name = CHROMEDRIVER_NAME;
            }

            return name;
        }

        private string GetArchitecture()
        {
            string architecture = null;

            if (OperatingSystem.IsWindows())
            {
                if (RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    architecture = WIN64_PLATFORM;
                }
                else
                {
                    architecture = WIN32_PLATFORM;
                } 
            }
            else if (OperatingSystem.IsLinux())
            {
                if (RuntimeInformation.OSArchitecture == Architecture.X64)
                {
                    architecture = LINUX64_PLATFORM;
                }
                else
                {
                    architecture = LINUXARM64_PLATFORM;
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                if (RuntimeInformation.OSArchitecture == Architecture.Arm64)
                {
                    architecture = MAC_ARM64_PLATFORM;
                }
                else
                {
                    architecture = MAC_X64_PLATFORM;
                }
            }
            else
            {
                architecture = WIN64_PLATFORM;
            }

            return architecture;
        }

        public void InitChromeDriver(string binaryLocation = null, string arguments = DEFAULT_CHROME_DRIVER_ARGUMENTS)
        {
            int attempCount = 0;
            const int attempMax = 5;
            List<ChromeDriverVersionInfo> chromeVersionListAux = null;
            List<ChromeDriverVersionInfo> goodChromeVersionList = null;
            ChromeDriverVersionInfo chromeVersionInfo = null;
            bool readLoadLocalFiles = false;

        OPEN_DRIVER:
            try
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver = null;
                }
                attempCount++;

                ChromeOptions options = new ChromeOptions();
                options.AcceptInsecureCertificates = true;
                options.AddArguments(arguments);
                
                if (!string.IsNullOrWhiteSpace(binaryLocation))
                {
                    options.BinaryLocation = binaryLocation;
                }

                driver = new ChromeDriver(chromeDriverFolder, options);
                driver.Manage().Window.Minimize();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                IntPtr chromeDriverPtr = ApiOS.FindWindow(null, Path.Combine(chromeDriverFolder, GetChromeNameApp()));
                if (chromeDriverPtr != IntPtr.Zero)
                {
                    ApiOS.SendMessage(chromeDriverPtr, ApiOS.WM_CLOSE, 0, 0);
                }
                
                if (attempCount != attempMax)
                {
                    if (goodChromeVersionList == null || goodChromeVersionList.Count == 0 || readLoadLocalFiles)
                    {   
                        string versionMatching = null;
                        MatchCollection matchingCollection = Regex.Matches(errorMessage, CHROME_VERSION_FROM_ERROR_REGEX);

                        if (matchingCollection.Count >= 2)
                        {
                            versionMatching = matchingCollection[1].Value;
                        }

                        if (attempCount == 1)
                        {
                            goodChromeVersionList = GetLocalFileGoodChromeVersionList(Path.Combine(chromeDriverFolder, CHROMEDRIVER_LAST_GOOD_VERSION_FILE)
                                , Path.Combine(chromeDriverFolder, CHROMEDRIVER_GOOD_VERSION_FILE));
                        }

                        if (goodChromeVersionList?.Count == 0 || attempCount > 1)
                        {
                            readLoadLocalFiles = false;
                            goodChromeVersionList = GetGoodChromeVersionList();
                        }
                        else
                        {
                            readLoadLocalFiles = true;
                        }

                        if (goodChromeVersionList != null && goodChromeVersionList.Any())
                        {
                            if (versionMatching != null)
                            {
                                chromeVersionListAux = goodChromeVersionList.Where(v => Version.Parse(v.Version) <= Version.Parse(versionMatching))
                                    .OrderByDescending(v => Version.Parse(v.Version))
                                    .Take(1)
                                    .ToList();
                            }

                            if (chromeVersionListAux == null || chromeVersionListAux.Count == 0)
                            {
                                goodChromeVersionList = goodChromeVersionList.Where(f => f.Channel != null && f.Channel.ToLower() == STABLE.ToLower()).ToList();
                            }
                            else
                            {
                                goodChromeVersionList = chromeVersionListAux;
                            }
                        }
                    }

                    if (goodChromeVersionList?.Count > 0)
                    {
                        chromeVersionInfo = goodChromeVersionList.Last();
                        DownloadChromeDriver(chromeVersionInfo.Version, chromeVersionInfo.DownloadList
                            .FirstOrDefault(f => f.Platform.ToLower() == GetArchitecture().ToLower()).Url);
                        goodChromeVersionList.Remove(chromeVersionInfo);

                        goto OPEN_DRIVER;
                    }
                    else if (readLoadLocalFiles)
                    {
                        goto OPEN_DRIVER;
                    }
                }

                if (driver != null)
                {
                    driver.Quit();
                }

                driver = null;
                throw;
            }
        }

        public List<string> GetChromeVersionList()
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument doc = htmlWeb.Load(CHROMEDRIVER_URL);
            HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes("//a[@href]");

            return nodeList.Where(f => f.Attributes["href"].Value.Contains("chromedriver.storage.googleapis.com/index"))
                                            .Select(s => HttpUtility.ParseQueryString(new Uri(s.Attributes["href"].Value).Query).Get("path").Replace("/", string.Empty))
                                            .Distinct().ToList();
        }

        public void AddChromeVersionList(List<ChromeDriverVersionInfo> chromeDriverList, dynamic dynamicVersion, string channel = null)
        {
            if (dynamicVersion != null)
            {
                string version = dynamicVersion.version.ToString();
                long revision = long.Parse(dynamicVersion.revision.ToString());
                List<ChromeDriverDownloadVersionInfo> downloadVersionList = new List<ChromeDriverDownloadVersionInfo>();

                if (dynamicVersion.downloads?.chromedriver != null)
                {
                    foreach (dynamic downloadVersion in dynamicVersion.downloads.chromedriver)
                    {
                        downloadVersionList.Add(new ChromeDriverDownloadVersionInfo
                        {
                            Platform = downloadVersion.platform.ToString(),
                            Url = downloadVersion.url.ToString(),
                        });
                    }

                    if (downloadVersionList?.Count > 0)
                    {
                        chromeDriverList.Add(new ChromeDriverVersionInfo
                        {
                            Channel = channel,
                            Version = version,
                            Revision = revision,
                            DownloadList = downloadVersionList
                        });
                    }
                }
            }
        }

        public List<ChromeDriverVersionInfo> GetLocalFileGoodChromeVersionList(string lastGoodVersionsFile, string goodVersionsFile)
        {
            List<ChromeDriverVersionInfo> chromeDriverList = new List<ChromeDriverVersionInfo>();

            {
                if (File.Exists(lastGoodVersionsFile))
                {
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(lastGoodVersionsFile));
                    if (jsonObject != null)
                    {
                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Stable, "Stable");
                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Beta, "Beta");
                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Dev, "Dev");
                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Canary, "Canary");
                    }
                }
            }

            {
                if (File.Exists(goodVersionsFile))
                {
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(goodVersionsFile));
                    if (jsonObject != null)
                    {
                        foreach (dynamic dynamicVersion in jsonObject.versions)
                        {
                            AddChromeVersionList(chromeDriverList, dynamicVersion, null);
                        }
                    }
                }

                chromeDriverList = chromeDriverList.OrderByDescending(s => s.Version).ToList();
            }

            return chromeDriverList;
        }

        public List<ChromeDriverVersionInfo> GetGoodChromeVersionList()
        {
            List<ChromeDriverVersionInfo> chromeDriverList = new List<ChromeDriverVersionInfo>();

            {
                HttpClient httpClient =  new HttpClient();
                HttpResponseMessage response = httpClient.GetAsync(CHROMEDRIVER_URL_LAST_GOOD_VERSIONS).Result;
                if (response.EnsureSuccessStatusCode().StatusCode == HttpStatusCode.OK)
                {
                    string fileResult = response.Content.ReadAsStringAsync().Result;
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(fileResult);
                    if (jsonObject != null)
                    {
                        if (File.Exists(Path.Combine(chromeDriverFolder, CHROMEDRIVER_LAST_GOOD_VERSION_FILE)))
                        {
                            File.Delete(Path.Combine(chromeDriverFolder, CHROMEDRIVER_LAST_GOOD_VERSION_FILE));
                        }
                        File.WriteAllText(Path.Combine(chromeDriverFolder, CHROMEDRIVER_LAST_GOOD_VERSION_FILE), fileResult);

                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Stable, "Stable");
                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Beta, "Beta");
                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Dev, "Dev");
                        AddChromeVersionList(chromeDriverList, jsonObject.channels.Canary, "Canary");
                    }
                }
            }

            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = httpClient.GetAsync(CHROMEDRIVER_URL_GOOD_VERSIONS).Result;
                if (response.EnsureSuccessStatusCode().StatusCode == HttpStatusCode.OK)
                {
                    string fileResult = response.Content.ReadAsStringAsync().Result;
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(fileResult);
                    if (jsonObject != null)
                    {
                        if (File.Exists(Path.Combine(chromeDriverFolder, CHROMEDRIVER_GOOD_VERSION_FILE)))
                        {
                            File.Delete(Path.Combine(chromeDriverFolder, CHROMEDRIVER_GOOD_VERSION_FILE));
                        }
                        File.WriteAllText(Path.Combine(chromeDriverFolder, CHROMEDRIVER_GOOD_VERSION_FILE), fileResult);

                        foreach (dynamic dynamicVersion in jsonObject.versions)
                        {
                            AddChromeVersionList(chromeDriverList, dynamicVersion, null);
                        }
                    }
                }

                chromeDriverList = chromeDriverList.OrderByDescending(s => s.Version).ToList();
            }

            return chromeDriverList;
        }

        public string CurrentVersionChromeDriver()
        {
            string chromeDriverPath = Path.Combine(chromeDriverFolder, GetChromeNameApp());

            if (File.Exists(chromeDriverPath))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = chromeDriverPath;
                process.StartInfo.Arguments = "-v";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string[] outputSplit = process.StandardOutput.ReadToEnd().Split(' ');
                return outputSplit[1];
            }
            else
            {
                return null;
            }
        }

        public void DownloadChromeDriver(string versionDownload, string downloadUrl)
        {
            string chromeDriverPath = Path.Combine(Path.Combine(chromeDriverFolder, GetChromeNameApp()));
            string currentVersion = CurrentVersionChromeDriver();

            if (currentVersion != versionDownload)
            {
                try
                {
                    //Utilities.WriteLog(CommonConstants.PATH_LOG_FILE, string.Format("Descargando versión: {0}", versionDownload), true);                    
                    //string fileDownload = string.Format(CHROMEDRIVER_URL_DOWNLOAD_FORMAT, versionDownload);
                    string fileDownload = downloadUrl;

                    if (File.Exists(chromeDriverPath))
                    {
                        string fileOldVersion = string.Concat(chromeDriverPath, ".oldversion");
                        if (File.Exists(fileOldVersion))
                        {
                            File.Delete(fileOldVersion);
                        }

                        File.Move(chromeDriverPath, fileOldVersion);
                    }

                    // CLEAN ALL CHROMEDRIVER FILES 
                    List<string> chromeDriverFileList = Directory.GetFiles(string.Concat(Path.GetDirectoryName(chromeDriverPath)), "*chromedriver*.*").ToList();
                    if (chromeDriverFileList?.Count > 0)
                    {
                        foreach (string chromeDriverFile in chromeDriverFileList)
                        {
                            if (!chromeDriverFile.Contains("oldversion"))
                            {
                                File.Delete(chromeDriverFile);
                            }
                        }
                    }

                    string zipFile = string.Concat(chromeDriverPath, ZIP_EXTENSION);
                    if (File.Exists(zipFile))
                    {
                        File.Delete(zipFile);
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = client.GetAsync(fileDownload).Result)
                        {
                            response.EnsureSuccessStatusCode();

                            byte[] data = response.Content.ReadAsByteArrayAsync().Result;
                            File.WriteAllBytes(zipFile, data);
                        }
                    }

                    if (File.Exists(zipFile))
                    {
                        if (Path.GetExtension(zipFile).ToLower() == ZIP_EXTENSION)
                        {
                            using (ZipArchive archive = ZipFile.Open(zipFile, ZipArchiveMode.Read))
                            {
                                foreach (ZipArchiveEntry archiveEntry in archive.Entries)
                                {
                                    if (!File.Exists(Path.Combine(chromeDriverFolder, archiveEntry.Name)))
                                    {
                                        archiveEntry.ExtractToFile(Path.Combine(chromeDriverFolder, archiveEntry.Name));
                                    }
                                }
                            }

                            File.Delete(zipFile);
                        }
                    }
                    else
                    {
                        throw new Exception("Could not download file chromdriver");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public IWebDriver GetChromeDriver()
        {
            return driver;
        }

        //private void WaitForPageLoad()
        //{
        //    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        //    wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        //}

        //private bool LoadPage(string pageUrl, int delay = 1, bool maximizeWindow = true)
        //{
        //    bool result = false;

        //    if (driver != null)
        //    {
        //        if (this.pageUrl == null || this.pageUrl != pageUrl)
        //        {
        //            driver.Navigate().GoToUrl(pageUrl);
        //            WaitForPageLoad();
        //            System.Threading.Thread.Sleep(delay * 1000);
        //            this.pageUrl = pageUrl;

        //            if (action == null)
        //            {
        //                action = new Actions(driver);
        //            }

        //            action.SendKeys(OpenQA.Selenium.Keys.Escape);
        //            result = true;
        //        }

        //        if (this.pageUrl != null)
        //        {
        //            result = true;
        //        }

        //        try
        //        {
        //            if (maximizeWindow)
        //            {
        //                driver.Manage().Window.Maximize();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }

        //    return result;
        //}
    }
}

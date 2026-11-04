using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.IO;

namespace ProcessAutomation.Component
{
    public class StopComponent : TaskComponentBaseBC, ITaskComponent
    {   
        //private const uint MB_TOPMOST = 0x00040000;
        //private const uint MB_OK = 0x00000000;
        //private const uint MB_OKCANCEL = 0x00000001;
        //private const uint MB_ABORTRETRYIGNORE = 0x00000002;
        //private const uint MB_YESNOCANCEL = 0x00000003;
        //private const uint MB_YESNO = 0x00000004;
        //private const uint MB_RETRYCANCEL = 0x00000005;
        private IWebDriver driver = null;
        private Actions action = null;
        private string pageUrl = null;
        private ChromeDriverManager driverManager = null;

        private const string CHROMEDRIVER_EXE = "chromedriver.exe";
        private const string CHROMEDRIVER_URL = "https://chromedriver.chromium.org/downloads";
        private const string CHROMEDRIVER_URL_DOWNLOAD_FORMAT = "https://chromedriver.storage.googleapis.com/{0}/chromedriver_win32.zip";
        private const string CHROME_DRIVER_PATH_REXEG = @"(?:[a-zA-Z]:{1}([a-zA-Z0-9\\\s]+)(?<=\\)chrome.exe)";
        private const string CHROME_VERSION_FROM_ERROR_REGEX = @"((?:(?:[0-9]+\.[0-9\.]+))|(?:[0-9]+))";

        public StopComponent() : base()
        {
        }

        public bool InitComponent()
        {
            bool success = false;
            try
            {
                driverManager = new ChromeDriverManager(Path.Combine(processBC.GetWorkspacePath(), processBC.GetProcessFolder(), CommonConstants.EXTERNAL_FOLDER));
                driverManager.InitChromeDriver();
                driver = driverManager.GetChromeDriver();
                success = true;
            }
            catch (Exception ex)
            {
                WriteMessageLog(ex.Message);
                WriteMessageLog("You need to update google chrome");
                success = false;
            }

            return success;
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

        private void WaitForPageLoad()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        private bool LoadPage(string pageUrl, int delay)
        {
            bool result = false;

            if (driver != null)
            {
                if (this.pageUrl == null || this.pageUrl != pageUrl)
                {   
                    driver.Navigate().GoToUrl(pageUrl);
                    WaitForPageLoad();
                    System.Threading.Thread.Sleep(delay * 1000);
                    this.pageUrl = pageUrl;

                    if (action == null)
                    {
                        action = new Actions(driver);
                    }

                    action.SendKeys(OpenQA.Selenium.Keys.Escape);
                    result = true;
                }

                if (this.pageUrl != null)
                {
                    result = true;
                }

                try
                {

                    driver.Manage().Window.Maximize();
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            const string message = "Closing...";
            string htmlPath = string.Format("file://{0}", Path.Combine(processBC.GetWorkspacePath(), processBC.GetProcessFolder(), "Resources/Pages/StopPageComponent.html"));
            LoadPage(htmlPath, 3);

            try
            {
                int delay = processBC.GetProcessedInputValue<int>();

                driver.Manage().Window.Maximize();
                IWebElement btnClearMessage = driver.FindElement(By.Id("btnClearMessage"));
                btnClearMessage.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(delay));
                IWebElement element = wait.Until(d =>
                {
                    var el = d.FindElement(By.Id("lblMessage"));
                    return el.Displayed && el.Enabled ? el : null;
                });

                wait.Until(d => element.Text.Contains(message));

                btnClearMessage.Click();
                driver.Manage().Window.Minimize();

                StepSuccess();
            }
            catch (Exception ex)
            {
                StepFail(ex.ToString());
            }

            return null;
        }

        public void EndComponent()
        {
            try
            {
                pageUrl = null;
                driver.Dispose();
            }
            catch
            { }
        }
    }
}

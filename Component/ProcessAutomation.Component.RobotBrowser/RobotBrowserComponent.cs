using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static IronPython.Modules._ast;

namespace ProcessAutomation.Component
{
    public class RobotBrowserComponent : TaskComponentBaseBC, ITaskComponent
    {
        private const string DRIVER_KEY = "Driver";
        private const string PAGE_URL_KEY = "PageUrl";
        private const string DEFAULT_CHROME_DRIVER_ARGUMENTS = "--incognito --disable-infobars --kiosk";
        private Dictionary<int, Dictionary<string, object>> componentDictionary = null;

        public RobotBrowserComponent() : base()
        {
            componentDictionary = new Dictionary<int, Dictionary<string, object>>();
        }

        private IWebDriver GetDriver(int uniqueId)
        {
            IWebDriver driver = null;
            if (componentDictionary[uniqueId].ContainsKey(DRIVER_KEY))
            {
                driver = (IWebDriver)componentDictionary[uniqueId]?[DRIVER_KEY];
            }
            return driver;
        }

        private void SetDriver(int uniqueId, IWebDriver driver)
        {
            if (!componentDictionary[uniqueId].ContainsKey(DRIVER_KEY))
            {
                componentDictionary[uniqueId].Add(DRIVER_KEY, driver);
            }
            else
            {
                componentDictionary[uniqueId][DRIVER_KEY] = driver;
            }
        }

        private string GetPageUrl(int uniqueId)
        {
            string pageUrl = null;
            if (componentDictionary[uniqueId].ContainsKey(PAGE_URL_KEY))
            {
                pageUrl = componentDictionary[uniqueId]?[PAGE_URL_KEY]?.ToString();
            }
            return pageUrl;
        }

        private void SetPageUrl(int uniqueId, string pageUrl)
        {
            if (!componentDictionary[uniqueId].ContainsKey(PAGE_URL_KEY))
            {
                componentDictionary[uniqueId].Add(PAGE_URL_KEY, pageUrl);
            }
            else
            {
                componentDictionary[uniqueId][PAGE_URL_KEY] = pageUrl;
            }
        }

        private string GetBravePath()
        {
            string foundPaths = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                List<string> possiblePaths = new List<string>
                {
                    @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
                    @"C:\Program Files (x86)\BraveSoftware\Brave-Browser\Application\brave.exe"
                };

                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (!string.IsNullOrEmpty(localAppData))
                {
                    possiblePaths.Add(Path.Combine(localAppData, @"BraveSoftware\Brave-Browser\Application\brave.exe"));
                }

                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPaths = path;
                        break;
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string[] paths =
                {
                    "/Applications/Brave Browser.app/Contents/MacOS/Brave Browser",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                                "Applications/Brave Browser.app/Contents/MacOS/Brave Browser")
                };

                foreach (var path in paths)
                {
                    if (File.Exists(path))
                    {
                        foundPaths = path;
                        break;
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string[] paths =
                {
                    "/usr/bin/brave-browser",
                    "/usr/bin/brave",
                    "/opt/brave.com/brave/brave-browser",
                    "/snap/bin/brave",
                    "/var/lib/flatpak/app/com.brave.Browser/current/active/export/bin/com.brave.Browser"
                };

                foreach (var path in paths)
                {
                    if (File.Exists(path))
                    {
                        foundPaths = path;
                        break;
                    }
                }
            }

            return foundPaths;
        }

        private IWebElement GetWebElement(int uniqueId, string objectSearched, string by)
        {
            IWebElement element = null;

            try
            {
                //ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.Id(""));
                switch (by)
                {
                    case CommonConstants.ID:
                        element = GetDriver(uniqueId).FindElement(By.Id(objectSearched));
                        break;
                    case CommonConstants.NAME:
                        element = GetDriver(uniqueId).FindElement(By.Name(objectSearched));
                        break;
                    case CommonConstants.TAG_NAME:
                        element = GetDriver(uniqueId).FindElement(By.TagName(objectSearched));
                        break;
                    case CommonConstants.TEXT:
                        element = GetDriver(uniqueId).FindElement(By.XPath(string.Format(CommonConstants.FIND_BY_TEXT_FORMAT, objectSearched)));
                        break;
                    case CommonConstants.CONTAINS:
                        element = GetDriver(uniqueId).FindElement(By.XPath(string.Format(CommonConstants.FIND_BY_CONTAINS_FORMAT, objectSearched)));
                        break;
                    case CommonConstants.CLASS_NAME:
                        element = GetDriver(uniqueId).FindElement(By.ClassName(objectSearched));
                        break;
                    case CommonConstants.CSS_SELECTOR:
                        element = GetDriver(uniqueId).FindElement(By.CssSelector(objectSearched));
                        break;
                    case CommonConstants.XPATH:
                        element = GetDriver(uniqueId).FindElement(By.XPath(objectSearched));
                        break;
                    default:
                        element = GetDriver(uniqueId).FindElement(By.Id(objectSearched));
                        break;
                }
            }
            catch (Exception)
            {

            }

            return element;
        }

        private ReadOnlyCollection<IWebElement> GetWebElementCollection(int uniqueId, string objectSearched, string by)
        {
            ReadOnlyCollection<IWebElement> elementCollection = null;

            try
            {
                switch (by)
                {
                    case CommonConstants.ID:
                        elementCollection = GetDriver(uniqueId).FindElements(By.Id(objectSearched));
                        break;
                    case CommonConstants.NAME:
                        elementCollection = GetDriver(uniqueId).FindElements(By.Name(objectSearched));
                        break;
                    case CommonConstants.TAG_NAME:
                        elementCollection = GetDriver(uniqueId).FindElements(By.TagName(objectSearched));
                        break;
                    case CommonConstants.TEXT:
                        elementCollection = GetDriver(uniqueId).FindElements(By.XPath(string.Format(CommonConstants.FIND_BY_TEXT_FORMAT, objectSearched)));
                        break;
                    case CommonConstants.CONTAINS:
                        elementCollection = GetDriver(uniqueId).FindElements(By.XPath(string.Format(CommonConstants.FIND_BY_CONTAINS_FORMAT, objectSearched)));
                        break;
                    case CommonConstants.CLASS_NAME:
                        elementCollection = GetDriver(uniqueId).FindElements(By.ClassName(objectSearched));
                        break;
                    case CommonConstants.CSS_SELECTOR:
                        elementCollection = GetDriver(uniqueId).FindElements(By.CssSelector(objectSearched));
                        break;
                    case CommonConstants.XPATH:
                        elementCollection = GetDriver(uniqueId).FindElements(By.XPath(objectSearched));
                        break;
                    default:
                        elementCollection = GetDriver(uniqueId).FindElements(By.Id(objectSearched));
                        break;
                }                
            }
            catch (Exception)
            {

            }

            return elementCollection;
        }

        private void WaitForPageLoad(int uniqueId)
        {
            WebDriverWait wait = new WebDriverWait(GetDriver(uniqueId), TimeSpan.FromSeconds(20));
            wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        private bool LoadPage(int uniqueId, string pageUrl, int delay)
        {
            bool result = false;

            if (GetDriver(uniqueId) != null)
            {
                if (GetPageUrl(uniqueId) == null || GetPageUrl(uniqueId) != pageUrl)
                {
                    GetDriver(uniqueId).Navigate().GoToUrl(pageUrl);
                    WaitForPageLoad(uniqueId);
                    System.Threading.Thread.Sleep(delay * 1000);
                    SetPageUrl(uniqueId, pageUrl);

                    Actions action = new Actions(GetDriver(uniqueId));
                    action.SendKeys(OpenQA.Selenium.Keys.Escape);
                    result = true;
                }

                if (GetPageUrl(uniqueId) != null)
                {
                    result = true;
                }

                try
                {

                    GetDriver(uniqueId).Manage().Window.Maximize();
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            bool success = false;
            try
            {
                CfgRobotBrowserDTO config = processBC.GetProcessedComponentValue<CfgRobotBrowserDTO>();
                if (config == null || string.IsNullOrWhiteSpace(config.Browser))
                {
                    throw new InvalidOperationException("You need to configure the component");
                }

                string binaryLocation = null;
                if (config.Browser == "OTHER")
                {
                    binaryLocation = config.BinaryLocation;
                    if (!File.Exists(binaryLocation))
                        throw new InvalidOperationException("BinaryLocation not found");
                }
                else if (config.Browser == "BRAVE")
                {
                    binaryLocation = GetBravePath();
                    if (!File.Exists(binaryLocation))
                        throw new InvalidOperationException("BRAVE not found, In the browser, select \"OTHER\" and add the direct path to the executable file.");
                }

                string options = !string.IsNullOrWhiteSpace(config.Options) ? config.Options.Trim() : string.Empty;
                if (config.Incognito)
                {
                    options = string.Concat(options, " ", DEFAULT_CHROME_DRIVER_ARGUMENTS).Trim();
                }

                if (componentDictionary == null)
                { 
                    componentDictionary = new Dictionary<int, Dictionary<string, object>>();
                }

                if (!componentDictionary.ContainsKey(componentInfo.Id))
                {
                    componentDictionary.Add(componentInfo.Id, new Dictionary<string, object>());
                }

                IWebDriver driver = GetDriver(componentInfo.Id);
                string pageUrl = GetPageUrl(componentInfo.Id);

                if (driver == null)
                {
                    ChromeDriverManager driverManager = driverManager = new ChromeDriverManager(Path.Combine(processBC.GetWorkspacePath(), processBC.GetProcessFolder(), CommonConstants.EXTERNAL_FOLDER));
                    
                    driverManager.InitChromeDriver(binaryLocation);
                    driver = driverManager.GetChromeDriver();
                    SetDriver(componentInfo.Id, driver);
                }

                if (driver != null)
                {
                    success = true;
                }
            }
            catch (InvalidOperationException ex)
            {
                success = false;
                WriteMessageLog(ex.Message);
            }
            catch (Exception ex)
            {
                success = false;
                WriteMessageLog(ex.Message);
                WriteMessageLog("You need to update google chrome or ChromeDriver update fails");
                WriteMessageLog("Can you download here https://googlechromelabs.github.io/chrome-for-testing/ or https://chromedriver.chromium.org/downloads");
                WriteMessageLog("and replace chromedriver.exe with the new one");
            }
            return success;
        }

        private void AddAttributeToList(ref List<Dictionary<string, object>> resultList, IWebElement element, List<string> attributeList, bool onlyText)
        {
            if (element == null)
                return;

            Dictionary<string, object> values = new Dictionary<string, object>();
            if (onlyText)
            {
                if (element.TagName.ToUpper() == "INPUT")
                {
                    values.Add("value", element.GetAttribute("value"));
                }
                else
                {
                    values.Add("value", element.Text?.Trim());
                }
            }
            else
            {
                foreach (string attribute in attributeList)
                {
                    if (element.TagName.ToUpper() == "INPUT")
                    {
                        if (string.IsNullOrWhiteSpace(attribute))
                        {
                            values.Add("innerHTML", element.GetAttribute("innerHTML"));
                        }
                        else
                        {
                            values.Add(attribute, element.GetAttribute(attribute));
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(attribute))
                        {
                            values.Add("innerHTML", element.GetAttribute("innerHTML"));
                        }
                        else
                        {
                            values.Add(attribute, element.GetAttribute(attribute));
                        }
                    }
                }
            }

            resultList.Add(values);
        }

        private enum ConcatMode
        {
            ValuesOnly,
            KeyValue
        }

        public static List<string> PrepareDataBatches<T>(IEnumerable<T> source, int chunkSize, string separator, Func<T, string> formatter)
        {
            if (chunkSize <= 0)
            {
                return new()
                {
                    string.Join(separator, source.Select(formatter))
                };
            }

            return source
                .Chunk(chunkSize)
                .Select(chunk =>
                    string.Join(separator, chunk.Select(formatter)))
                .ToList();
        }

        protected IObjectType ProcessGet()
        {
            IObjectType resultValue = null;            
            if (GetParameter("ELEMENT_PARAM") != null)
            {
                bool multiple = GetParameter<bool>("MULTIPLE_PARAM");
                bool onlyText = GetParameter<bool>("ONLY_TEXT_PARAM");
                bool concatUnion = GetParameter<bool>("CONCAT_UNION_PARAM");
                int quantityUnion = GetParameter<int>("QUANTITY_UNION_PARAM");
                string concatSeparator = GetParameter<string>("CONCAT_SEPARATOR_PARAM");
                string attribute = GetParameter<string>("ATTRIBUTE_PARAM");
                ReadOnlyCollection<IWebElement> elementCollection = (ReadOnlyCollection<IWebElement>)GetParameter("ELEMENT_PARAM");
                List<string> attributeList = !string.IsNullOrEmpty(attribute) && !onlyText ? attribute.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() : null;
                List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

                if (concatUnion && attributeList?.Count > 0)
                {
                    if (quantityUnion > 0)
                    {
                        attributeList = attributeList.GetRange(0, quantityUnion);
                    }
                    else
                    {
                        attributeList = attributeList.GetRange(0, 1);
                    }
                }

                if (concatUnion && quantityUnion > 0)
                {
                    int countAttribute = 0;
                    foreach (IWebElement element in elementCollection)
                    {
                        if (attributeList?.Count > 0)
                        {
                            if (concatUnion && quantityUnion > 0)
                            {
                                attribute = attributeList[countAttribute];
                                countAttribute++;

                                if (countAttribute >= quantityUnion)
                                {
                                    countAttribute = 0;
                                }
                            }
                            else
                            {
                                attribute = attributeList[0];
                            }
                        }

                        AddAttributeToList(ref resultList, element, attributeList, onlyText);

                        if (GetParameter("EVENT_PARAM") != null)
                        {
                            DoEvent(element, GetParameter("EVENT_PARAM").ToString());
                        }
                    }
                }
                else
                {
                    foreach (IWebElement element in elementCollection)
                    {
                        AddAttributeToList(ref resultList, element, attributeList, onlyText);

                        if (GetParameter("EVENT_PARAM") != null)
                        {
                            DoEvent(element, GetParameter("EVENT_PARAM").ToString());
                        }
                    }
                }

                if (resultList.Count > 0)
                {
                    // ConcatMode mode = ConcatMode.ValuesOnly;
                    // Func<Dictionary<string, object>, string> formatter;

                    // if (mode == ConcatMode.ValuesOnly)
                    // {
                    //     formatter = dict => string.Join("-", dict.Values);
                    // }
                    // else if (mode == ConcatMode.KeyValue)
                    // {
                    //     formatter = dict => string.Join("-", dict.Select(kv => $"{kv.Key}:{kv.Value}"));
                    // }
                    // else
                    // {
                    //     formatter = dict => ""; // Tu caso por defecto
                    // }
                    
                    // List<string> resultValueList = PrepareDataBatches(resultList, quantityUnion, concatSeparator, formatter);

                    if (multiple)
                    {
                        if (onlyText)
                        {
                            resultValue = new ListObjectType(ConvertOnlyTextValues(resultList));
                        }
                        else
                        {
                            resultValue = new ListObjectType(resultList);
                        }
                    }
                    else
                    {
                        resultValue = new ObjectType(resultList[0]);
                    }
                }

                StepSuccess();
            }

            return resultValue;
        }

        private List<string> ConvertOnlyTextValues(List<Dictionary<string, object>> resultList)
        {
            List<string> textResultList = new List<string>();
            if (resultList.Count > 0)
            {
                foreach (var item in resultList)
                {
                    if (item.ContainsKey("value"))
                    {
                        textResultList.Add(item["value"]?.ToString());
                    }
                }
            }

            return textResultList;
        }

        protected IObjectType ProcessSet()
        {
            TextType resultValue = null;
            if (GetParameter("ELEMENT_PARAM") != null)
            {
                RobotBrowserDTO robotDTO = processBC.GetProcessedInputValue<RobotBrowserDTO>();
                ReadOnlyCollection<IWebElement> elementCollection = (ReadOnlyCollection<IWebElement>)GetParameter("ELEMENT_PARAM");

                foreach (IWebElement element in elementCollection)
                {
                    resultValue = robotDTO.Value;
                    element.Clear();
                    element.SendKeys(resultValue);

                    if (GetParameter("EVENT_PARAM") != null)
                    {
                        DoEvent(element, GetParameter("EVENT_PARAM").ToString());
                    }
                }

                StepSuccess();
            }
            return resultValue;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            int componentId = runProcessInfo.ComponentInfo.Id;
            IObjectType result = null;
            RobotBrowserDTO robotDTO = processBC.GetProcessedInputValue<RobotBrowserDTO>();

            if (robotDTO != null && (robotDTO.CurrentPage || LoadPage(componentId, robotDTO.Page, robotDTO.Delay)))
            {
                string objectId = processBC.GetProcessedValueFromString<string>(robotDTO.Object);
                string searchType = processBC.GetProcessedValueFromString<string>(robotDTO.SearchType);

                ReadOnlyCollection<IWebElement> elementCollection = null;

                if (robotDTO.Multiple)
                {
                    elementCollection = GetWebElementCollection(componentId, objectId, searchType);                        
                }
                else
                {
                    IWebElement element = GetWebElement(componentId, objectId, searchType);
                    if (element != null)
                    {
                        elementCollection = new ReadOnlyCollection<IWebElement>(new List<IWebElement>() { element });
                    }
                }

                if (elementCollection != null && elementCollection.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(robotDTO.Event))
                    {
                        AddParameter("EVENT_PARAM", robotDTO.Event);
                    }

                    AddParameter("ELEMENT_PARAM", elementCollection);
                    AddParameter("MULTIPLE_PARAM", robotDTO.Multiple);
                    AddParameter("ONLY_TEXT_PARAM", robotDTO.OnlyText);
                    AddParameter("CONCAT_UNION_PARAM", robotDTO.ConcatUnion);
                    AddParameter("QUANTITY_UNION_PARAM", robotDTO.QuantityUnion);
                    AddParameter("CONCAT_SEPARATOR_PARAM", robotDTO.ConcatSeparator);
                    AddParameter("ATTRIBUTE_PARAM", robotDTO.Attribute);

                    try
                    {
                        if (robotDTO.Action == "SET")
                        {
                            result = ProcessSet();
                        }
                        else
                        {
                            result = ProcessGet();
                        }
                    }
                    catch (Exception ex)
                    {
                        StepFail(ex.Message);
                    }
                }
            }

            return result;
        }

        public void EndComponent()
        {
            if (componentDictionary != null)
            {
                foreach(KeyValuePair<int, Dictionary<string, object>> component in componentDictionary)
                {
                    try
                    {
                        ((IWebDriver)component.Value[DRIVER_KEY]).Dispose();
                    }
                    catch
                    {
                    }
                }

                componentDictionary.Clear();
                componentDictionary = null;
            }            
        }

        private void DoEvent(dynamic element, string eventName)
        {
            try
            {
                switch (eventName)
                {
                    case CommonConstants.CLICK:
                        element.Click();
                        break;
                    case CommonConstants.ENTER:
                        element.SendKeys(OpenQA.Selenium.Keys.Enter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

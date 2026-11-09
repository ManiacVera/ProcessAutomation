using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Component.Manager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ProcessAutomation.Common.DTO.Enum;

namespace ProcessAutomation.Component
{
    public class RestApiComponent : TaskComponentBaseBC, ITaskComponent
    {
        private string baseUrl = null;

        public RestApiComponent() : base()
        { 
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            CfgRestApiDTO configDTO = processBC.GetProcessedComponentValue<CfgRestApiDTO>();
            if (configDTO != null && configDTO.BaseUrl != null)
            {
                baseUrl = configDTO.BaseUrl;
            }

            return true;
        }

        public void EndComponent()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        protected IObjectType ProcessGet(RunProcessInfo runProcessInfo)
        {
            TextType result = string.Empty;

            string inputValue = processBC.GetProcessedInputValue<string>();

            if (!string.IsNullOrWhiteSpace(inputValue))
            {
                string methodResult = WebApiService.ProcessApi(WebApiService.ApiMethodEnum.GET, null, inputValue, null);

                if (methodResult != null)
                {
                    dynamic dynamicResult = (dynamic)JsonConvert.DeserializeObject(methodResult);

                    try
                    {
                        if (dynamicResult != null)
                        {
                            string jsonType = dynamicResult.GetType()?.Name?.ToUpper();
                            if (jsonType == "JARRAY" || dynamicResult["ErrorCode"] == null)
                            {
                                //if (jsonType == "JARRAY" || jsonType == "JOBJECT")
                                //{
                                result = dynamicResult.ToString();
                                StepSuccess();
                                //}
                                //else if (dynamicResult["ErrorCode"] != null)
                                //{
                                //    StepFail($"StatusCode: {dynamicResult.ErrorCode}, Message: {dynamicResult.Result}");
                                //}
                            }
                            else
                            {
                                StepFail($"StatusCode: {dynamicResult.ErrorCode}, Message: {dynamicResult.Result}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StepFail(ex.Message);
                    }
                }
                else
                {
                    StepFail("No responde el api");
                }
            }
            else
            {
                StepFail("Falta capturar URL");
            }

            return result;
        }

        protected IObjectType ProcessSet(RunProcessInfo runProcessInfo)
        {
            TextType result = string.Empty;
            RestApiDTO restApiDTO = processBC.GetProcessedInputValue<RestApiDTO>();

            if (restApiDTO != null && !string.IsNullOrWhiteSpace(restApiDTO.Method))
            {
                string url = null;
                if (restApiDTO.Url != null)
                {
                    url = restApiDTO.Url.Trim();
                }              

                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (!string.IsNullOrWhiteSpace(baseUrl))
                    {
                        Uri baseUri = new Uri(baseUrl);
                        url = new Uri(baseUri, url).AbsoluteUri;
                    }

                    Dictionary<string, string> header = null;
                    if (restApiDTO.Header != null &&
                        !string.IsNullOrWhiteSpace(restApiDTO.Header))
                    {
                        header = JsonConvert.DeserializeObject<Dictionary<string, string>>(restApiDTO.Header);
                    }

                    string body = null;
                    if (restApiDTO.Body != null && !string.IsNullOrWhiteSpace(restApiDTO.Body))
                    {
                        body = restApiDTO.Body.Trim();
                    }

                    WebApiService.ApiMethodEnum apiMethodEnum = (WebApiService.ApiMethodEnum)Enum.Parse(typeof(WebApiService.ApiMethodEnum), restApiDTO.Method.Trim());
                    string methodResult = WebApiService.ProcessApi(apiMethodEnum, header, url, body);

                    if (methodResult != null)
                    {
                        dynamic dynamicResult = (dynamic)JsonConvert.DeserializeObject(methodResult);
                        string jsonType = dynamicResult.GetType()?.Name?.ToUpper();

                        try
                        {
                            if (jsonType == "JARRAY" || dynamicResult["ErrorCode"] == null)
                            {
                                //if (dynamicResult["ErrorCode"] == null)
                                //{
                                result = dynamicResult.ToString();
                                StepSuccess();
                                //}
                            }
                            else
                            {
                                StepFail($"StatusCode: {dynamicResult.ErrorCode}, Message: {dynamicResult.Result}");
                            }
                        }
                        catch (Exception ex)
                        {
                            StepFail(ex.Message);
                        }
                    }
                    else
                    {
                        StepFail("No responde el api");
                    }
                }
                else
                {
                    StepFail("Falta Url Api");
                }
            }
            else
            {
                StepFail("Falta capturar URL");
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            IObjectType result = null;

            if (runProcessInfo.InputValueType == ValueTypeEnum.JSON.ToString())
            {
                result = ProcessSet(runProcessInfo);
            }
            else
            {
                result = ProcessGet(runProcessInfo);
            }


            return result;
        }
    }
}

using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Component.Service;
using System;

namespace ProcessAutomation.Component
{
    public class FacebookComponent : TaskComponentBaseBC, ITaskComponent
    {
        // private string accessToken = null;
        private FacebookService facebookService = null;
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;

        public FacebookComponent() : base()
        {
            facebookService = new FacebookService();
        }

        public bool InitComponent()
        {
            return true;
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

        private IObjectType ProcessMethod()
        {
            ObjectType result = null;
            dynamic dynamicValue = processBC.GetProcessedInputValue<dynamic>();
            string pageId = dynamicValue.PageId?.ToString();

            facebookService.SetPageToken(pageId, dynamicValue.AccessToken?.ToString());

            if (pageId == "FEED" && dynamicValue.Message != null)
            {
                facebookService.PublishMessage(dynamicValue.Message.ToString());
            }
            else if (pageId == "PHOTOS" && dynamicValue.Photo != null)
            {
                facebookService.PublishPhotoByUrl(dynamicValue.Photo.ToString());
            }
            else if (pageId == "FEED_AND_PHOTOS" && dynamicValue.Message != null && dynamicValue.Photo != null)
            {
                string photoId = facebookService.PublishPhotoByUrl(dynamicValue.Photo.ToString());
                facebookService.PublishPhotoWithMessage(photoId, dynamicValue.Message.ToString());
            }

            return result;
        }

        private bool FacebookAuthorization()
        {
            bool isValid = false;

            return isValid;
        }       

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            return ProcessMethod();
        }

        public void EndComponent()
        {
        }
    }
}

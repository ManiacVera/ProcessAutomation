using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Attribute;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Timers;

namespace ProcessAutomation.Component
{
    /// <summary>
    /// COMPONENT IS FOR VALIDATION BY ANY FLAG (SQL REGEDIT, FILE TXT, CONTENT FILE, ETC)
    /// </summary>
    [DependencyComponent("Database")]
    public class FlagComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.FINISHED;
        private string databaseComponent = null;
        private string ifComponent = null;

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            dynamic config = processBC.GetProcessedComponentValue<dynamic>();

            if (config != null)
            {
                databaseComponent = config.DatabaseComponent.ToString();
                ifComponent = config.IfComponent.ToString();
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

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            FlagService service = GetComponentService<FlagService>();
            FlagDTO dto = processBC.GetProcessedInputValue<FlagDTO>();

            if (string.IsNullOrWhiteSpace(dto.DatabaseComponent) && !string.IsNullOrEmpty(databaseComponent))
            {
                dto.DatabaseComponent = databaseComponent;
            }

            //if (string.IsNullOrWhiteSpace(dto.IfComponent) && !string.IsNullOrEmpty(ifComponent))
            //{
            //    dto.IfComponent = ifComponent;
            //}
            
            bool resultBool = false;
            int retriesCount = 1;

        RETRIES:
            resultBool = (ObjectType)service.ProcessMethod((IComponentDTO)dto).Result;
            Timer timer = null;
            if (!resultBool && (dto.Retries == -1 || (dto.Retries > 0 && retriesCount <= dto.Retries)))
            {
                if (dto.Interval > 0)
                {
                    bool intervalCompleted = false;
                    timer = new Timer() { Interval = 1000 * dto.Interval };
                    timer.Start();

                    timer.Elapsed += (Object source, System.Timers.ElapsedEventArgs e) =>
                    {
                        intervalCompleted = true;
                    };

                    while (!intervalCompleted) { }

                    timer.Stop();
                    timer.Dispose();
                }

                retriesCount++;
                goto RETRIES;
            }

            return (ObjectType)resultBool;
        }
    }
}
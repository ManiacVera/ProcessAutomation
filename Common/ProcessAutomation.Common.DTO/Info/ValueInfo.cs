using System;

namespace ProcessAutomation.Common.DTO.Info
{
    public class ValueInfo
    {
        public string ProcessName { get; set; }
        public string TaskName { get; private set; }

        /// <summary>
        /// Esta propiedad se agrego con el fin de poder dar valor, por ejemplo: 
        /// Cuando se deshabilita un paso, puede ser que la variable
        /// la esten usando mas adelante, entonces se puede mandar una alerta de error
        /// para evitar deshabilitar ese paso o bien deshabilitar todo lo entrelazado
        /// </summary>
        public int StepNumber { get; private set; }
        public string Key { get; private set; }
        public dynamic Value { get; set; }
        public string ValueType { get; set; }
        public string OutputValueType { get; set; }
        public bool IsPublic { get; set; }
        public string Name { get; private set; }
        public DateTime? LastUpdate { get; set; }
        public int ThreadTaskId { get; private set; }
        public int VirtualParentThreadTaskId { get; private set; }
        public int VirtualThreadTaskId { get; private set; }

        public ValueInfo(string processName, string taskName, int stepNumber, string key, dynamic value, string valueType, string outputValueType, bool isPublic,
            string name, DateTime? lastUpdate, int threadTaskId, int virtualParentThreadTaskId, int virtualThreadTaskId)
        {
            this.ProcessName = processName;
            this.TaskName = taskName;
            this.StepNumber = stepNumber;
            this.Key = key;
            this.Value = value;
            this.ValueType = valueType;
            this.OutputValueType = outputValueType;
            this.IsPublic = isPublic;
            this.Name = name;
            this.LastUpdate = lastUpdate;
            this.ThreadTaskId = threadTaskId;
            this.VirtualParentThreadTaskId = virtualParentThreadTaskId;
            this.VirtualThreadTaskId = virtualThreadTaskId;
        }

        public ValueInfo(ValueInfo valueInfo)
        {
            this.ProcessName = valueInfo.ProcessName;
            this.TaskName = valueInfo.TaskName;
            this.StepNumber = valueInfo.StepNumber;
            this.Key = valueInfo.Key;
            this.Value = valueInfo.Value;
            this.ValueType = valueInfo.ValueType;
            this.OutputValueType = valueInfo.OutputValueType;
            this.IsPublic = valueInfo.IsPublic;
            this.Name = valueInfo.Name;
            this.LastUpdate = valueInfo.LastUpdate;
            this.ThreadTaskId = valueInfo.ThreadTaskId;
            this.VirtualParentThreadTaskId = valueInfo.VirtualParentThreadTaskId;
            this.VirtualThreadTaskId = valueInfo.VirtualThreadTaskId;
        }
    }
}

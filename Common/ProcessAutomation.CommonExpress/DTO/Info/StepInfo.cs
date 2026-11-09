using ProcessAutomation.Framework.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class StepInfo
    {
        public int EncapsulatedId { get; set; }
        public string? EncapsulatedPartitionType { get; set; }
        public int Number {get;set;}
        public string? Description { get; set; }
        public string? Variable { get; set; }
        public bool IsPublicVariable { get; set; }
        public string? Component { get; set; }
        //public int ComponentIndex { get; set; }
        //public string Object {get;set;}


        //public string ObjectType {get;set;}
        //public string SearchType {get;set;}
        public virtual string? Value { get; set; }
        public string? InputValueType { get; set; }
        public string? ValueAction { get; set; }
        public bool ApplyFormula { get; set; }
        public string? OutputValueType { get; set; }
        public string? OutputResultOption { get; set; }
        public string? ValueTypeDeclaration { get; set; }
        public int Delay { get; set; }
        public bool Required { get; set; }
        public bool Async { get; set; }
        public bool Log { get; set; }
        public bool Disable { get; set; }
        public List<ValueOptionInfo>? ValueOptionList { get; set; }
        public List<object>? ParameterList { get; set; }
    }
}

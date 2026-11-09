using ProcessAutomation.Common.DTO.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAutomation.Component
{
    public class BarcodeDTO : IComponentDTO
    {
        public string Action { get; set; }
        public string BarcodeType { get; set; }
        public string OutputPath { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool OnlyBarcode { get; set; }
    }
}

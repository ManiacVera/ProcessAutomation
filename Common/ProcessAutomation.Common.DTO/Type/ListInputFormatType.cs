using ProcessAutomation.Common.DTO.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Common.DTO.Type
{
    public class ListInputFormatType : List<InputFormatInfo>
    {
        public ListInputFormatType()
        {

        }

        public ListInputFormatType(List<InputFormatInfo> collection) : base(collection)
        {
        }

        //public static implicit operator ListInputFormatType(List<InputFormatInfo> collection)
        //{
        //    return new ListInputFormatType(collection);
        //}

        private InputFormatInfo? GetInputFormat(string field)
        {
            InputFormatInfo? inputFormat = null;
            if (Count > 0)
            {
                inputFormat = this.FirstOrDefault(f => f.Field == field);
            }
            return inputFormat;
        }

        public InputFormatInfo? this[string field]
        {
            get { return GetInputFormat(field); }
        }
    }
}

using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Common.DTO.Type
{
    public sealed class TextType : ITextType, IObjectType
    {
        readonly string value;

        public object Value
        {
            get
            {
                return value;
            }
        }

        public TextType(string value)
        {
            this.value = value;
        }

        public static implicit operator string(TextType text)
        {
            return text.value;
        }

        public static implicit operator TextType(string text)
        {
            return new TextType(text);
        }

        public override string? ToString()
        {
            string? result = null;

            if (value != null)
            {
                result = value.ToString();
            }

            return result;
        }
    }
}

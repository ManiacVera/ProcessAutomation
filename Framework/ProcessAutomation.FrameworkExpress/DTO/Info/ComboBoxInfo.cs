namespace ProcessAutomation.Framework.DTO.Info
{
    public class ComboBoxInfo
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public ComboBoxInfo()
        {

        }

        public ComboBoxInfo(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProcessViewerWin.App.Windows.Controls
{
    public enum ProgressBarDisplayText
    {
        Percentage,
        CustomText
    }

    public partial class ProgressBarExtended : ProgressBar
    {
        //Property to set to decide whether to print a % or Text
        private ProgressBarDisplayText m_DisplayStyle;

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public ProgressBarDisplayText DisplayStyle
        {
            get { return m_DisplayStyle; }
            set { m_DisplayStyle = value; }
        }

        //Property to hold the custom text
        private string m_CustomText;

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string CustomText
        {
            get { return m_CustomText; }
            set
            {
                m_CustomText = value;
                this.Invalidate();
            }
        }

        private string fontName;
        private FontStyle fontBold;
        private double fontSize;

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string FontName { get; set; } = "Arial";

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public double FontSize { get; set; } = 9;
        
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public bool FontBold { get; set; } = false;
        //public string FontColor { get; set; }
        //public bool FontItalic { get; set; }

        private const int WM_PAINT = 0x000F;

        public ProgressBarExtended()
        {   
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            fontName = string.IsNullOrEmpty(FontName) ? "Arial" : FontName;
            fontBold = FontBold ? FontStyle.Bold : FontStyle.Regular;
            fontSize = FontSize;
            //string fontColor = string.IsNullOrEmpty(ForeColor) ? ForeColor : FontColor;

            switch (m.Msg)
            {
                case WM_PAINT:
                    int m_Percent = Convert.ToInt32((Convert.ToDouble(Value) / Convert.ToDouble(Maximum)) * 100);
                    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;

                    using (Graphics g = Graphics.FromHwnd(Handle))
                    {
                        using (Brush textBrush = new SolidBrush(ForeColor))
                        {

                            switch (DisplayStyle)
                            {
                                case ProgressBarDisplayText.CustomText:
                                    TextRenderer.DrawText(g, CustomText, new Font(fontName, Convert.ToSingle(fontSize), fontBold), new Rectangle(0, 0, this.Width, this.Height), Color.Black, flags);
                                    break;
                                case ProgressBarDisplayText.Percentage:
                                    TextRenderer.DrawText(g, string.Format("{0}%", m_Percent), new Font(fontName, Convert.ToSingle(fontSize), fontBold), new Rectangle(0, 0, this.Width, this.Height), Color.Black, flags);
                                    break;
                            }

                        }
                    }

                    break;
            }

        }

    }
}

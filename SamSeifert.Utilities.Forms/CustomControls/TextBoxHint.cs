using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.CustomControls
{
    /// <summary>
    /// A textbox that supports a watermak hint.
    /// </summary>
    public class TextBoxHint : TextBox
    {
        new readonly bool DesignMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

        /// <summary>
        /// Gets or Sets the text that will be presented as the watermak hint
        /// </summary>
        public string WatermarkText { get; set; } = "This is a text box!";

        /// <summary>
        /// Create a new TextBox that supports watermak hint
        /// </summary>
        public TextBoxHint()
        {
            this.ApplyWatermark();

            GotFocus += (source, e) =>
            {
                RemoveWatermak();
            };

            LostFocus += (source, e) =>
            {
                ApplyWatermark();
            };

        }

        /// <summary>
        /// Remove watermark from the textbox
        /// </summary>
        private void RemoveWatermak()
        {
            if (this.DesignMode) return;
            if (this.Text == this.WatermarkText)
            {
                this.Text = "";
                this.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Applywatermak immediately
        /// </summary>
        private void ApplyWatermark()
        {
            if (this.DesignMode) return;
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = WatermarkText;
                this.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// Apply watermak to the textbox. 
        /// </summary>
        /// <param name="newText">Text to apply</param>
        public void ApplyWatermark(string newText)
        {
            if (this.DesignMode) return;
            WatermarkText = newText;
            ApplyWatermark();
        }    
    }
}

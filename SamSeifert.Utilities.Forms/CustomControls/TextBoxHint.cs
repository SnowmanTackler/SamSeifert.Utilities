using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Gets or Sets the text that will be presented as the watermak hint
        /// </summary>
        public string WatermarkText { get; set; } = "Type here";

        /// <summary>
        /// Whether watermark effect is enabled or not
        /// </summary>
        public bool WatermarkActive { get; set; } = true;

        /// <summary>
        /// Create a new TextBox that supports watermak hint
        /// </summary>
        public TextBoxHint()
        {
            this.Text = WatermarkText;
            this.ForeColor = Color.Gray;

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
        public void RemoveWatermak()
        {
            if (this.WatermarkActive)
            {
                this.WatermarkActive = false;
                this.Text = "";
                this.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Applywatermak immediately
        /// </summary>
        public void ApplyWatermark()
        {
            if (!this.WatermarkActive && string.IsNullOrEmpty(this.Text)
                || ForeColor == Color.Gray)
            {
                this.WatermarkActive = true;
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
            WatermarkText = newText;
            ApplyWatermark();
        }    
    }
}

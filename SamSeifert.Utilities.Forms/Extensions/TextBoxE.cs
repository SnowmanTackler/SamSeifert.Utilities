using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.Utilities.Concurrent;
using SamSeifert.Utilities.Files.Json;
using SamSeifert.Utilities.Logging;

namespace SamSeifert.Utilities.Extensions
{
    public static class TextBoxE
    {
        public static void Unpack(this TextBox cb, JsonDict dict, string key)
        {
            object outo;
            if (dict.TryGetValue(key, out outo))
                cb.Text = outo as String;
        }

        public static void Pack(this TextBox cb, JsonDict dict, string key)
        {
            dict[key] = cb.Text;
        }

        public class TextBoxLogger : BaseLogger
        {
            private readonly BaseLogger WrappedLogger = new TimeLogger();
            private readonly TextBox Box;

            public TextBoxLogger(TextBox box)
            {
                this.Box = box;
            }

            public override string Write(DateTime time, LogLevel level, string message, Exception exc)
            {
                var str = WrappedLogger.Write(time, level, message, exc);

                if (this.Box == null) return str;
                if (this.Box.IsDisposed) return str;

                var act = new Action(() => {
                    this.Box.AppendText(message + Environment.NewLine);
                });

                if (this.Box.InvokeRequired) this.Box.BeginInvoke(act); // Don't Wait
                else act();

                return str;
            }
        }

        public class TextBoxLogger2 : BaseLogger
        {
            private readonly BaseLogger WrappedLogger = new TimeLogger();
            private readonly BackgroundTaskManager manager;
            private readonly TextBox Box;

            public TextBoxLogger2(BackgroundTaskManager manager, TextBox box)
            {
                this.manager = manager;
                this.Box = box;
            }

            public override string Write(DateTime time, LogLevel level, string message, Exception exc)
            {
                var str = WrappedLogger.Write(time, level, message, exc);
                manager.RunOnMainThread(() =>
                {
                    this.Box.AppendText(message + Environment.NewLine);
                });
                return str;
            }
        }

        /// <summary>
        /// For Console Logging
        /// </summary>
        public static void UseAsLogger(this TextBox tb)
        {
            Logger.Default = new TextBoxLogger(tb);
        }

    }
}

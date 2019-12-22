using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Logging
{
    public abstract class BaseLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="exc">Can be null.</param>
        public abstract string Write(DateTime time, LogLevel level, String message, Exception exc);

        public void Debug(String message, Exception exc = null)
        {
            this.Write(DateTime.Now, LogLevel.Debug, message, exc);
        }

        public void Info(String message, Exception exc = null)
        {
            this.Write(DateTime.Now, LogLevel.Info, message, exc);
        }

        public void Warn(String message, Exception exc = null)
        {
            this.Write(DateTime.Now, LogLevel.Warn, message, exc);
        }

        public void Error(String message, Exception exc = null)
        {
            this.Write(DateTime.Now, LogLevel.Error, message, exc);
        }

        public static string ToStringLite(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return "D";
                case LogLevel.Info: return "I";
                case LogLevel.Warn: return "W";
                case LogLevel.Error: return "E";
                default: return "?";
            }
        }
    }
}

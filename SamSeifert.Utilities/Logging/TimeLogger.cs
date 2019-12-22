using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Logging
{
    public class TimeLogger : BaseLogger
    {
        private readonly string DateTimeFormat = "HH:mm:ss.fff";

        public override string Write(DateTime time, LogLevel level, string message, Exception exc)
        {
            var sb = new StringBuilder();

            sb.Append(BaseLogger.ToStringLite(level));
            sb.Append(" ");
            sb.Append(time.ToString(DateTimeFormat));
            sb.Append(" ");
            sb.Append(message);

            var str = sb.ToString();

            Console.WriteLine(str);

            return str;
        }
    }
}

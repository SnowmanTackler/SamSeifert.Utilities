using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public class Logger : IDisposable
    {
        public static void WriteLine()
        {
            Logger._Writer("", _IndentLock_Indent);
        }

        public static void WriteLine(String s)
        {
            Logger._Writer(s, _IndentLock_Indent);
        }

        public static void WriteLine(params object[] doug)
        {
            Logger._Writer(String.Join(", ", doug), _IndentLock_Indent);
        }

        public static void WriteWarning(Object sender, String hint)
        {
            Type t = (sender is Type) ? (sender as Type) : sender.GetType();
            Logger._Writer("Warning - " + t.FullName + ": " + hint, _IndentLock_Indent);
        }

        public static void WriteError(Object sender, String hint)
        {
            Type t = (sender is Type) ? (sender as Type) : sender.GetType();
            Logger._Writer("Error - " + t.FullName + ": " + hint, _IndentLock_Indent);
        }

        public static void WriteException(Object sender, String hint, Exception exc)
        {
            Type t = (sender is Type) ? (sender as Type) : sender.GetType();
            Logger._Writer("Exception - " + t.FullName + ": " + hint + ", " + exc.ToString(), _IndentLock_Indent);
        }

        private static volatile Action<String, String> _Writer = (String message, String indent) => {
            Console.WriteLine(indent + message);
        };

        public static Action<String> Writer
        {
            set
            {
                Logger._Writer = (String message, String indent) => value(message);
            }
        }

        public static Action<String, String> WriterSupportTabs
        {
            set
            {
                Logger._Writer = value;
            }
        }

        private Func<String> _DisposeFunction;
        private int _AddedIndentLength;

        private readonly static object _IndentLock = new object();
        private static String _IndentLock_IndentString = "\t";
        private static String _IndentLock_Indent = "";

        /// <summary>
        /// Can't be removed
        /// </summary>
        public static void AddTab()
        {
            lock (_IndentLock)
            {
                Logger._IndentLock_Indent += Logger._IndentLock_IndentString;
            }
        }

        private Logger(Func<String> dispose_function)
        {
            lock (_IndentLock)
            {
                this._AddedIndentLength = _IndentLock_IndentString.Length;
                Logger._IndentLock_Indent += _IndentLock_IndentString;
            }
            this._DisposeFunction = dispose_function;
        }

        public void Dispose()
        {
            lock (_IndentLock)
            {
                _IndentLock_Indent = _IndentLock_Indent.Substring(0, _IndentLock_Indent.Length - this._AddedIndentLength);
            }
            Logger.WriteLine(this._DisposeFunction());
            this._DisposeFunction = null;
        }

        public static Logger Time(String message)
        {
            Logger.WriteLine(message);
            var stp = new Stopwatch();
            stp.Start();
            return new Logger(() =>
            {
                var elapsed = stp.Elapsed;
                return message + " ... " + elapsed.TotalSeconds.ToString("0.00") + " seconds";
            });
        }
    }
}

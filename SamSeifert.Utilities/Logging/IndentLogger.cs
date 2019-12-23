using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Logging
{
    public class IndentLogger : BaseLogger
    {

        private readonly BaseLogger WrappedLogger;
        public int IndentLength = 0;

        public IndentLogger()
        {
            this.WrappedLogger = new TimeLogger();
        }

        public IndentLogger(BaseLogger baseLogger)
        {
            this.WrappedLogger = baseLogger;
        }

        private class Indenter : IDisposable
        {
            private Action _DisposeFunction;

            public Indenter(Action dispose_function)
            {
                this._DisposeFunction = dispose_function;
            }

            public void Dispose()
            {
                this._DisposeFunction();
                this._DisposeFunction = null;
            }
        }

        public IDisposable Time(String message)
        {
            this.Info(message);
            this.IndentLength++;
            var stp = new Stopwatch();
            stp.Start();
            return new Indenter(() =>
            {
                var elapsed = stp.Elapsed;
                this.IndentLength--;
                this.Info(message + " ... " + elapsed.TotalSeconds.ToString("0.00") + " seconds");
            });
        }

        public override string Write(DateTime time, LogLevel level, string message, Exception exc)
        {
            var tabbedMessage = new string(' ', this.IndentLength * 4) + message;
            return this.WrappedLogger.Write(time, level, tabbedMessage, exc);
        }
    }
}

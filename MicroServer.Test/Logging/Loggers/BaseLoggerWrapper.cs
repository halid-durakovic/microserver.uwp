using System;
using System.Collections.Generic;
using System.Text;

using MicroServer.Logging;
using MicroServer.Logging.Loggers;

namespace MicroServer.Test.Logging.Loggers
{
    public class BaseLoggerWrapper : BaseLogger
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseLogger" /> class.
        /// </summary>
        /// <param name="typeThatLogs">Type being logged.</param>
        public BaseLoggerWrapper(Type typeThatLogs)
            : base(typeThatLogs)
        {
            Entries = new List<LogEntry>();
        }


        public List<LogEntry> Entries { get; set; }

        public string FormatException(Exception exception)
        {
            var sb = new StringBuilder();
            base.BuildExceptionDetails(exception, 4, sb);
            return sb.ToString();
        }

        public override void Write(LogEntry entry)
        {
            Entries.Add(entry);
        }
    }
}
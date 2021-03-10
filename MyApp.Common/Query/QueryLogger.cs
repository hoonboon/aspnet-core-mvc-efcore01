using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MyApp.Common.Query
{
    public class QueryLogger : IDisposable
    {
        private readonly string _command;
        private readonly ILogger _logger;
        private readonly Stopwatch stopwatch = new Stopwatch();

        public QueryLogger(string command, ILogger logger)
        {
            _command = command;
            _logger = logger;
            stopwatch.Start();
        }

        public void Dispose()
        {
            stopwatch.Stop();
            _logger.LogInformation($"Query [Execution time = {stopwatch.ElapsedMilliseconds}ms]: " + _command);
        }
    }
}

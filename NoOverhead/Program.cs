using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

// c# 8 - by default no nulls
#nullable enable

namespace NoOverhead
{
    // The class is oversimplified for the sake of example
    internal class Logger
    {
        public Logger(ILog log) => Log = log;
        // legacy approach 
        public ILog Log { get; }
        // the pattern can be extended to WARN, INFO ... levels
        public ILog? LogIf => Log.IsDebugEnabled ? Log : null;

    }
    /****************************
     * 1) We have many millions of calls in an average workflow
     * 2) Most of the code is instrumented with logging
     * 3) By default the configuration is WARN so debug / info messages would be NOP
     * 4) Let's see what is the fastest way to NOP
     ****************************/
    [MemoryDiagnoser]
    public class Benchmark
    {
        private readonly Logger _log;
        public Benchmark() => _log = new Logger(LogManager.GetLogger(typeof(Benchmark)));

        
        [Benchmark(Baseline = true)]
        public void Nop()
        {
            const int sampleId = 10;
            for (int i = 0; i < 100_000; i++)
            {
                // log level is WARN so this is a NOP operation
                _log.Log.DebugFormat($"{nameof(Nop)} is called with {nameof(sampleId)} and value of {sampleId + i}");
            }
            
        }
        [Benchmark()]
        public void NopOptimized()
        {
            const int sampleId = 10;
            for (int i = 0; i < 100_000; i++)
            {
                // log level is WARN so this is a NOP operation
                _log.LogIf?.DebugFormat($"{nameof(Nop)} is called with {nameof(sampleId)} and value of {sampleId + i}");
            }
            
        }

    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            // Uncomment the next line to enable log4net internal debugging
            log4net.Util.LogLog.InternalDebugging = true;

            //var benchmark
            //    = new Benchmark();
            //benchmark.Nop();
            //benchmark.NopOptimized();

            var summary = BenchmarkRunner.Run<Benchmark>();
            // This will shutdown the log4net system
            log4net.LogManager.Shutdown();
            System.Console.ReadLine();
        }
    }
}

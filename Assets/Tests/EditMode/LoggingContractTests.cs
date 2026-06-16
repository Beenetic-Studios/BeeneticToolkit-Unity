using System.Collections.Generic;
using NUnit.Framework;
using BeeneticToolkit.Logging;
using BeeneticToolkit.Logging.Enums;

namespace BeeneticToolkit.Unity.Tests {

    // Exercises the LogManager fan-out, a custom LoggerBase sink (the Unity-sink pattern), and
    // LoggerService null-safety.
    public class LoggingContractTests {

        private sealed class CaptureLogger : LoggerBase {
            public readonly List<string> Entries = new List<string>();
            public CaptureLogger() : base("capture") { }
            protected override void WriteEntry(LogSeverity severity, string entry) => Entries.Add(entry);
        }

        [Test]
        public void Manager_FansMessageOutToSink() {
            var sink = new CaptureLogger();
            var log = new LogManager().AddLogger(sink);

            log.Info("hello", this);

            Assert.AreEqual(1, sink.Entries.Count);
            StringAssert.Contains("hello", sink.Entries[0]);
        }

        [Test]
        public void LoggerService_IsNullSafe_WhenNotInitialized() {
            LoggerService.Initialize(null);
            Assert.DoesNotThrow(() => LoggerService.Info("no-op before init"));
        }
    }
}

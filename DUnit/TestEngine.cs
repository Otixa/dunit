using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DUnit
{
    public class TestEngine
    {
        private NLog.ILogger logger;
        public TestEngine(System.IO.FileInfo scriptPath, System.IO.DirectoryInfo testDirectory, System.IO.FileInfo logOutputPath)
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting test engine");
            logger.Info("Script Path : {0}", scriptPath.FullName);
            logger.Info("Test Directory : {0}", testDirectory.FullName);

            logger.Info("Initializing Universe");
            var environment = new DU.DUEnvironment();

            logger.Info("Loading compiled script");
            var scriptModule = Newtonsoft.Json.JsonConvert.DeserializeObject<DU.OutputModule>(System.IO.File.ReadAllText(scriptPath.FullName));
            environment.LoadScript(scriptModule);

            var results = new Dictionary<string, bool>();
            var junitLogger = new JUnitLog();

            foreach (var test in testDirectory.EnumerateFiles("*.lua", System.IO.SearchOption.AllDirectories))
            {
                logger.Debug("Resetting universe");
                logger.Info("Running tests in {0}", test.FullName);
                environment.Scaffold();
                var start = DateTime.UtcNow;
                using (var sr = new System.IO.StreamReader(test.OpenRead()))
                {
                    try
                    {
                        var result = environment.ExecuteLua(sr.ReadToEnd());
                        results[test.Name] = true;
                        junitLogger.AddSuccess(test.Name, DateTime.UtcNow - start);
                        logger.Info("Test {0} was successful", test.Name);
                    }
                    catch (Exception e)
                    {
                        results[test.Name] = false;
                        junitLogger.AddFailure(test.Name, e.Message, DateTime.UtcNow - start);
                        logger.Error("Test {0} failed", test.Name);
                    }
                }
            }

            if (logOutputPath != null) {
                logOutputPath.Directory.Create();
                var junitLog = junitLogger.Serialize();
                System.IO.File.WriteAllText(logOutputPath.FullName, junitLog);
            }

            if (results.Any(x => !x.Value)) throw new Exception("One or more tests failed");

        }
    }
}

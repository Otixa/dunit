using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DUnit
{
    public class TestEngine
    {
        private NLog.ILogger logger;
        public TestEngine(System.IO.DirectoryInfo luaDirectory, System.IO.FileInfo scriptPath, System.IO.DirectoryInfo testDirectory, System.IO.FileInfo logOutputPath)
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting test engine");
            logger.Info("Script Path : {0}", scriptPath.FullName);
            logger.Info("Test Directory : {0}", testDirectory.FullName);

            

            logger.Info("Loading compiled script");
            var scriptModule = Newtonsoft.Json.JsonConvert.DeserializeObject<DU.OutputModule>(System.IO.File.ReadAllText(scriptPath.FullName));

            logger.Info("Initializing Universe");
            var environment = new DU.DUEnvironment(luaDirectory, scriptModule);

            var results = new Dictionary<string, bool>();
            var junitLogger = new JUnitLog();

            foreach (var test in testDirectory.EnumerateFiles("*.lua", System.IO.SearchOption.AllDirectories))
            {
                logger.Debug("Resetting universe");
                logger.Info("Running tests in {0}", test.FullName);
                var testEnvironment = environment.BuildEnvironment();
                var start = DateTime.UtcNow;
                using (var sr = new System.IO.StreamReader(test.OpenRead()))
                {
                    MoonSharp.Interpreter.Table testUnitTable = null;
                    try
                    {
                        testUnitTable = environment.LoadTest(testEnvironment, sr.ReadToEnd());

                        MoonSharp.Interpreter.Closure OTS = testUnitTable["OneTimeSetup"] as MoonSharp.Interpreter.Closure;
                        MoonSharp.Interpreter.Closure OTC = testUnitTable["OneTimeCleanup"] as MoonSharp.Interpreter.Closure;
                        MoonSharp.Interpreter.Closure TS = testUnitTable["Setup"] as MoonSharp.Interpreter.Closure;
                        MoonSharp.Interpreter.Closure TC = testUnitTable["Cleanup"] as MoonSharp.Interpreter.Closure;
                        testUnitTable.Remove("OneTimeSetup");
                        testUnitTable.Remove("OneTimeCleanup");
                        testUnitTable.Remove("Setup");
                        testUnitTable.Remove("Cleanup");

                        //Run One Time Setup
                        OTS?.Call();
                        //Run Setup
                        //Run Test
                        //Run Cleanup
                        foreach (var tablePair in testUnitTable.Pairs)
                        {
                            var testName = tablePair.Key.String ?? "Unknown";
                            try
                            {
                                TS?.Call();
                                tablePair.Value.Function.Call();
                                TC?.Call();

                                results[new Guid().ToString()] = true;
                                junitLogger.AddSuccess(scriptPath.Name, testName, DateTime.UtcNow - start);
                                logger.Info("Test {0} was successful", testName);
                            }
                            catch (Exception e)
                            {
                                results[new Guid().ToString()] = false;
                                junitLogger.AddFailure(scriptPath.Name, testName, e.Message, DateTime.UtcNow - start);
                                logger.Error("Test {0} failed with error {1}", testName, e.Message);
                            }
                        }
                        //Run One Time Cleanup

                        OTC?.Call();
                    }
                    catch (Exception e)
                    {
                        results[new Guid().ToString()] = false;
                        junitLogger.AddFailure(scriptPath.Name, test.Name, e.Message, DateTime.UtcNow - start);
                        logger.Error("Test {0} failed with error {1}", test.Name, e.Message);
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

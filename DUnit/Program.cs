using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace DUnit
{
    enum ExitCode : int
    {
        Success = 0,
        GeneralError = 1,
        TestError = 2
    }

    class Program
    {
        static int Main(string[] args)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting DUnit");

            var rootDir = AppDomain.CurrentDomain.BaseDirectory;
            var versionHash = System.IO.File.ReadAllText(System.IO.Path.Combine(rootDir, "versionhash"));
            var luaDirectory = new System.IO.DirectoryInfo(System.IO.Path.Combine(rootDir, "LuaLibraries"));


            logger.Info($"Version {versionHash}");

            var app = new CommandLineApplication();
            app.Name = "DUnit";
            app.Description = "Dual Universe script unit tester";
            app.HelpOption("-?|-h|--help");

            try
            {
                app.Command("test", (command) =>
            {
                command.Description = "Run a set of unit tests on a DU script";

                var scriptPath = command.Option("-s|--script", "DUBuild output script, or any pastable lua code - supports wildcards", CommandOptionType.SingleValue);
                var testsPath = command.Option("-t|--testpath", "Path to the folder containing test units", CommandOptionType.SingleValue);
                var logPath = command.Option("-l|--log", "Path to the junit log output irectory", CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    if (!scriptPath.HasValue())
                    {
                        logger.Error("Missing script path");
                        app.ShowHint();
                        Environment.Exit(2);
                    }

                    if (!testsPath.HasValue())
                    {
                        logger.Error("Missing test directory path");
                        app.ShowHint();
                        Environment.Exit(2);
                    }

                    var scriptPathInfo = new System.IO.FileInfo(scriptPath.Value());
                    var testsPathInfo = new System.IO.DirectoryInfo(testsPath.Value());
                    System.IO.DirectoryInfo logPathInfo = logPath.HasValue() ? new System.IO.DirectoryInfo(logPath.Value()) : null;

                    var matchedSourceFiles = new List<System.IO.FileInfo>();
                    matchedSourceFiles.AddRange(scriptPathInfo.Directory.EnumerateFiles(scriptPathInfo.Name));

                    logger.Info("Matched {0} source files", matchedSourceFiles.Count);

                    bool failed = false;

                    foreach(var scriptFilePath in matchedSourceFiles)
                    {
                        try
                        {
                            if (scriptFilePath.Name.Contains(".min.")) continue;
                            logger.Info("Processing {0}", scriptFilePath.FullName);
                            var logFileName = $"{scriptFilePath.Name}.xml";
                            var logFile = logPathInfo == null ? new System.IO.FileInfo(logFileName) : new System.IO.FileInfo(System.IO.Path.Combine(logPathInfo.FullName, logFileName));
                            var testEngine = new TestEngine(luaDirectory, scriptFilePath, testsPathInfo, logFile);
                        }
                        catch (Exception e)
                        {
                            logger.Error("Error processing {0}, {1}", scriptFilePath.Name, e.Message);
                            failed = true;
                        }
                       
                    }
                    if (failed)
                    {
                        throw new TestRunException("One or more of the unit tests failed");
                    }
                    return 0;
                });


            });
                app.Execute(args);
            }
            catch (TestRunException ex)
            {
                logger.Fatal(ex.Message);
                return (int)ExitCode.TestError;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return (int)ExitCode.GeneralError;
            }
            return (int)ExitCode.Success;
        }
    }
}

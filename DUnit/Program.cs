using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace DUnit
{
    class Program
    {
        static void Main(string[] args)
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

                    foreach(var scriptFilePath in matchedSourceFiles)
                    {
                        logger.Info("Processing {0}", scriptFilePath.FullName);
                        if (scriptFilePath.Name.Contains(".min.")) return 0;
                        var logFileName = $"{scriptFilePath.Name}.xml";
                        var logFile = logPathInfo == null ? new System.IO.FileInfo(logFileName) : new System.IO.FileInfo(System.IO.Path.Combine(logPathInfo.FullName, logFileName));
                        var testEngine = new TestEngine(luaDirectory, scriptFilePath, testsPathInfo, logFile);
                    }

                    return 0;
                });


            });
            app.Execute(args);

        }
    }
}

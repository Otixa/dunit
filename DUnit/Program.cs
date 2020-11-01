using Microsoft.Extensions.CommandLineUtils;
using System;

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
                    System.IO.FileInfo logPathInfo = logPath.HasValue() ? new System.IO.FileInfo(logPath.Value()) : null;

                    foreach(var scriptFilePath in scriptPathInfo.Directory.EnumerateFiles(scriptPathInfo.Name, System.IO.SearchOption.TopDirectoryOnly))
                    {
                        var logFileName = $"{scriptFilePath.Name}.xml";
                        var logFile = logPathInfo == null ? new System.IO.FileInfo(logFileName) : new System.IO.FileInfo(System.IO.Path.Combine(logPathInfo.Directory.FullName, logFileName));
                        var testEngine = new TestEngine(scriptPathInfo, testsPathInfo, logFile);
                    }

                    return 0;
                });


            });
            app.Execute(args);

        }
    }
}

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

                var scriptPath = command.Option("-s|--script", "DUBuild output script, or any pastable lua code", CommandOptionType.SingleValue);
                var testsPath = command.Option("-t|--testpath", "Path to the folder containing test units", CommandOptionType.SingleValue);
                var logPath = command.Option("-l|--log", "Path to the junit output log", CommandOptionType.SingleValue);

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

                    var testEngine = new TestEngine(scriptPathInfo, testsPathInfo, logPathInfo);

                    return 0;
                });


            });
            app.Execute(args);

        }
    }
}

using System;
using ManyConsole;
using Serilog;
using StructureMap;

namespace Utilitarian.Migrations.Console
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var container = Container.For<ConsoleRegistry>();
            var commands = container.GetAllInstances<ConsoleCommand>();

            var exitCode = ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);

            Log.Information("Complete");

            System.Console.ReadKey();

            return exitCode;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;

            Log.Error("An unhandled exception has occurred: {0}", exception.Message);
            Log.Error(exception, "");

            System.Console.ReadKey();

            Environment.Exit(-1);
        }
    }
}

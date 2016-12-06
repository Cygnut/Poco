using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using NDesk.Options;

namespace Poco.Voter
{
    using Core;
    using Core.Configuration;

    class Program
    {
        static void Run(string databaseConnectionString)
        {
            // Load configuration.
            var config = ConfigurationReader<Config>.FromSource(
                new MySqlDbSource(databaseConnectionString, "Poco.Voter"),
                new Dictionary<string, object> { { "DatabaseConnectionString", databaseConnectionString } }
            );

            // Dump configuration.
            ConfigurationReader<Config>.WriteParameterValues(Console.Out, config);

            // Run the main program.
            var cancel = new CancellationTokenSource();
            new Voter().Execute(config, cancel);
        }

        static void Main(string[] args)
        {
            // Interpret the command line.
            string databaseConnectionString = null;
            bool pause = false;
            bool config = false;
            bool help = false;            

            try
            {
                var p = new OptionSet()
                {
                    {
                        "d|database=",
                        "MySql database connection string.",
                        v => databaseConnectionString = v 
                    },
                    {
                        "p|pause",
                        "Pause the application when done.",
                        v => pause = v != null  // Flag
                    },
                    {
                        "c|config",
                        "Show database configuration options.",
                        v => config = v != null // Flag
                    },
                    {
                        "h|help",
                        "Show help.",
                        v => help = v != null   // Flag
                    },
                };

                p.Parse(args);

                if (config)
                {
                    ConfigurationReader<Config>.WriteParameterDescriptions(Console.Out);
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadKey();
                    return;
                }

                if (help)
                {
                    Console.WriteLine("{0} v{1}", AssemblyDetails.Title, AssemblyDetails.Version);
                    Console.WriteLine(AssemblyDetails.Description);
                    Console.WriteLine("Options:");
                    p.WriteOptionDescriptions(Console.Out);
                    return;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to read command line with error {0}", e);
                Environment.Exit(1);
                return;
            }

            // Run the main program.
            try
            {
                Run(databaseConnectionString);
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during voting: {0}", e);
                Environment.Exit(1);
                return;
            }
            finally
            {
                if (pause)
                {
                    Console.WriteLine("Press a key to continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}

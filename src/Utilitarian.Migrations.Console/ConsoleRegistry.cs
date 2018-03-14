﻿using ManyConsole;
using StructureMap;
using Utilitarian.Migrations.Interfaces;
using Utilitarian.Migrations.Models;
using Utilitarian.Migrations.Services;
using Utilitarian.Settings;

namespace Utilitarian.Migrations.Console
{
    public class ConsoleRegistry : Registry
    {
        public ConsoleRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<Migration>();

                scan.WithDefaultConventions();

                scan.AddAllTypesOf<ConsoleCommand>();
                scan.AddAllTypesOf<Migration>();
                scan.AddAllTypesOf<IVersionRepository>();
                scan.AddAllTypesOf<IDatabaseUtilityService>();
            });
            
            For<IConnectionStringProvider>().Use<AppSettingsConnectionStringProvider>();
            For<IRepositoryService>().Use<RepositoryService>();
        }
    }
}
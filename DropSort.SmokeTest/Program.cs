using Infrastructure;
using Infrastructure.Persistence.Sqlite;
using DropSort.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// ===== DI =====
services.AddInfrastructure("dropsort.db");

// ===== BUILD PROVIDER =====
var provider = services.BuildServiceProvider();

// ===== INIT DB =====
provider
    .GetRequiredService<DatabaseInitializer>()
    .Initialize();

// ===== SET download_root (CHỈ SET) =====
var settings = provider.GetRequiredService<ISettingRepository>();

settings.Set("download_root", @"D:\SortedDownloads");

Console.WriteLine("download_root set to: " + settings.Get("download_root"));
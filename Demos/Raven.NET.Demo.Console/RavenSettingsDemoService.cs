using Raven.NET.Core.Providers.Interfaces;

namespace Raven.NET.Demo.Console;

public class RavenSettingsDemoService
{
    private readonly IRavenSettingsProvider _settingsProvider;

    public RavenSettingsDemoService(IRavenSettingsProvider settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }

    public void Run()
    {
        System.Console.WriteLine("--- Raven setting tests. ---");
        System.Console.WriteLine("Ravens configuration changes dynamically when appsettings.json is changed.");
        System.Console.WriteLine("To exit demo type q\n");

        do
        {
            var ravenSettings = _settingsProvider.GetRavens();

            foreach (var raven in ravenSettings)
            {
                System.Console.WriteLine(
                    $"Found config for: {raven.Key} with values: {raven.Value.AutoDestroy}, {raven.Value.LogLevel}");
            }

        } while (System.Console.ReadLine() != "q");
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: timer-cli [duration_in_minutes] [alert_interval_in_seconds]");
            return;
        }

        int durationMinutes = int.Parse(args[0]);
        int alertIntervalSeconds = int.Parse(args[1]);

        var exeDirectory = AppContext.BaseDirectory;
        var appSettingsPath = Path.Combine(exeDirectory, "appsettings.json");

        var builder = new ConfigurationBuilder()
            .SetBasePath(exeDirectory)
            .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true);

        var serviceProvider = new ServiceCollection()
            .AddLogging(configure =>
            {
                configure.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "hh:mm:ss ";
                }).SetMinimumLevel(LogLevel.Information);
            })
            .AddSingleton<TimerService>()
            .BuildServiceProvider();

        var timerService = serviceProvider.GetService<TimerService>();
        await timerService.RunTimerAsync(durationMinutes, alertIntervalSeconds);
    }
}

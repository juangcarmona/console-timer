using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

public class TimerService
{
    private readonly ILogger<TimerService> _logger;

    public TimerService(ILogger<TimerService> logger)
    {
        _logger = logger;
    }

    public async Task RunTimerAsync(int durationMinutes, int alertIntervalSeconds)
    {
        int totalSeconds = durationMinutes * 60;
        int elapsedSeconds = 0;
        int lastAlert = 0;

        _logger.LogInformation($"Starting countdown for {durationMinutes} minutes with alerts every {alertIntervalSeconds} seconds.");

        while (elapsedSeconds < totalSeconds)
        {
            await Task.Delay(1000);
            elapsedSeconds++;

            ShowProgress(elapsedSeconds, totalSeconds);

            // Emitir un sonido cada vez que se alcanza el intervalo
            if (elapsedSeconds - lastAlert >= alertIntervalSeconds)
            {
                PlaySound();
                lastAlert = elapsedSeconds;
            }
        }

        Console.WriteLine("\nTime's up!");
        PlaySound();
    }

    private void ShowProgress(int current, int total)
    {
        double progress = (double)current / total;
        int progressWidth = 50;
        int filled = (int)(progress * progressWidth);
        string bar = new string('#', filled).PadRight(progressWidth);
        Console.CursorLeft = 0;
        Console.Write($"[{bar}] {progress * 100:F1}%");
    }

    private void PlaySound()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Para Windows
                Console.Beep(1000, 500);
            }
            else
            {
                // get absolute path to sound file at assets/sound.wav
                string soundFile = GetSoundFilePath("assets/sound.wav");
                PlaySoundOnLinux(soundFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing sound: {ex.Message}");
        }
    }


    private static string GetSoundFilePath(string relativePath)
    {
        // 1. Buscar en AppContext.BaseDirectory
        string baseDirectoryPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (File.Exists(baseDirectoryPath))
        {
            return baseDirectoryPath;
        }

        // 2. Buscar un nivel más arriba
        string parentDirectoryPath = Path.Combine(AppContext.BaseDirectory, "..", relativePath);
        if (File.Exists(parentDirectoryPath))
        {
            return Path.GetFullPath(parentDirectoryPath);
        }

        return null; // Si no se encontró el archivo
    }

    private static void PlaySoundOnLinux(string soundFile)
    {
        try
        {
            Process.Start("aplay", soundFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to play sound on Linux: {ex.Message}");
            // Tell the user to verify alsa-utils ins installed
            Console.WriteLine("Make sure 'alsa-utils' is installed and the sound file path is correct.");

        }
    }
}

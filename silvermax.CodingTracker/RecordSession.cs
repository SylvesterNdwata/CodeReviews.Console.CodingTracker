using Spectre.Console;
using System.Diagnostics;

namespace silvermax.CodingTracker;

internal class RecordSession
{
    private Stopwatch? _stopwatch;
    private readonly CodingController _controller = new();
    public void Start()
    {
        if (_stopwatch != null && _stopwatch.IsRunning) return;

        AnsiConsole.MarkupLine("[yellow]Session starting now[/]");
        Thread.Sleep(1000);
        Console.Clear();

        AnsiConsole.MarkupLine("[red]Press 'S' to stop the session[/]\n");

        _stopwatch = new Stopwatch();

        DateTime startTime = DateTime.UtcNow;
        _stopwatch.Start();

        int timerRow = Console.CursorTop;

        var timerThread = new Thread(() =>
        {
            while (_stopwatch.IsRunning)
            {
                Console.SetCursorPosition(0, timerRow);
                AnsiConsole.Markup(
                    $"[blue]Elapsed: {_stopwatch.Elapsed:hh\\:mm\\:ss}[/]    ");
                Thread.Sleep(1000);
            }
        });

        timerThread.Start();

        while (Console.ReadKey(true).Key != ConsoleKey.S) { }

        _stopwatch.Stop();

        DateTime endTime = DateTime.UtcNow;
        timerThread.Join();

        Console.SetCursorPosition(0, timerRow + 2);
        AnsiConsole.MarkupLine("\n[green]Session Stopped![/]");
        AnsiConsole.MarkupLine($"Session duration: [cyan]{_stopwatch.Elapsed:hh\\:mm\\:ss}[/]");

        string start = startTime.ToString("HH:mm");
        string end = endTime.ToString("HH:mm");
        string date = DateTime.UtcNow.ToString("dd-MMM-yyyy");
        string duration = $"{_stopwatch.Elapsed:hh\\:mm}";

        _controller.AddToDb(date, start, end, duration);

        AnsiConsole.MarkupLine("[green]Session successfully saved[/]");

        AnsiConsole.MarkupLine("Press Any Key to continue...");
        Console.ReadKey();
    }
}

using Microsoft.Data.Sqlite;
using Dapper;
using silvermax.CodingTracker.Models;
using Spectre.Console;
using System.Globalization;

namespace silvermax.CodingTracker;

internal class CodingController
{
    private readonly DbConfig dbConfig = new();

    public void initDb()
    {
        
        using (var connection = new SqliteConnection(dbConfig.ConnectionString))
        {
            string createTableSql = @"
            CREATE TABLE IF NOT EXISTS coding_session (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                StartTime TEXT,
                EndTime TEXT,
                Duration TEXT
                )";

            connection.Execute(createTableSql);
        }
    }

    public void AddToDb(string date, string startTime, string endTime, string duration)
    {

        using (var connection = new SqliteConnection(dbConfig.ConnectionString))
        {
            var newCodingSession = new CodingSession
            {
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration
            };

            string insertSql = "INSERT INTO coding_session (Date, StartTime, EndTime, Duration) VALUES (@Date, @StartTime, @EndTime, @Duration);";

            connection.Execute(insertSql, newCodingSession);
        }
    }

    public void ShowSessions()
    {
        using (var connection = new SqliteConnection(dbConfig.ConnectionString))
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Date[/]");
            table.AddColumn("[yellow]Start Time[/]");
            table.AddColumn("[yellow]End Time[/]");
            table.AddColumn("[yellow]Duration (Hours)[/]");

            var sessions = connection.Query<CodingSession>("SELECT * FROM coding_session").ToList();

            foreach (var session in sessions)
            {
                table.AddRow(
                    session.Id.ToString(),
                    $"[purple]{session.Date}[/]",
                    $"[blue]{session.StartTime}[/]",
                    $"[blue]{session.EndTime}[/]",
                    $"[green]{session.Duration}[/]"
                    );
            }

            AnsiConsole.Write(table);
        }

        AnsiConsole.MarkupLine("Press Any Key to continue...");
        Console.ReadKey();
    }
}
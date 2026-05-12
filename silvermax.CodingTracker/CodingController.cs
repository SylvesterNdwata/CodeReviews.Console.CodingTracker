using Microsoft.Data.Sqlite;
using Dapper;
using silvermax.CodingTracker.Models;
using Spectre.Console;
using System.Globalization;

namespace silvermax.CodingTracker;

internal class CodingController
{
    private readonly DbConfig dbConfig = new();
    private readonly UserInput userInput = new();

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

            if (sessions.Count < 0)
            {
                AnsiConsole.MarkupLine("[red]No rows found in the database");
            }
            else
            {
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
        }

        AnsiConsole.MarkupLine("Press Any Key to continue...");
        Console.ReadKey();
    }

    public void UpdateSession()
    {
        ShowSessions();
        var recordId = GetRecordId("update");

        using (var connection = new SqliteConnection(dbConfig.ConnectionString))
        {

            bool exists = connection.ExecuteScalar<bool>(
                "SELECT COUNT(1) FROM coding_session WHERE Id = @Id", new { Id = recordId });

            while (!exists)
            {
                AnsiConsole.MarkupLine($"[red]Record {recordId} does not exist.[/]. Type 0 to go back to the menu...");
                if (Console.ReadLine() == "0")
                {
                    return;
                }
                UpdateSession();
            }

            var (date, startTime, endTime, duration) = userInput.GetUserInput();

            string sql = @"UPADTE coding_session
                            SET Date = @Date,
                                StartTime = @StartTime,
                                EndTime = @EndTime,
                                Duration = @Duration
                            WHERE Id = @Id";

            var sessionToUpdate = new CodingSession
            {
                Id = recordId,
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration
            };

            connection.Execute(sql, sessionToUpdate);
            
            AnsiConsole.MarkupLine($"[green]Record with Id {recordId} deleted successfully");
        }

        AnsiConsole.MarkupLine("Press Any Key to continue...");
        Console.ReadKey();
    }

    public void DeleteSession()
    {
        ShowSessions();
        var recordId = GetRecordId("delete");

        using (var connection = new SqliteConnection(dbConfig.ConnectionString))
        {
            bool exists = connection.ExecuteScalar<bool>(
                "SELECT COUNT(1) FROM coding_session WHERE Id = @Id", new { Id = recordId });

            while (!exists)
            {
                AnsiConsole.MarkupLine($"[red]Record {recordId} does not exist.[/]. Type 0 to go back to the menu...");
                if (Console.ReadLine() == "0")
                {
                    return;
                }
                DeleteSession();
            }

            string sql = "DELETE FROM coding_session WHERE Id = @Id";

            connection.Execute(sql, new { Id = recordId });
            
            AnsiConsole.MarkupLine($"[green]Record with Id {recordId} deleted successfully[/]");
        }

        AnsiConsole.MarkupLine("Press Any Key to continue...");
        Console.ReadKey();
    }

    private int GetRecordId(string action)
    {
        var recordId = 0;

        AnsiConsole.Prompt(
            new TextPrompt<string>($"Please enter the Id of the session you want to {action}: ")
            .Validate(input =>
            {
                if (!int.TryParse(input, out recordId))
                {
                    return ValidationResult.Error("Please enter a valid recordId");
                }

                return ValidationResult.Success();
            }));

        return recordId;
    }
}
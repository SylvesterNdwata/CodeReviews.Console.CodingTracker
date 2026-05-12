using Spectre.Console;
using System.Globalization;

namespace silvermax.CodingTracker;

internal class UserInput
{
    private readonly Validation validator = new();
    public (string date, string startTime, string endTime, string duration) GetUserInput()
    {
        string date = AnsiConsole.Prompt(
            new TextPrompt<string>("Please input the [green]date[/] (Format: dd-mm-yy): ")
            .Validate(input => validator.ValidateDate(input)
            ? ValidationResult.Success()
            : ValidationResult.Error("[red]Invalid date.[/] (Format: dd-MM-yy).")));

        DateTime parsedDate = DateTime.ParseExact(date, "dd-MM-yy", new CultureInfo("de-DE"));

        string startTime = AnsiConsole.Prompt(
            new TextPrompt<string>("Please input the [green]startTime[/] in HH:mm (24h format): ")
            .Validate(input => validator.ValidateTime(input)
                ? ValidationResult.Success()
                : ValidationResult.Error("[red]Invalid format![/]. Please use HH:mm (e.g., 13:00)")));

        DateTime start = DateTime.Parse(startTime);

        string endTime = AnsiConsole.Prompt(
            new TextPrompt<string>("Please input the [green]endTime[/] in HH:mm (24h format): ")
            .Validate(input =>
            {
                if (!validator.ValidateTime(input))
                {
                    return ValidationResult.Error("[red]Invalid format![/]. Please use HH:mm (e.g., 13:00)");
                }

                if (DateTime.Parse(input) <= start)
                {
                    return ValidationResult.Error("[red]End time must be after the start time![/]");
                }

                return ValidationResult.Success();
            }));

        DateTime end = DateTime.Parse(endTime);

        TimeSpan duration = end - start;

        AnsiConsole.MarkupLine($"Started: {startTime}, Ended: {endTime}, Duration: {duration.TotalHours:F2} hours");

        return (parsedDate.ToString("dd-MMM-yyyy"), startTime, endTime, $"{duration.TotalHours:F2}");
    }
}

using System.Globalization;

namespace silvermax.CodingTracker;

public class Validation
{
    public bool ValidateTime(string time)
    {
        return DateTime.TryParseExact(time, "HH:mm", new CultureInfo("de-DE"), DateTimeStyles.None, out _);
    }

    public bool ValidateDate(string dateInput)
    {
        return DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("de-DE"), DateTimeStyles.None, out _);
    }
}

namespace silvermax.CodingTracker;

internal class AddSession
{
    private readonly UserInput userInput = new();
    private readonly CodingController _controller = new();
    public void GetSession()
    {
        var(date, startTime, endTime, duration) = userInput.GetUserInput();

        _controller.AddToDb(date, startTime, endTime, duration);
    }
}

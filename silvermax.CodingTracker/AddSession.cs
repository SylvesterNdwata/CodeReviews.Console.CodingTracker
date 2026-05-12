namespace silvermax.CodingTracker;

internal class AddSession
{
    private readonly UserInput userInput = new();
    public void GetSession()
    {
        userInput.GetUserInput();
    }
}

using Spectre.Console;
using static silvermax.CodingTracker.Enums;

namespace silvermax.CodingTracker;

internal class UserInterface
{
    RecordSession recordSession = new RecordSession();
    AddSession addSession = new AddSession();
    private readonly CodingController _controller = new();
    public void Start()
    {
        _controller.initDb();

        bool openApp = true;
        while (openApp)
        {
            Console.Clear();
            var choice = AnsiConsole.Prompt(
        new SelectionPrompt<Menu>()
        .Title("Please choose the action you want to do.")
        .AddChoices(Enum.GetValues<Menu>()));

            switch (choice)
            {
                case Menu.StartSession:
                    recordSession.Start();
                    break;

                case Menu.AddSession:
                    addSession.GetSession();
                    break;

                case Menu.ViewSessions:
                    _controller.ShowSessions();
                    break;

                case Menu.DeleteSession:
                    _controller.DeleteSession();
                    break;

                case Menu.UpdateSession:
                    _controller.UpdateSession();
                    break;

                case Menu.CloseApp:
                    openApp = false;
                    break;
            }
        }
    }
}

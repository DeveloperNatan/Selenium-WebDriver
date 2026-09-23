using ConsoleApp1;

Console.WriteLine("=== Automations ===");
Console.WriteLine("1 - Youtube Music");
Console.WriteLine("2 - Youtube");

string? option = Console.ReadLine();

switch (option)
{
    case "1":
        YtMusicAutomation.Run();
        break;
    case "2":
        YtCloseChat.Run();
        break;
    case "0":
        Console.WriteLine("Bye.");
        break;
    default:
        Console.WriteLine("Invalid Option.");
        break;
}
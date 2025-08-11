using System.Text;
using BananaAIBot.Interfaces;
using BananaAIBot.Services;

namespace BananaAIBot;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        const string apiKey = "key";

        IAiService aiService = new Response(apiKey);

        var handler = new ConsoleInteractionHandler(aiService);

        await handler.RunAsync();
    }
}
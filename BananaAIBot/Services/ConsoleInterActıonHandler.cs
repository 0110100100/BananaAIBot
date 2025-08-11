using System.Text;
using BananaAIBot;
using BananaAIBot.Interfaces;
using BananaAIBot.Models;

public class ConsoleInteractionHandler
{
    private readonly IAiService _aiService;

    public ConsoleInteractionHandler(IAiService aiService)
    {
        _aiService = aiService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Вставь переписку (можно с метками 'Клиент:' и 'Менеджер:', а можно просто текст).");
        Console.WriteLine("После окончания ввода — введи пустую строку:\n");

        var sb = new StringBuilder();
        string line;
        while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            sb.AppendLine(line);

        Console.WriteLine("\nВведите данные об устройстве.");

        Console.Write("Модель телефона: ");
        var model = Console.ReadLine() ?? "";

        Console.Write("Производитель: ");
        var brand = Console.ReadLine() ?? "";

        var deviceInfo = new DeviceInfo(model, brand);

        var inputText = sb.ToString();

        bool hasLabels = inputText.Contains("Клиент:") && inputText.Contains("Менеджер:");

        if (hasLabels)
        {
            var conversationHistory = ConversationParser.ParseConversationWithLabels(inputText);
            var systemPrompt =
                "Ты — менеджер сервисного центра по ремонту телефонов.\nПроанализируй переписку, сформируй краткое саммари и вежливый ответ клиенту." +
                "\nОтвет должен быть строго в формате:\n\n=== Summary ===\n(здесь краткое саммари)\n\n=== Reply ===\n(здесь вежливый и профессиональный ответ)";

            var context = new ChatContext(systemPrompt, conversationHistory, deviceInfo);
            var result = await _aiService.ProcessDialogAsync(context);

            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n=== Саммари ===");
                Console.WriteLine(result.Value.Summary);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n=== Предложенный ответ клиенту ===");
                Console.WriteLine(result.Value.Reply);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
            }
        }
        else
        {
            var result = await _aiService.ProcessRawConversationAsync(inputText, deviceInfo);

            if (result.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n=== Саммари ===");
                Console.WriteLine(result.Value.Summary);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n=== Предложенный ответ клиенту ===");
                Console.WriteLine(result.Value.Reply);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
            }
        }
    }
}
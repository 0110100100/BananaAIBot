using BananaAIBot.Models;

namespace BananaAIBot;

public static class ConversationParser
{
    public static List<ChatMessage> ParseConversationWithLabels(string fullText)
    {
        var messages = new List<ChatMessage>();
        var lines = fullText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            string role = null;
            string content = line;

            if (line.StartsWith("Клиент:", StringComparison.OrdinalIgnoreCase))
            {
                role = "user";
                content = line.Substring("Клиент:".Length).Trim();
            }
            else if (line.StartsWith("Менеджер:", StringComparison.OrdinalIgnoreCase))
            {
                role = "assistant";
                content = line.Substring("Менеджер:".Length).Trim();
            }
            else
            {
                role = "user";
            }

            messages.Add(new ChatMessage(role, content, DateTime.Now));
        }

        return messages;
    }
}
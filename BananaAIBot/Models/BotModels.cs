namespace BananaAIBot.Models
{
    public record ChatMessage(string Role, string Content, DateTime Timestamp);

    public record DeviceInfo(string Model, string Manufacturer);

    public record ChatContext(string SystemPrompt, List<ChatMessage> ConversationHistory, DeviceInfo Device);

    public record BotResponse(string Summary, string Reply);

    public record OpenRouterRequest(string model, List<OpenRouterMessage> messages, int max_tokens);

    public record OpenRouterMessage(string role, string content);

    public record OpenRouterResponse(OpenRouterChoice[] choices);

    public record OpenRouterChoice(OpenRouterMessage message);
}
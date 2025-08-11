using CSharpFunctionalExtensions;
using BananaAIBot.Models;
using Refit;
using BananaAIBot.Interfaces;

namespace BananaAIBot.Services;

public class Response : IAiService
{
    private readonly IOpenRouterApi _api;
    private readonly string _bearer;

    public Response(string apiKey)
    {
        _bearer = $"Bearer {apiKey}";
        _api = RestService.For<IOpenRouterApi>(new HttpClient { BaseAddress = new Uri("https://openrouter.ai") });
    }

    public async Task<Result<BotResponse>> ProcessDialogAsync(ChatContext context)
    {
        return await Result.Try(async () =>
            {
                var messages = new List<OpenRouterMessage>
                {
                    new OpenRouterMessage("system", context.SystemPrompt + "\n\n" +
                                                    "На основе переписки сделай краткое саммари обращения и ниже предложи ответ клиенту, " +
                                                    "разделяя их так:\n\n" +
                                                    "=== Summary ===\n(текст саммари)\n\n=== Reply ===\n(текст ответа)")
                };

                foreach (var msg in context.ConversationHistory)
                {
                    if (!string.IsNullOrWhiteSpace(msg.Content))
                        messages.Add(new OpenRouterMessage(msg.Role, msg.Content.Trim()));
                }

                var deviceInfo = $"Устройство: {context.Device.Model}, Производитель: {context.Device.Manufacturer}";
                messages.Add(new OpenRouterMessage("user", deviceInfo));

                messages.Add(new OpenRouterMessage("user",
                    "Пожалуйста, сделай саммари и ответ, используя формат с разделителями === Summary === и === Reply ===."));

                var request = new OpenRouterRequest(
                    model: "deepseek/deepseek-r1:free",
                    messages: messages,
                    max_tokens: 400);

                var resp = await _api.CreateChatCompletion(request, _bearer);
                var text = resp.choices[0].message.content;

                const string summaryMarker = "=== Summary ===";
                const string replyMarker = "=== Reply ===";

                int summaryStart = text.IndexOf(summaryMarker);
                int replyStart = text.IndexOf(replyMarker);

                string summary = "";
                string reply = "";

                if (summaryStart >= 0 && replyStart > summaryStart)
                {
                    summary = text.Substring(summaryStart + summaryMarker.Length,
                        replyStart - (summaryStart + summaryMarker.Length)).Trim();
                    reply = text.Substring(replyStart + replyMarker.Length).Trim();
                }
                else
                {
                    reply = text.Trim();
                }

                return new BotResponse(summary, reply);
            })
            .MapError(errorMessage => $"Ошибка при вызове API: {errorMessage}");
    }

    public Task<Result<BotResponse>> ProcessRawConversationAsync(string fullConversationText, DeviceInfo deviceInfo) =>
    Result.Try(async () =>
    {
        var messages = new List<OpenRouterMessage>
        {
            new OpenRouterMessage("system",
                "Ты — менеджер сервисного центра по ремонту телефонов. " +
                "Ты получаешь переписку между клиентом и менеджером, но в тексте нет явных меток, кто говорит — клиент или менеджер. " +
                "Определи, кто говорит в каждом сообщении, раздели переписку по ролям, " +
                "сделай краткое саммари обращения и предложи вежливый, профессиональный ответ клиенту."),

            new OpenRouterMessage("user", fullConversationText),

            new OpenRouterMessage("user",
                $"Дополнительно информация об устройстве: модель — {deviceInfo.Model}, " +
                $"производитель — {deviceInfo.Manufacturer}"),

            new OpenRouterMessage("user",
                "Пожалуйста, выведи результат в формате:\n=== Summary ===\n(саммари)\n\n=== Reply ===\n(ответ клиенту)")
        };

        var request = new OpenRouterRequest(
            model: "deepseek/deepseek-r1:free",
            messages: messages,
            max_tokens: 800);

        var resp = await _api.CreateChatCompletion(request, _bearer);
        var text = resp.choices[0].message.content;

        const string summaryMarker = "=== Summary ===";
        const string replyMarker = "=== Reply ===";

        int summaryStart = text.IndexOf(summaryMarker);
        int replyStart = text.IndexOf(replyMarker);

        string summary = "";
        string reply = "";

        if (summaryStart >= 0 && replyStart > summaryStart)
        {
            summary = text.Substring(summaryStart + summaryMarker.Length,
                replyStart - (summaryStart + summaryMarker.Length)).Trim();
            reply = text.Substring(replyStart + replyMarker.Length).Trim();
        }
        else
        {
            reply = text.Trim();
        }

        return new BotResponse(summary, reply);
    })
    .MapError(errorMessage => $"Ошибка при вызове API: {errorMessage}");

}
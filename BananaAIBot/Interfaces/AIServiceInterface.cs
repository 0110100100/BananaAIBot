using CSharpFunctionalExtensions;
using BananaAIBot.Models;

namespace BananaAIBot.Interfaces;

public interface IAiService
{
    Task<Result<BotResponse>> ProcessDialogAsync(ChatContext context);
    Task<Result<BotResponse>> ProcessRawConversationAsync(string fullText, DeviceInfo device);
}
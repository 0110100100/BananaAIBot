using Refit;
using BananaAIBot.Models;

namespace BananaAIBot.Interfaces
{
    public interface IOpenRouterApi
    {
        [Post("/api/v1/chat/completions")]
        Task<OpenRouterResponse> CreateChatCompletion([Body] OpenRouterRequest request,
            [Header("Authorization")] string authHeader);
    }
}
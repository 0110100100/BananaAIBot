using BananaAIBot.Services;
using BananaAIBot.Models;
using FluentAssertions;

namespace BananaAIBot.Tests;

[TestFixture]
public class ResponseTests
{
    private const string ApiKey = "key";

    [Test]
    public async Task ProcessRawConversationAsync_ShouldReturnSummaryAndReply_ForDisplayIssue()
    {
        var aiService = new Response(ApiKey);

        var conversation = @"
Здравствуйте, у меня перестал работать дисплей на телефоне.
Добрый день! Какая у вас модель телефона и когда появилась эта проблема?
У меня Samsung Galaxy S9, проблема началась пару дней назад, экран просто не включается.
Понятно. Вы случайно не роняли телефон или не было попадания влаги?
Нет, телефон не падал и не попадал под дождь.
Спасибо за информацию. Мы можем провести диагностику, чтобы точно определить причину. Обычно замена дисплея стоит около 4500 рублей.
А сколько времени займет ремонт?
Обычно около 2-3 дней. Если хотите, можем записать вас на диагностику уже сегодня.
Да, записывайте, спасибо.
Отлично, ждем вас сегодня с 14 до 18 часов по адресу: ул. Ленина, 10.
Спасибо, до встречи.
Спасибо, до встречи!
";

        var deviceInfo = new DeviceInfo("Samsung Galaxy S9", "Samsung");

        var result = await aiService.ProcessRawConversationAsync(conversation, deviceInfo);

        result.IsSuccess.Should().BeTrue();
        result.Value.Summary.Should().NotBeNullOrWhiteSpace();
        result.Value.Reply.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ProcessRawConversationAsync_ShouldReturnSummaryAndReply_ForBatteryIssue()
    {
        var aiService = new Response(ApiKey);

        var conversation = @"
Добрый день, телефон быстро разряжается.
Здравствуйте! Какая модель устройства?
Это iPhone 11.
Понятно. Как давно началась проблема?
Примерно неделю назад.
Возможно, проблема в аккумуляторе. Мы можем провести диагностику и заменить батарею при необходимости.
Сколько будет стоить замена?
Около 3000 рублей.
Спасибо, хочу записаться на ремонт.
Хорошо, ждем вас завтра с 10 до 14 по адресу: ул. Пушкина, 15.
";

        var deviceInfo = new DeviceInfo("iPhone 11", "Apple");

        var result = await aiService.ProcessRawConversationAsync(conversation, deviceInfo);

        result.IsSuccess.Should().BeTrue();
        result.Value.Summary.Should().NotBeNullOrWhiteSpace();
        result.Value.Reply.Should().NotBeNullOrWhiteSpace();
    }
}

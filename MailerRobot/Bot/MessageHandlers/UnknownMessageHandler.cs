using IronSalesmanBot.Bot.Domain;
using IronSalesmanBot.Bot.MessageHandlers.Base;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;

namespace IronSalesmanBot.Bot.MessageHandlers;

[MessageHandler(HandlerName.Unknown)]
internal class UnknownMessageHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;

	public UnknownMessageHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(MessageData message)
	{
		await _botClient.SendAsync(message.From.ChatId,
			"Unknown command",
			replyMarkup: Keyboard.GetMainKeyboard());

		return default!;
	}
}
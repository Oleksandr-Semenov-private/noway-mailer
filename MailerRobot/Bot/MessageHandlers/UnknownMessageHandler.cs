using MailerRobot.Bot.Domain;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;

namespace MailerRobot.Bot.MessageHandlers;

[MessageHandler(HandlerName.Unknown)]
internal class UnknownMessageHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;

	public UnknownMessageHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		await _botClient.SendAsync(message.From.ChatId,
			"Unknown command",
			replyMarkup: Keyboard.GetMainKeyboard());
		
		return default!;
	}
}
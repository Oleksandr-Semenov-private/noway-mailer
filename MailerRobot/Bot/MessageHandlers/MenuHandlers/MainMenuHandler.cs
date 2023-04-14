using MailerRobot.Bot.Domain;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;

namespace MailerRobot.Bot.MessageHandlers;

[MessageHandler(HandlerName.MainMenu)]
internal class MainMenuHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;

	public MainMenuHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		if (message.HandlerInfo.Data?.Equals("Override:True") ?? false)
			await _botClient.OverridePreviousAsync(message.From.ChatId,
				"Вы попали в главное меню",
				replyMarkup: Keyboard.GetMainKeyboard());
		else
			await _botClient.SendAsync(message.From.ChatId,
				"Вы попали в главное меню",
				replyMarkup: Keyboard.GetMainKeyboard());

		return default!;
	}
}
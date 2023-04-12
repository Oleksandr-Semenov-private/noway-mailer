using IronSalesmanBot.Bot.Domain;
using IronSalesmanBot.Bot.MessageHandlers.Base;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;

namespace IronSalesmanBot.Bot.MessageHandlers;

[MessageHandler(HandlerName.MainMenu)]
internal class MainMenuHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;

	public MainMenuHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(MessageData message)
	{
		if (message.HandlerInfo.Data?.Equals("Override:True") ?? false)
			await _botClient.OverridePreviousAsync(message.From.ChatId,
				"Let's start!",
				replyMarkup: Keyboard.GetMainKeyboard());
		else
			await _botClient.SendAsync(message.From.ChatId,
				"Let's start!",
				replyMarkup: Keyboard.GetMainKeyboard());

		return default!;
	}
}
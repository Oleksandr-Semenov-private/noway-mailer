using IronSalesmanBot.Bot.MessageHandlers.Base;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using Telegram.Bot.Types;

namespace IronSalesmanBot.Bot.MessageHandlers;


[MessageHandler(HandlerName.BackButton)]
internal class BackButtonHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public BackButtonHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(MessageData message)
	{
		_message = message;
		
		await _botClient.BackButton(message.From.ChatId);

		return default!;
	}
}
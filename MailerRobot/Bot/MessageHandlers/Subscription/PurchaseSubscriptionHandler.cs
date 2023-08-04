using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Subscription;

[MessageHandler(HandlerName.PurchaseSubscription)]
internal class PurchaseSubscriptionHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public PurchaseSubscriptionHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		await _botClient.OverridePreviousAsync(message.From.ChatId,
			"Выбранная подписка: отсутствует" +
			"\n\n1 день 15$" +
			"\n\n3 дня 5$" +
			"\n\n7 дней 115$" +
			"\n\n30 дней 300$",
			replyMarkup: GetServicesKeyboard());

		return default!;
	}
	
	private InlineKeyboardMarkup GetServicesKeyboard()
	{
		return new InlineKeyboardMarkup(new[] { PurchaseSubscriptionFirstRaw(), GetBackButton() });
	}
	private static List<InlineKeyboardButton> PurchaseSubscriptionFirstRaw()
	{
		return new List<InlineKeyboardButton>
		{
			new("Cryptobot")
			{
				CallbackData = new HandlerInfo(HandlerName.SelectSubscription).Serialize()
			}
		};
	}
	private static List<InlineKeyboardButton> GetBackButton()
	{
		return new List<InlineKeyboardButton>
		{
			new("← Back")
			{
				CallbackData = new HandlerInfo(HandlerName.BackButton).Serialize()
			}
		};
	}
}
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Subscription;


[MessageHandler(HandlerName.SelectSubscription)]
internal class SelectSubscription : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public SelectSubscription(ITelegramBot botClient)
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
			new("1")
			{
				CallbackData = new HandlerInfo(HandlerName.SubscriptionFor1Day).Serialize()
			},
			new("3")
			{
				CallbackData = new HandlerInfo(HandlerName.SubscriptionFor3Days).Serialize()
			},
			new("7")
			{
				CallbackData = new HandlerInfo(HandlerName.SubscriptionFor7Days).Serialize()
			},
			new("30")
			{
				CallbackData = new HandlerInfo(HandlerName.SubscriptionFor30Days).Serialize()
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
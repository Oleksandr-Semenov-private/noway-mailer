using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Subscription.Pay;

[MessageHandler(HandlerName.SubscriptionFor1Day)]
internal class BuySubscriptionFor1Day : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public BuySubscriptionFor1Day(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		await _botClient.OverridePreviousAsync(message.From.ChatId,
			"К оплате 1$" +
			"\n\nВыберите валюту для оплаты:" ,
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
			new("USDT")
			{
				CallbackData = new HandlerInfo(HandlerName.Pay1Day).Serialize()
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
using CryptoPay;
using CryptoPay.Types;
using MailerRobot.Bot.Domain;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Subscription.Pay;

[MessageHandler(HandlerName.CheckPayment)]
internal class CheckPayment : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public CheckPayment(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;
		
		var subscription = await subscriber.Subscriptions.FirstOrDefault().GetRemainingDaysToStringAsync();
		
		await _botClient.SendAsync(message.From.ChatId,
			"👋 Вы попали в главное меню" +
			$"\n📍 Ваш id: {subscriber.Id}" +
			$"\n📝 Подписка: {subscription}",
			replyMarkup: Keyboard.GetMainKeyboard());
		
		return default!;
	}
	
	private InlineKeyboardMarkup GetServicesKeyboard(Invoice invoice)
	{
		return new InlineKeyboardMarkup(new[] { PurchaseSubscriptionFirstRaw(invoice), GetBackButton() });
	}
	private static List<InlineKeyboardButton> PurchaseSubscriptionFirstRaw(Invoice invoice)
	{
		return new List<InlineKeyboardButton>
		{
			new("Перейти к оплате")
			{
				Url = invoice.PayUrl
			},
			new("Проверить оплату")
			{
				CallbackData = new HandlerInfo(HandlerName.CheckPayment).Serialize()
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
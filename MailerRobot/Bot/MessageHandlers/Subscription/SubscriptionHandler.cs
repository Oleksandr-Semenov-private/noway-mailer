using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Subscription;

[MessageHandler(HandlerName.Subscription)]
internal class SubscriptionHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public SubscriptionHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		await _botClient.OverridePreviousAsync(message.From.ChatId,
			"Выбранная подписка: отсутствует",
			replyMarkup: GetServicesKeyboard());

		
		return default!;
	}
	
	private InlineKeyboardMarkup GetServicesKeyboard()
	{
		return new InlineKeyboardMarkup(new[] { PurchaseSubscription(), GetBackButton() });
	}
	private static List<InlineKeyboardButton> PurchaseSubscription()
	{
		return new List<InlineKeyboardButton>
		{
			new("Купить подписку")
			{
				CallbackData = new HandlerInfo(HandlerName.PurchaseSubscription).Serialize()
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
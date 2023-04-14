using MailerRobot.Bot.Domain;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.MailSender;

[MessageHandler(HandlerName.EmailEntered)]
internal class EmailEnteredHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;
	private ISubscriptionPersistence _subscriptionPersistence;

	public EmailEnteredHandler(ITelegramBot botClient, ISubscriptionPersistence subscriptionPersistence)
	{
		_botClient = botClient;
		_subscriptionPersistence = subscriptionPersistence;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		_subscriptionPersistence.AddEmail(subscriber, _message.HandlerInfo.Data);

		/*await _botClient.OverridePreviousAsync(message.From.ChatId,
			"Успешно отправлено",
			replyMarkup: GetServicesKeyboard());*/

		await _botClient.DeletePreviousAsync(message.From.ChatId, "Введите ссылку:");
		
		var text = "Отправлена ссылка:" + subscriber.SubscriberData.Link + " на email : " + subscriber.SubscriberData.Email;
		
		await _botClient.SendAsync(message.From.ChatId,
			text);
			
		await _botClient.SendAsync(message.From.ChatId,
			"Вы попали в главное меню",
			replyMarkup: Keyboard.GetMainKeyboard());

		subscriber.State = InputState.Idle;

		//subscriber.SubscriberData.Link = ;

		return default!;
	}
	
	private InlineKeyboardMarkup GetServicesKeyboard()
	{
		var firstButtons = GetFirstRawButtons();
		//var secondButtons = GetSecondRawButtons();

		return new InlineKeyboardMarkup(new[] {firstButtons, GetBackButton()});
	}
	
	private static List<InlineKeyboardButton> GetFirstRawButtons()
	{
		var getServicesButton = new InlineKeyboardButton("🇩🇪 Ebay")
		{
			CallbackData = new HandlerInfo(HandlerName.EbayDe).Serialize()
		};
		
		var buttons = new List<InlineKeyboardButton>();

		//if (role is Role.Admin)
			buttons.Add(getServicesButton);
		
		return buttons;
	}
	
	private static List<InlineKeyboardButton> GetBackButton()
	{
		return new List<InlineKeyboardButton>
		{
			new("< Back")
			{
				CallbackData = new HandlerInfo(HandlerName.BackButton).Serialize()
			}
		};
	}
}
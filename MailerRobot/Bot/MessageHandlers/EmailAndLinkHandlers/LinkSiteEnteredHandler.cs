using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.MailSender;

[MessageHandler(HandlerName.LinkEntered)]
internal class LinkSiteEnteredHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;
	ISubscriptionPersistence _subscriptionPersistence;
	
	public LinkSiteEnteredHandler(ITelegramBot botClient, ISubscriptionPersistence subscriptionPersistence)
	{
		_botClient = botClient;
		_subscriptionPersistence = subscriptionPersistence;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		await _botClient.DeletePreviousAsync(message.From.ChatId, "Введите ссылку:");
		
		await _botClient.SendAsync(message.From.ChatId,
			"Введите email:",
			replyMarkup: GetServicesKeyboard());

		_subscriptionPersistence.AddLink(subscriber, _message.HandlerInfo.Data);
		
		subscriber.State = InputState.WaitingForEmail;
		
		return default!;
	}
	
	private InlineKeyboardMarkup GetServicesKeyboard()
	{
		return new InlineKeyboardMarkup(new[] {GetBackButton()});
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
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
	private readonly ISubscriptionPersistence _subscriptionPersistence;

	public MainMenuHandler(ITelegramBot botClient, ISubscriptionPersistence subscriptionPersistence)
	{
		_botClient = botClient;
		_subscriptionPersistence = subscriptionPersistence;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		var subscription = subscriber.Subscriptions.FirstOrDefault();

		var q = "отсутствует";

		if (subscription != null)
			q = $"{subscription.EndDate - DateTime.Now}";
		
		
		if (message.HandlerInfo.Data?.Equals("Override:True") ?? false)
			await _botClient.OverridePreviousAsync(message.From.ChatId,
				"👋 Вы попали в главное меню" +
				$"\n📍 Ваш id: {subscriber.Id}" +
				$"\n📝 Подписка: {q}",
				replyMarkup: Keyboard.GetMainKeyboard());
		else
			await _botClient.SendAsync(message.From.ChatId,
				"👋 Вы попали в главное меню" +
				$"\n📍 Ваш id: {subscriber.Id}" +
				$"\n📝 Подписка: {q}",
				replyMarkup: Keyboard.GetMainKeyboard());

		return default!;
	}
}
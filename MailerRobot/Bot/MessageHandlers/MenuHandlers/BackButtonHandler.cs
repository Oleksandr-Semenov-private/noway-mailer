using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;

namespace MailerRobot.Bot.MessageHandlers;


[MessageHandler(HandlerName.BackButton)]
internal class BackButtonHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public BackButtonHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		var subscription = "отсутствует";

		if (subscriber.Subscriptions.Count > 0)
			subscription = await subscriber.Subscriptions.OrderDescending().FirstOrDefault().GetRemainingDaysToStringAsync();
		
		await _botClient.BackButton(message.From.ChatId, "👋 Вы попали в главное меню" +
														$"\n📍 Ваш id: {subscriber.Id}" +
														$"\n📝 Подписка: {subscription}"
		);

		subscriber.State = InputState.Idle;
		
		return default!;
	}
}
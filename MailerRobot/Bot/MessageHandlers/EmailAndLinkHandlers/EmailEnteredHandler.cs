using MailerRobot.Bot.Domain;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using MailerRobot.Bot.Services;
using MediatR;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.MailSender;

[MessageHandler(HandlerName.EmailEntered)]
internal class EmailEnteredHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private readonly ISubscriptionPersistence _subscriptionPersistence;
	private readonly IConfiguration _configuration;
	private readonly HttpClient _client;
	private MessageData _message = null!;
	private readonly IMediator _mediator;

	public EmailEnteredHandler(ITelegramBot botClient, ISubscriptionPersistence subscriptionPersistence,
		IConfiguration configuration, IMediator mediator)
	{
		_botClient = botClient;
		_subscriptionPersistence = subscriptionPersistence;
		_configuration = configuration;
		_mediator = mediator;
		_client = new HttpClient();
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		await _subscriptionPersistence.AddEmailAsync(subscriber, _message.HandlerInfo.Data);

		await _botClient.DeletePreviousAsync(message.From.ChatId, "Введите ссылку:");

		try
		{
			//await SendEmail(subscriber);
			//await SendEmailAPI(subscriber);

			var sendEmailNotification = new SendEmailApiNotification(subscriber.SubscriberData.Email,
				subscriber.SubscriberData.Link,
				subscriber.SubscriberData.Type);

			await _mediator.Publish(sendEmailNotification);

			await _botClient.SendAsync(message.From.ChatId,
				"✅ Письмо на email : " +
				subscriber.SubscriberData.Email + " было успешно отправлено.");
		}
		catch (Exception ex)
		{
			await _botClient.SendAsync(message.From.ChatId,
				"❌ Ошибка при отправке письма: " + ex.Message);
		}
		finally
		{
			var subscription = "отсутствует";

			if (subscriber.Subscriptions.Count > 0)
				subscription = await subscriber.Subscriptions.OrderDescending().FirstOrDefault().GetRemainingDaysToStringAsync();
			
			await _botClient.SendAsync(message.From.ChatId,
				"👋 Вы попали в главное меню" +
				$"\n📍 Ваш id: {subscriber.Id}" +
				$"\n📝 Подписка: {subscription}",
				replyMarkup: Keyboard.GetMainKeyboard());

			subscriber.State = InputState.Idle;
		}

		return default!;
	}

	private async Task SendEmailAPI(Subscriber subscriber)
	{
		using var client = new HttpClient();
		client.BaseAddress = new Uri("https://noway-mailer.herokuapp.com");

		var parameters = new Dictionary<string, string>
		{
			{"email", subscriber.SubscriberData.Email},
			{"link", subscriber.SubscriberData.Link}
		};

		var queryString = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;

		var url = "";

		if (subscriber.SubscriberData.Type == ServiceType.EbayDe)
			url = "https://noway-mailer.herokuapp.com/api/Sender/Ebay?" + queryString;
		else
			url = "https://noway-mailer.herokuapp.com/api/Sender/DHL?" + queryString;

		var response = await client.GetAsync(url);
		var responseContent = await response.Content.ReadAsStringAsync();
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
			new("← Back")
			{
				CallbackData = new HandlerInfo(HandlerName.BackButton).Serialize()
			}
		};
	}
}
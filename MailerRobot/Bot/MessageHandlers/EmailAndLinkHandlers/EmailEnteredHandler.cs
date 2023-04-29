using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using MailerRobot.Bot.Domain;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.Domain.Responses;
using MailerRobot.Bot.MessageHandlers.Base;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.MailSender;

[MessageHandler(HandlerName.EmailEntered)]
internal class EmailEnteredHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private readonly ISubscriptionPersistence _subscriptionPersistence;
	private readonly IConfiguration _configuration;
	private MessageData _message = null!;
	private readonly HttpClient _client;

	public EmailEnteredHandler(ITelegramBot botClient, ISubscriptionPersistence subscriptionPersistence,
		IConfiguration configuration)
	{
		_botClient = botClient;
		_subscriptionPersistence = subscriptionPersistence;
		_configuration = configuration;
		_client = new HttpClient();
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		_subscriptionPersistence.AddEmail(subscriber, _message.HandlerInfo.Data);

		await _botClient.DeletePreviousAsync(message.From.ChatId, "Введите ссылку:");

		try
		{
			await SendEmail(subscriber);

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
			await _botClient.SendAsync(message.From.ChatId,
				"Вы попали в главное меню",
				replyMarkup: Keyboard.GetMainKeyboard());

			subscriber.State = InputState.Idle;
		}

		return default!;
	}

	private async Task SendEmail(Subscriber subscriber)
	{
		
		var smtpServer = "pop4.ocnk.net";
		var smtpPort = 587;
		var smtpUsername = "support@toratorashop.com";
		var smtpPassword = "toratora";

		var senderEmail = "support@toratorashop.com";
		var recipientEmail = subscriber.SubscriberData.Email;
		var subject = "";

		if (subscriber.SubscriberData.Type == ServiceType.DHL)
			subject = "Unser Team hat einen Kunden für Sie gefunden #234422847";

		if (subscriber.SubscriberData.Type == ServiceType.EbayDe)
			subject = "Nutzer-Anfrage zu deiner Anzeige!";
		
		//var subject = "Nutzer-Anfrage zu deiner Anzeige!";
		var body = await GetBody(subscriber);

		var smtpClient = new SmtpClient(smtpServer, smtpPort);
		smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
		smtpClient.EnableSsl = true;

		var message = new MailMessage(senderEmail, recipientEmail, subject, body);
		message.IsBodyHtml = true;

		//message.From = new MailAddress(senderEmail, "EBAY_Kleinanzeigen");

		if (subscriber.SubscriberData.Type == ServiceType.DHL)
		{
			
			message.From = new MailAddress(senderEmail, "•DHL•Lieferungen");
		}
		else
		{
			message.From = new MailAddress(senderEmail, "EBAY_Kleinanzeigen");
		}
			
		
		smtpClient.Send(message);
	}

	private async Task<string> GetBody(Subscriber subscriber)
	{
		var link = await GetShortLink(subscriber.SubscriberData.Link);

		var path = $@"C:\\Users\\Admin\\RiderProjects\\MailerRobot\\MailerRobot\\Bot\\Templates\\{subscriber.SubscriberData.Type}.html";
		
		var hmtlBody = File.ReadAllText(path)
							.Replace("PutYourLinkHere", link);

		return hmtlBody;
	}

	private async Task<string> GetShortLink(string link)
	{
		_client.BaseAddress = new Uri("https://n9.cl/");

		var content = JsonContent.Create(new
		{
			url = link
		});

		var msg = await _client.PostAsync("api/short", content);

		var response = await ReadResponseAsync<N9Response>(msg);

		return response.Short;
	}

	private static async Task<TResponse> ReadResponseAsync<TResponse>(HttpResponseMessage msg,
		CancellationToken ct = default) where TResponse : class
	{
		var response = await msg.Content.ReadAsStringAsync(ct);

		return IsJson(response) ? response.Deserialize<TResponse>() : default!;
	}

	private static bool IsJson(string response)
	{
		return (response.StartsWith("{") && response.TrimEnd().EndsWith("}")) ||
				(response.StartsWith("[") && response.TrimEnd().EndsWith("]"));
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
			new("← Back")
			{
				CallbackData = new HandlerInfo(HandlerName.BackButton).Serialize()
			}
		};
	}
}
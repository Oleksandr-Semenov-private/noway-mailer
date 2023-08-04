using CryptoPay;
using CryptoPay.Types;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Subscription.Pay;

[MessageHandler(HandlerName.Pay1Day)]
internal class Pay1Day : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public Pay1Day(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;
		
		var cryptoPayClient = new CryptoPayClient("112943:AA0X3i5yXTduM2hyLs5f1DXcjphHssKRXpY");
		var application = await cryptoPayClient.GetMeAsync();
		
		var invoice = await cryptoPayClient.CreateInvoiceAsync(
			Assets.USDT,
			0.1,
			description: "test");

		var q = invoice.Status;
		
		InlineKeyboardMarkup inlineKeyboard = new(new[]
		{
			// first row
			new[]
			{
				InlineKeyboardButton.WithCallbackData(text: "Кнопка 1", callbackData: "post"),
				InlineKeyboardButton.WithCallbackData(text: "Кнопка 2", callbackData: "12"),
			},
 
		});

		await _botClient.OverridePreviousAsync(message.From.ChatId,
			$"К оплате {invoice.Amount} USDT" +
			"\n\nДля оплаты перейдите по ссылке:",
			replyMarkup: GetServicesKeyboard(invoice));
		
		return default!;
	}
	
	private InlineKeyboardMarkup GetServicesKeyboard(Invoice invoice)
	{
		return new InlineKeyboardMarkup(new[] { PurchaseSubscriptionFirstRaw(invoice), GetBackButton() });
	}
	private static List<InlineKeyboardButton> PurchaseSubscriptionFirstRaw(Invoice invoice)
	{
		//var inlineKeyboardButton = InlineKeyboardButton.WithCallbackData("Проверить оплату", invoice.Status.Serialize());

		return new List<InlineKeyboardButton>
		{
			new("Перейти к оплате")
			{
				Url = invoice.PayUrl
			},
			new("Проверить оплату")
			{
				CallbackData = new HandlerInfo(HandlerName.CheckPayment, invoice.InvoiceId.Serialize()).Serialize()
			}
		};
	}

	private static string CheckPayment(Invoice invoice)
	{
		return invoice.Status == Statuses.paid ? "Оплачено" : "Не оплачено";
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
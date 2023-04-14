using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.MailSender;

[MessageHandler(HandlerName.ChooseService)]
internal class ChooseServiceHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public ChooseServiceHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		await _botClient.OverridePreviousAsync(message.From.ChatId,
			"Выберите сервис:",
			replyMarkup: GetServicesKeyboard());

		subscriber.State = InputState.WaitingForLinkOnSite;

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
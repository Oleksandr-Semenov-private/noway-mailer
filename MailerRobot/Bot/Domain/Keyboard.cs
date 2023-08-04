using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.Domain;

public static class Keyboard
{
	public static InlineKeyboardMarkup GetMainKeyboard()
	{
		var buttons = new List<InlineKeyboardButton>
		{
			new("📩 Отправить письмо") {CallbackData = new HandlerInfo(HandlerName.ChooseService).Serialize()},
			//new("Bot") {CallbackData = new HandlerInfo(HandlerName.Config).Serialize()}
		};	
		
		var secondRawButtons = new List<InlineKeyboardButton>
		{
			new("📔 Подписки") {CallbackData = new HandlerInfo(HandlerName.Subscription).Serialize()},
			new("❓ Поддержка") {CallbackData = new HandlerInfo(HandlerName.Support).Serialize()},
			//new("Bot") {CallbackData = new HandlerInfo(HandlerName.Config).Serialize()}
		};
		

		return new InlineKeyboardMarkup(new []{buttons, secondRawButtons});
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
	
	private static List<InlineKeyboardButton> GetSecondRawButtons()
	{
		var getServicesButton = new InlineKeyboardButton("🇩🇪 DHL")
		{
			CallbackData = new HandlerInfo(HandlerName.DHL).Serialize()
		};
		
		var buttons = new List<InlineKeyboardButton>();

		//if (role is Role.Admin)
		buttons.Add(getServicesButton);
		
		return buttons;
	}
}
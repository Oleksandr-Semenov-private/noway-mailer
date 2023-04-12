using IronSalesmanBot.Bot.MessageHandlers.Base;
using MailerRobot.Bot;
using MailerRobot.Bot.Domain.MessageModels;
using Telegram.Bot.Types.ReplyMarkups;

namespace IronSalesmanBot.Bot.Domain;

public static class Keyboard
{
	public static InlineKeyboardMarkup GetMainKeyboard()
	{
		var buttons = new List<InlineKeyboardButton>
		{
			new("📩 Отправить письмо") {CallbackData = new HandlerInfo(HandlerName.MailSender).Serialize()},
			new("Bot") {CallbackData = new HandlerInfo(HandlerName.Config).Serialize()}
		};

		return new InlineKeyboardMarkup(buttons);
	}
}
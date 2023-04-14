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
			new("Bot") {CallbackData = new HandlerInfo(HandlerName.Config).Serialize()}
		};

		return new InlineKeyboardMarkup(buttons);
	}
}
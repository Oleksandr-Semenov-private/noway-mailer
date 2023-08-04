using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Support;

[MessageHandler(HandlerName.Support)]
internal class SupportHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public SupportHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;

		await _botClient.OverridePreviousAsync(message.From.ChatId,
			"Связь: @thatwayyouknow",
			replyMarkup: GetServicesKeyboard());

		subscriber.State = InputState.WaitingForLinkOnSite;

		return default!;
	}
	
	private InlineKeyboardMarkup GetServicesKeyboard()
	{
		return new InlineKeyboardMarkup(new[] { GetBackButton() });
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
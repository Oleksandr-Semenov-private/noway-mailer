using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.MessageHandlers.Services.De;


[MessageHandler(HandlerName.EbayDe)]
internal class EbayDeHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private MessageData _message = null!;

	public EbayDeHandler(ITelegramBot botClient)
	{
		_botClient = botClient;
	}

	protected override async Task<string> GetAnswer(Subscriber subscriber, MessageData message)
	{
		_message = message;
		
		await _botClient.OverridePreviousAsync(message.From.ChatId,
			"Введите ссылку:",
			replyMarkup: GetEbayDeKeyboard());

		subscriber.State = InputState.WaitingForLinkOnSite;
		
		subscriber.SubscriberData.Type = ServiceType.EbayDe;
		
		return default!;
	}
	
	private InlineKeyboardMarkup GetEbayDeKeyboard()
	{
		return new InlineKeyboardMarkup(new[] {GetBackButton()});
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
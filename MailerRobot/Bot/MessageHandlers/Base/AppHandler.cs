using IronSalesmanBot.Bot.Domain.Data;
using MailerRobot.Bot;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using Telegram.Bot.Types.ReplyMarkups;

namespace IronSalesmanBot.Bot.MessageHandlers.Base;

internal abstract class AppHandler : MessageHandler
{
	private readonly ITelegramBot _botClient;
	private readonly IConfiguration _configuration;

	protected AppHandler(ITelegramBot botClient, IConfiguration configuration)
	{
		_botClient = botClient;
		_configuration = configuration;
	}

	protected override async Task<string> GetAnswer(MessageData message)
	{
		await _botClient.OverridePreviousAsync(message.From.ChatId, "Choose an application",
			replyMarkup: GetAppKeyboard());

		return default!;
	}

	protected abstract HandlerName GetNextHandlerName();

	protected abstract HandlerName GetPreviousHandlerName();

	private InlineKeyboardMarkup GetAppKeyboard()
	{
		var buttons = Enum.GetValues<AppName>()
						.Select(e => e.ToString())
						.Select(e => new InlineKeyboardButton(e)
						{
							CallbackData = new HandlerInfo(GetNextHandlerName(), Data: e).Serialize()
						});

		return new InlineKeyboardMarkup(new[] {buttons, GetBackButton()});
	}

	private IEnumerable<InlineKeyboardButton> GetBackButton()
	{
		return new List<InlineKeyboardButton>
		{
			new("< Back") {CallbackData = new HandlerInfo(GetPreviousHandlerName()).Serialize()}
		};
	}
}
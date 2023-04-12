using MailerRobot.Bot.Domain.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace MailerRobot.Bot;

public class AppBackGround : BackgroundService
{
	private readonly ITelegramBot _bot;
	private readonly IHandlers _handlers;

	public AppBackGround(ITelegramBot bot, IHandlers handlers)
	{
		_bot = bot;
		_handlers = handlers;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_bot.StartReceiving(new DefaultUpdateHandler(_handlers.HandleUpdateAsync, _handlers.HandleError));
		//await botClient.SendTextMessageAsync("CHAT_ID", "Бот был успешно запущен.");

		//_bot.SendAsync(5908829889, "Бот был успешно запущен.");
	}
}


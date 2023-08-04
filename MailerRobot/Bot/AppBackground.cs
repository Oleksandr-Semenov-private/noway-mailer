using MailerRobot.Bot.Domain.Interfaces;
using Telegram.Bot.Polling;

namespace MailerRobot.Bot;

public class AppBackGround : BackgroundService
{
	private readonly ITelegramBot _bot;
	private readonly IHandlers _handlers;
	private readonly IConfiguration _configuration;
	private readonly ISubscriptionPersistence _subscriptionPersistence;

	public AppBackGround(ITelegramBot bot, IHandlers handlers, IConfiguration configuration, ISubscriptionPersistence subscriptionPersistence)
	{
		_bot = bot;
		_handlers = handlers;
		_configuration = configuration;
		_subscriptionPersistence = subscriptionPersistence;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_bot.StartReceiving(new DefaultUpdateHandler(_handlers.HandleUpdateAsync, _handlers.HandleError));
		
		var subscribers = _subscriptionPersistence.GetSubscribers();
		
		//await botClient.SendTextMessageAsync("CHAT_ID", "Бот был успешно запущен.");
	}
}


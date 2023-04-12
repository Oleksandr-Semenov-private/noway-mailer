using Telegram.Bot;
using Telegram.Bot.Types;

namespace MailerRobot.Bot.Domain.Interfaces;

public interface IHandlers
{
	Task HandleError(ITelegramBotClient botClient, Exception exception,
		CancellationToken cancellationToken);

	Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
		CancellationToken cancellationToken);
}
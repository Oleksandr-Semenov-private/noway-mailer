using MailerRobot.Bot.Domain.MessageModels;

namespace IronSalesmanBot.Bot.MessageHandlers.Base;

internal abstract class MessageHandler
{
	//TODO: Verification
	public async Task<string> GetAnswerAsync(MessageData message)
	{
		var user = message.From;

		return await GetAnswer(message);
	}

	protected virtual Task<string> GetAnswerForUnregistered(MessageData message)
	{
		return Task.FromResult("You are not registered.");
	}

	protected abstract Task<string> GetAnswer(MessageData message);
}
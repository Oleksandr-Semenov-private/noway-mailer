using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;

namespace MailerRobot.Bot.MessageHandlers.Base;

internal abstract class MessageHandler
{
	
	public async Task<string> GetAnswerAsync(Subscriber subscriber,MessageData message)
	{
		var user = message.From;

		return await GetAnswer(subscriber, message);
	}

	protected virtual Task<string> GetAnswerForUnregistered(MessageData message)
	{
		return Task.FromResult("You are not registered.");
	}

	protected abstract Task<string> GetAnswer(Subscriber subscriber, MessageData message);
}
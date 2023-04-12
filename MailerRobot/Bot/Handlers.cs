using System.Text.RegularExpressions;
using Autofac;
using IronSalesmanBot.Bot.MessageHandlers.Base;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = MailerRobot.Bot.Domain.Models.User;

namespace MailerRobot.Bot;

public class Handlers : IHandlers
{
	private readonly ITelegramBot _bot;
	private readonly ILifetimeScope _scope;

	public Handlers(ITelegramBot bot, ILifetimeScope scope)
	{
		_bot = bot;
		_scope = scope;
	}

	public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
	{
		var errorMessage = exception switch
		{
			ApiRequestException apiRequestException =>
				$"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
			_ => exception.Message
		};

		if (exception.InnerException is not null)
			errorMessage += $"\r\n{exception.InnerException.Message}";

		Console.WriteLine(errorMessage);

		return Task.CompletedTask;
	}

	public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
		CancellationToken cancellationToken)
	{
		try
		{
			var messageData = update.Type switch
			{
				UpdateType.Message => await MapMessageData(update.Message!),
				UpdateType.EditedMessage => await MapMessageData(update.EditedMessage!),
				UpdateType.CallbackQuery => await MapMessageData(update.CallbackQuery!),
				_ => null!
			};

			await HandleMessageAsync(messageData);
		}
		catch (Exception exception)
		{
			//_logger.Error(exception, "Error handling update");
			Console.WriteLine(exception.Message);
			await HandleError(botClient, exception, cancellationToken);
		}
	}

	private async Task HandleMessageAsync(MessageData message)
	{
		await using var scope = _scope.BeginLifetimeScope();

		var handler = scope.ResolveKeyed<MessageHandler>(message.HandlerInfo.HandlerName);

		var answer = await handler.GetAnswerAsync(message);

		if (string.IsNullOrEmpty(answer) is false)
			await _bot.SendAsync(message.From.ChatId, answer);
	}

	private async Task<MessageData> MapMessageData(Message message)
	{
		await _bot.SaveAsync(message);
		
		return new MessageData(message.MessageId,
			DeserializeMessage(message),
			message.From?.LanguageCode!,
			MapUser(message.From!));
	}

	private static User MapUser(Telegram.Bot.Types.User user)
	{
		return new User
		{
			ChatId = user.Id,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Username = user.Username
		};
	}
	
	private async Task<MessageData> MapMessageData(CallbackQuery query)
	{
		return new MessageData(query.Message!.MessageId,
			query.Data!.Deserialize<HandlerInfo>(),
			query.From.LanguageCode!,
			new User() {ChatId = query.From.Id});
	}

	private static HandlerInfo DeserializeMessage(Message message)
	{
		if (Regex.IsMatch(message.Text!, @"^\w{32}$"))
			return new HandlerInfo(HandlerName.Verification, Data: message.Text!);

		if (message.Text! == "/run")
			return new HandlerInfo(HandlerName.MainMenu);

		return message.Text!.Deserialize<HandlerInfo>();
	}
}
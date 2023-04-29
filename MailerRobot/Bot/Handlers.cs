using System.Text.RegularExpressions;
using Autofac;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.MessageModels;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.MessageHandlers.Base;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = MailerRobot.Bot.Domain.Models.User;

namespace MailerRobot.Bot;

public class Handlers : IHandlers
{
	private readonly ITelegramBot _bot;
	private readonly ILifetimeScope _scope;
	private readonly ISubscriptionPersistence _subscriptionPersistence;
	private static ITelegramBot _botClient;

	public Handlers(ITelegramBot bot, ILifetimeScope scope, ISubscriptionPersistence subscriptionPersistence)
	{
		_bot = bot;
		_botClient = bot;
		_scope = scope;
		_subscriptionPersistence = subscriptionPersistence;
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
		Subscriber subscriber = null;
		
		try
		{
			var subscribers = _subscriptionPersistence.GetSubscribers();
			long clientId = 0;

			try
			{
				clientId = update.Message.From.Id;
			}
			catch (Exception e)
			{
				clientId = update.CallbackQuery.From.Id;
			}
				
			subscriber = subscribers.SingleOrDefault(s => s.Id == clientId.ToString());
		
			if (subscriber is null)
			{
				subscriber = new Subscriber { Id = clientId.ToString() };
				_subscriptionPersistence.AddSubscriber(subscriber);
			}
			
			var messageData = update.Type switch
			{
				UpdateType.Message => await MapMessageData(subscriber,update.Message!),
				UpdateType.EditedMessage => await MapMessageData(subscriber,update.EditedMessage!),
				UpdateType.CallbackQuery => await MapMessageData(update.CallbackQuery!),
				_ => null!
			};
			
			await HandleMessageAsync(subscriber, messageData);
		}
		catch (Exception exception)
		{
			//_logger.Error(exception, "Error handling update");
			Console.WriteLine(exception.Message);
			await HandleError(botClient, exception, cancellationToken);
		}
	}

	private async Task HandleMessageAsync(Subscriber subscriber, MessageData message)
	{
		await using var scope = _scope.BeginLifetimeScope();

		var handler = scope.ResolveKeyed<MessageHandler>(message.HandlerInfo.HandlerName);
		
		var answer = await handler.GetAnswerAsync(subscriber, message);

		if (string.IsNullOrEmpty(answer) is false)
			await _bot.SendAsync(message.From.ChatId, answer);
	}

	private async Task<MessageData> MapMessageData(Subscriber subscriber, Message message)
	{
		await _bot.SaveAsync(message);
		
		return new MessageData(message.MessageId,
			DeserializeMessage(subscriber,message),
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

	private static HandlerInfo DeserializeMessage(Subscriber subscriber, Message message)
	{
		if (subscriber.State == InputState.WaitingForLinkOnSite)
		{
			if (Regex.IsMatch(message.Text!, @"^(http|https)://"))
				return new HandlerInfo(HandlerName.LinkEntered, Data: message.Text!);

			WrongLink(message);
		}
		
		if (subscriber.State == InputState.WaitingForEmail)
		{
			if (Regex.IsMatch(message.Text!, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
				return new HandlerInfo(HandlerName.EmailEntered, Data: message.Text!);

			WrongEmail(message);
		}
		
		
		if (Regex.IsMatch(message.Text!, @"^\w{32}$"))
			return new HandlerInfo(HandlerName.Verification, Data: message.Text!);

		if (message.Text! == "/run")
			return new HandlerInfo(HandlerName.MainMenu);

		return message.Text!.Deserialize<HandlerInfo>();
	}

	private static async Task WrongLink(Message message)
	{
		await _botClient.DeletePreviousAsync(message.From.Id, "Введите email:");

		await _botClient.SendAsync(message.From.Id,
			"Введите ссылку:",
			replyMarkup: GetServicesKeyboard());
	}

	private static async Task WrongEmail(Message message)
	{
		await _botClient.DeletePreviousAsync(message.From.Id, "Введите email:");

		await _botClient.SendAsync(message.From.Id,
			"Введите email:",
			replyMarkup: GetServicesKeyboard());
	}
	
	private static InlineKeyboardMarkup GetServicesKeyboard()
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
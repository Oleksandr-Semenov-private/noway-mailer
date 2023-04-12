using MailerRobot.Bot.Domain.Interfaces;
using Autofac;
using IronSalesmanBot.Bot.Domain;
using IronSalesmanBot.Bot.Domain.Data.Models;
using MailerRobot.Bot.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.Domain.Wrappers;

public class TelegramBot : ITelegramBot
{
	private readonly ITelegramBotClient _botClient;
	private ILifetimeScope _scope;

	public TelegramBot(IConfiguration configuration, ILifetimeScope scope)
	{
		_scope = scope;
		_botClient = new TelegramBotClient(configuration.GetSection("Token").Value);
	}

	public void StartReceiving(DefaultUpdateHandler defaultUpdateHandler)
	{
		_botClient.StartReceiving(defaultUpdateHandler);
	}
	
	public async Task SendAsync(
		long chatId,
		string text,
		ParseMode parseMode = ParseMode.Markdown,
		int? replyToMessageId = default,
		InlineKeyboardMarkup? replyMarkup = default,
		CancellationToken cancellationToken = default)
	{
		var msgEntity = await TrySendMessageAsync(chatId, text, parseMode, replyToMessageId, replyMarkup,
			cancellationToken);

		await using var scope = _scope.BeginLifetimeScope();
		var appContext = scope.Resolve<ApplicationContext>();

		appContext.Messages.Add(msgEntity);
		await appContext.SaveChangesAsync(cancellationToken);
	}
	
	public async Task SaveAsync(Message message)
	{
		var msg = new MessageChatInfo
		{
			ChatId = message.Chat.Id,
			MessageId = message.MessageId,
			Text = message.Text,
			Date = DateTime.Now,
			IsDeleted = false,
			Type = MessageChatType.Received,
			Keyboard = message.ReplyMarkup
		};

		await using var scope = _scope.BeginLifetimeScope();
		var appContext = scope.Resolve<ApplicationContext>();

		await appContext.Messages.AddAsync(msg);
		await appContext.SaveChangesAsync();
	}
	public async Task OverridePreviousAsync(
		long chatId,
		string? text,
		ParseMode parseMode = ParseMode.Markdown,
		InlineKeyboardMarkup? replyMarkup = default,
		CancellationToken cancellationToken = default)
	{
		await using var scope = _scope.BeginLifetimeScope();
		var appContext = scope.Resolve<ApplicationContext>();

		var previousMsg = await GetPreviousMessageAsync(chatId, appContext, cancellationToken);

		if (previousMsg is null)
			return;

		var message = await _botClient.EditMessageTextAsync(chatId, previousMsg.MessageId,
			text ?? previousMsg.Text!, parseMode, replyMarkup: replyMarkup, cancellationToken: cancellationToken);

		var msgEntity = previousMsg with
		{
			Id = 0,
			Text = message.Text,
			Date = DateTime.Now
		};

		previousMsg.IsDeleted = true;

		appContext.Messages.Add(msgEntity);
		await appContext.SaveChangesAsync(cancellationToken);
	}
	
	public async Task BackButton(long chatId, ParseMode parseMode = ParseMode.Markdown, InlineKeyboardMarkup? replyMarkup = default, CancellationToken cancellationToken = default)
	{
		await using var scope = _scope.BeginLifetimeScope();
		var appContext = scope.Resolve<ApplicationContext>();

		var previousMsg = await GetPreviousMessageAsync(chatId, appContext, cancellationToken);
		
		if (previousMsg is null)
			return;
		
		var message = await _botClient.EditMessageTextAsync(chatId, previousMsg.MessageId,
			previousMsg.Text!, parseMode, replyMarkup: Keyboard.GetMainKeyboard(), cancellationToken: cancellationToken);

		var msgEntity = previousMsg with
		{
			Id = 0,
			Text = message.Text,
			Date = DateTime.Now
		};

		previousMsg.IsDeleted = true;

		appContext.Messages.Add(msgEntity);
		await appContext.SaveChangesAsync(cancellationToken);
	}
	
	private async Task<MessageChatInfo> TrySendMessageAsync(long chatId,
		string text,
		ParseMode parseMode, int? replyToMessageId, InlineKeyboardMarkup? replyMarkup,
		CancellationToken cancellationToken)
	{
		try
		{
			return await SendMessageAsync(chatId, text, parseMode, replyToMessageId, replyMarkup,
				cancellationToken);
		}
		catch (Exception e)
		{
			Console.WriteLine("Error while sending message");

			return new MessageChatInfo
			{
				ChatId = chatId,
				MessageId = 0,
				Text = text,
				Date = DateTime.Now,
				IsDeleted = true,
				Type = MessageChatType.Error,
				Keyboard = replyMarkup
			};
		}
	}
	
	private async Task<MessageChatInfo> SendMessageAsync(long chatId, string text,
		ParseMode parseMode,
		int? replyToMessageId, InlineKeyboardMarkup? replyMarkup, CancellationToken cancellationToken)
	{
		var msg = await _botClient.SendTextMessageAsync(chatId, text, parseMode, replyToMessageId: replyToMessageId,
			replyMarkup: replyMarkup, cancellationToken: cancellationToken);

		return new MessageChatInfo
		{
			ChatId = msg.Chat.Id,
			MessageId = msg.MessageId,
			Text = msg.Text,
			Date = DateTime.Now,
			IsDeleted = false,
			Type = MessageChatType.Sent,
			Keyboard = replyMarkup
		};
	}
	
	private static async Task<MessageChatInfo?> GetPreviousMessageAsync(long chatId,
		ApplicationContext context,
		CancellationToken cancellationToken)
	{
		return await context
					.Messages
					.Where(e => e.ChatId == chatId)
					.Where(e => e.IsDeleted == false)
					.Where(e => e.Type == MessageChatType.Sent)
					.OrderBy(e => e.Date)
					.LastOrDefaultAsync(cancellationToken);
	}
	
}
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.Domain.Interfaces;

public interface ITelegramBot
{
	void StartReceiving(DefaultUpdateHandler defaultUpdateHandler);
	Task SaveAsync(Message message);
	
	Task SendAsync(
		long chatId,
		string text,
		ParseMode parseMode = ParseMode.Markdown,
		int? replyToMessageId = default,
		InlineKeyboardMarkup? replyMarkup = default,
		CancellationToken cancellationToken = default);
	
	Task OverridePreviousAsync(
		long chatId,
		string? text,
		ParseMode parseMode = ParseMode.Markdown,
		InlineKeyboardMarkup? replyMarkup = default,
		CancellationToken cancellationToken = default);

	Task DeletePreviousAsync(
		long chatId,
		string? text,
		ParseMode parseMode = ParseMode.Markdown,
		InlineKeyboardMarkup? replyMarkup = default,
		CancellationToken cancellationToken = default);
	
	
	Task BackButton(long chatId,string text, ParseMode parseMode = ParseMode.Markdown, InlineKeyboardMarkup? replyMarkup = default, CancellationToken cancellationToken = default);
}
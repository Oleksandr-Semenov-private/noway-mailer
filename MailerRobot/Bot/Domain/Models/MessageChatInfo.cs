using Telegram.Bot.Types.ReplyMarkups;

namespace MailerRobot.Bot.Domain.Models;

public record MessageChatInfo
{
	public long Id { get; set; }
	public long ChatId { get; set; }
	public int MessageId { get; set; }
	public string? Text { get; set; }
	public DateTime Date { get; set; }
	public bool IsDeleted { get; set; }
	public MessageChatType Type { get; set; }
	public InlineKeyboardMarkup? Keyboard { get; set; }
}
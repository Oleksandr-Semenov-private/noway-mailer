namespace MailerRobot.Bot.Domain.Models;

public record UserConfig
{
	public long ChatId { get; set; }
	public bool RetrieveExceptions { get; set; }

	public User User { get; set; } = null!;
}
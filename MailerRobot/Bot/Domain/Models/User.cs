namespace MailerRobot.Bot.Domain.Models;

public record User
{
	public long ChatId { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? Username { get; set; }
	public Role Role { get; set; }
	
	public UserConfig Configs { get; set; } = null!;
}
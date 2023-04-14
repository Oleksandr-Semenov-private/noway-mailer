using MailerRobot.Bot.Domain.Data.Configurations;
using MailerRobot.Bot.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MailerRobot.Bot.Domain.Data;

public class ApplicationContext : DbContext
{
	private IConfiguration _configuration;

	public ApplicationContext(IConfiguration configuration)
	{
		_configuration = configuration;
		Database.EnsureCreated();
	}

	public DbSet<MessageChatInfo> Messages { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new MessageChatInfoConfiguration());
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlServer(_configuration.GetConnectionString("App"));
	}
}
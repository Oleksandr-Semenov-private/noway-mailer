using IntappDte.Extensions;
using IronSalesmanBot.Bot.Domain.Data.Models;
using MailerRobot.Bot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telegram.Bot.Types.ReplyMarkups;

namespace IronSalesmanBot.Bot.Domain.Data.Configurations;

internal class MessageChatInfoConfiguration : IEntityTypeConfiguration<MessageChatInfo>
{
	public void Configure(EntityTypeBuilder<MessageChatInfo> builder)
	{
		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id).ValueGeneratedOnAdd();
		builder.Property(x => x.MessageId).IsRequired();
		builder.Property(x => x.ChatId).IsRequired();
		builder.Property(x => x.Date).IsRequired();

		builder.Property(x => x.Type)
				.IsRequired()
				.HasConversion(
					v => v.ToString(),
					v => v.ToEnum<MessageChatType>());

		builder.Property(x => x.Keyboard).HasConversion(
			v => v.Serialize(),
			v => v.Deserialize<InlineKeyboardMarkup>());
	}
}
namespace MailerRobot.Bot.Domain.Models;

public class Subscription
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public DateTime EndDate { get; set; }
	
	public DateTime OrderDate { get; set; }

	public async Task<string> GetRemainingDaysToStringAsync()
	{
		var difference = EndDate - DateTime.Now;
		
		var totalHours = (int)difference.TotalHours;
		var hours = totalHours % 24; // Remaining hours after subtracting full days
		var minutes = difference.Minutes;

		var qwe = "";

		if (difference.Days < 1)
		{
			qwe =
				$"{totalHours} {(totalHours == 1 ? "час" : totalHours > 1 && totalHours < 5 ? "часа" : "часов")} {minutes} {(minutes == 1 ? "минута" : minutes > 1 && minutes < 5 ? "минуты" : "минут")}";
		}
		else
		{
			qwe = $"{difference.Days} дней {difference.Hours} часов";
		}

			return difference.TotalDays < 0
			? "отсутствует"
			: qwe;;
	}
	
	public int GetRemainingDays()
	{
		var difference = EndDate - DateTime.Now;
		return difference.TotalDays < 0 ? -1 : (int)difference.TotalDays;
	}
	
	
}

/*public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public Uri QueryUrl { get; set; }
    public List<string> IncludeKeywords { get; set; }
    public List<string> ExcludeKeywords { get; set; }
    public bool InitialPull { get; set; }
    public bool Enabled { get; set; }
}*/
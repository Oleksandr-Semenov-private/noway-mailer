namespace MailerRobot.Bot.Domain.Models;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime EndDate { get; set; }
    //public bool Active { get; set; }
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
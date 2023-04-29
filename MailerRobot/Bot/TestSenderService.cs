using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.Services;
using MediatR;

namespace MailerRobot.Bot;

public class TestSenderService 
{
	private HttpClient _client;
	private readonly IMediator _mediator;

	public TestSenderService(IMediator mediator)
	{
		_mediator = mediator;
		_client = new HttpClient();
	}

	protected  async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var emailBase = new List<string>();

		var path = @"C:\Users\Admin\RiderProjects\MailerRobot\MailerRobot\data\basee.txt";

		var lines = File.ReadAllLines(path);
		
		foreach (string line in lines)
		{
			emailBase.Add(line);
		}

		var link = "https://expressdelivery53.online/171564079";

		var counter = 0;

		foreach (var mail in emailBase)
		{
			await SendEmail(mail, link, ServiceType.EbayDe);
			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
			Console.WriteLine(++counter);
		}
		
		
		//await SendEmail("thesyom4egg@gmail.com", link, ServiceType.EbayDe);
	}

	private static async Task SendEmail(string email, string link, ServiceType serviceType)
	{
		using var httpClient = new HttpClient();
		var builder = new UriBuilder("http://localhost:5118/api/Sender/Email");
		builder.Query = $"email={email}&link={link}&serviceType={serviceType}";
		var response = await httpClient.GetAsync(builder.ToString());
		var responseBody = await response.Content.ReadAsStringAsync();
	}
}
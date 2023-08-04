using System.Net;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.Services;
using MediatR;

namespace MailerRobot.Bot;

public class TestSenderService : BackgroundService
{
	private HttpClient _client;
	private readonly IMediator _mediator;
	private static ITelegramBot _bot;

	public TestSenderService(IMediator mediator, ITelegramBot bot)
	{
		_mediator = mediator;
		_client = new HttpClient();
		_bot = bot;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		/*var emailBase = new List<string>();

		var path = @"C:\Users\Admin\RiderProjects\MailerRobot\MailerRobot\data\basee.txt";

		var lines = File.ReadAllLines(path);
		
		foreach (string line in lines)
		{
			emailBase.Add(line);
		}

		var link = "https://ebay-kleinanzeigen.orderwebid.site/refund/4555170099";

		var counter = 0;

		foreach (var mail in emailBase)
		{
			//await SendEmailAPI(mail, link);
			try
			{
				await Task.Delay(2200);
				var sendEmailNotification = new SendEmailApiNotification(mail, link, ServiceType.EbayCongrats);
				await _mediator.Publish(sendEmailNotification, stoppingToken);
				Console.WriteLine(++counter);
				//await _bot.SendAsync(5908829889, "Отправлено", cancellationToken: stoppingToken);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				//await _bot.SendAsync(5908829889, e.Message, cancellationToken: stoppingToken);
			}
		}*/
		
	}
	
	private async Task SendEmailAPI(string email, string link)
	{
		using var client = new HttpClient();
		client.BaseAddress = new Uri("https://noway-mailer.herokuapp.com");
		
		var parameters = new Dictionary<string, string>()
		{
			{"email", email},
			{"link", link}
		};
		var queryString = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;

		var url = "https://noway-mailer.herokuapp.com/api/Sender/ebay-congrats?" + queryString;
		var response = await client.GetAsync(url);

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
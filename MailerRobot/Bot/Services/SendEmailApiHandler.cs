using System.Net;
using MailerRobot.Bot.Domain.Models;
using MediatR;

namespace MailerRobot.Bot.Services;

public class SendEmailApiHandler : INotificationHandler<SendEmailApiNotification>
{
	public async Task Handle(SendEmailApiNotification notification, CancellationToken cancellationToken)
	{
		using var client = new HttpClient();
		client.BaseAddress = new Uri("https://noway-mailer.herokuapp.com");

		var parameters = new Dictionary<string, string>
		{
			{"email", notification.email},
			{"link", notification.link}
		};

		string url;

		switch (notification.ServiceType)
		{
			case ServiceType.EbayDe:
				url = "/api/Sender/ebay";

				break;
			case ServiceType.DHL:
				url = "/api/Sender/dhl";

				break;
			case ServiceType.EbayCongrats:
				url = "api/Sender/ebay-congrats";

				break;
			default:
				throw new Exception();

				break;
		}

		var queryString = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;
		url += "?" + queryString;

		var response = await client.GetAsync(url);
		response.EnsureSuccessStatusCode();

		if (response.StatusCode == HttpStatusCode.OK)
			return;

		throw new Exception("Error sending email: " + response.StatusCode);
	}
}
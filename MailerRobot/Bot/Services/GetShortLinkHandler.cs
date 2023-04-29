using MailerRobot.Bot.Domain.Responses;
using MediatR;

namespace MailerRobot.Bot.Services;

public class GetShortLinkHandler : IRequestHandler<GetShortLinkRequest, string>
{
	private readonly HttpClient _client;

	public GetShortLinkHandler()
	{
		_client = new HttpClient();
	}

	public async Task<string> Handle(GetShortLinkRequest request, CancellationToken ct)
	{
		_client.BaseAddress = new Uri("https://n9.cl/");

		var content = JsonContent.Create(new
		{
			url = request.link
		});

		var msg = await _client.PostAsync("api/short", content, ct);

		var response = await ReadResponseAsync<N9Response>(msg, ct);

		return response.Short;
	}
	
	private static async Task<TResponse> ReadResponseAsync<TResponse>(HttpResponseMessage msg,
		CancellationToken ct = default) where TResponse : class
	{
		var response = await msg.Content.ReadAsStringAsync(ct);

		return IsJson(response) ? response.Deserialize<TResponse>() : default!;
	}
	
	private static bool IsJson(string response)
	{
		return (response.StartsWith("{") && response.TrimEnd().EndsWith("}")) ||
				(response.StartsWith("[") && response.TrimEnd().EndsWith("]"));
	}
}
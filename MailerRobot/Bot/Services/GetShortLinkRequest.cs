using MediatR;

namespace MailerRobot.Bot.Services;

public record GetShortLinkRequest(string link) : IRequest<string>;
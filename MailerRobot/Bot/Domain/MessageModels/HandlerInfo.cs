using MailerRobot.Bot.MessageHandlers.Base;

namespace MailerRobot.Bot.Domain.MessageModels;

internal record HandlerInfo(HandlerName HandlerName, string? Data = default);
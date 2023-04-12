using MailerRobot.Bot.Domain.Models;

namespace MailerRobot.Bot.Domain.MessageModels;

internal record MessageData(int Id, HandlerInfo HandlerInfo, string Language, User From);
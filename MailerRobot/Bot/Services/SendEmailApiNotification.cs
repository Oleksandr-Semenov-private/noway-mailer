using MailerRobot.Bot.Domain.Models;
using MediatR;

namespace MailerRobot.Bot.Services;

public record SendEmailApiNotification(string email, string link, ServiceType ServiceType) : INotification;
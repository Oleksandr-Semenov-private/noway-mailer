using MailerRobot.Bot.Domain.Models;

namespace MailerRobot.Bot.Domain.Interfaces;

public interface IOutgoingNotifications
{
    Task NotifySubscribers(Subscription subscription, Result newLink);
}
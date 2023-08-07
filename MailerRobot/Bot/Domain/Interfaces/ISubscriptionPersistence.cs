using MailerRobot.Bot.Domain.Models;

namespace MailerRobot.Bot.Domain.Interfaces;

public interface ISubscriptionPersistence
{
    Task AddSubscriberAsync(Subscriber subscriber);
    Task<Subscriber[]> GetSubscribersAsync();
    bool RestoreData();
    Task SaveDataAsync();
    Task AddLinkAsync(Subscriber subscriber, string link);
    Task AddEmailAsync(Subscriber subscriber, string email);
    Task AddServiceTypeAsync(Subscriber subscriber, ServiceType serviceType);
    Task CreateSubscriptionAsync(Subscriber subscriber, int countOfDays);
    Task RenewSubscriptionAsync(Subscriber subscriber, int countOfDays);
    Task DeleteExpiredSubscriptionAsync(Subscriber subscriber);
}
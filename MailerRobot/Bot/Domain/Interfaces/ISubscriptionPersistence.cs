using MailerRobot.Bot.Domain.Models;

namespace MailerRobot.Bot.Domain.Interfaces;

public interface ISubscriptionPersistence
{
    List<Subscription> GetEnabledSubscriptions();
    void AddSubscriber(Subscriber subscriber);
    Subscriber[] GetSubscribers();
    bool RestoreData();
    void SaveData();
    void AddLink(Subscriber subscriber, string link);
    void AddEmail(Subscriber subscriber, string email);
    void AddServiceType(Subscriber subscriber, ServiceType serviceType);
}
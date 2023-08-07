using System.Collections.Concurrent;
using MailerRobot.Bot.Domain.Interfaces;
using MailerRobot.Bot.Domain.Models;
using ILogger = Serilog.ILogger;

namespace MailerRobot.Bot;

public class SubscriptionPersistence : ISubscriptionPersistence
{
   // private readonly ILogger _logger
    protected readonly IDataStorage DataStorage;
    protected ConcurrentBag<Subscriber> SubscriberList { get; set; }

    public SubscriptionPersistence(IDataStorage dataStorage)
    {
        Directory.CreateDirectory("data");
        DataStorage = dataStorage;
        SubscriberList = new ConcurrentBag<Subscriber>();
        RestoreData(); // Дожидаемся завершения асинхронной операции в конструкторе (не самое лучшее решение)
    }

    public async Task AddSubscriberAsync(Subscriber subscriber)
    {
        SubscriberList.Add(subscriber);
        await SaveDataAsync();
    }

    public Task<Subscriber[]> GetSubscribersAsync()
    {
        return Task.FromResult(SubscriberList.ToArray());
    }
    
    public bool RestoreData()
    {
        try
        {
            DataStorage.Load(Path.Join("data", "Subscribers.json"), out ConcurrentBag<Subscriber> data);
            SubscriberList = data;
            //_logger.Information($"Restored data: {data.Count} Subscribers");
        }
        catch (Exception e)
        {
            SubscriberList ??= new ConcurrentBag<Subscriber>();

            if (e is FileNotFoundException)
            {
                //_logger.Warning($"Could not restore subscribers: {e.Message}");
            }
            else
            {
                //_logger.Error(e, $"Error when restoring subscribers: {e.Message}");
            }

            return false;
        }

        return true;
    }

    public Task SaveDataAsync()
    {
        return DataStorage.SaveAsync(SubscriberList, Path.Join("data", "Subscribers.json"));
    }

    public async Task AddLinkAsync(Subscriber subscriber, string link)
    {
        subscriber.SubscriberData.Link = link;
        await SaveDataAsync();
    }

    public async Task AddEmailAsync(Subscriber subscriber, string email)
    {
        subscriber.SubscriberData.Email = email;
        await SaveDataAsync();
    }

    public async Task AddServiceTypeAsync(Subscriber subscriber, ServiceType serviceType)
    {
        subscriber.SubscriberData.Type = serviceType;
        await SaveDataAsync();
    }

    public async Task CreateSubscriptionAsync(Subscriber subscriber, int countOfDays)
    {
        var subscription = new Subscription() { EndDate = DateTime.Now.AddDays(countOfDays), OrderDate = DateTime.Now };
        subscriber.Subscriptions = new List<Subscription>() { subscription };
        await SaveDataAsync();
    }

    public async Task RenewSubscriptionAsync(Subscriber subscriber, int countOfDays)
    {
        var subscription = subscriber.Subscriptions.OrderByDescending(sub => sub.OrderDate).First();
        subscription.EndDate = subscription.EndDate.AddDays(countOfDays);
        await SaveDataAsync();
    }

    public async Task DeleteExpiredSubscriptionAsync(Subscriber subscriber)
    {
        subscriber.Subscriptions.RemoveAll(subscription => subscription.GetRemainingDays() < 0);
        await SaveDataAsync();
    }

    //public List<Subscription> GetEnabledSubscriptions()
    //{
    //    return GetSubscribers()
    //        .SelectMany(s => s.Subscriptions)
    //        .Where(s => s.Enabled)
    //        .ToList();
    //}
}
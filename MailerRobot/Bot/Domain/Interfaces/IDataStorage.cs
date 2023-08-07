namespace MailerRobot.Bot.Domain.Interfaces;

public interface IDataStorage
{
    Task SaveAsync<T>(T data, string fileName);
    void Load<T>(string fileName, out T data);
}
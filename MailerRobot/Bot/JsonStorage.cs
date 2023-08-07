using System.Text;
using MailerRobot.Bot.Domain.Interfaces;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;

namespace MailerRobot.Bot;

public class JsonStorage : IDataStorage
{
    private readonly SemaphoreSlim _lockObject = new SemaphoreSlim(1, 1);
    /*private readonly ILogger _logger;

    public JsonStorage(ILogger logger)
    {
        _logger = logger;
    }*/

    public async Task SaveAsync<T>(T data, string fileName)
    {
        //_logger.Verbose($"Saving data to file {fileName}");
        await _lockObject.WaitAsync();
        try
        {
            var jsonString = JsonConvert.SerializeObject(data, new JsonSerializerSettings { Formatting = Formatting.Indented });
            await File.WriteAllTextAsync(fileName, jsonString, Encoding.UTF8);
        }
        finally
        {
            _lockObject.Release();
        }
    }

    /// <summary>
    /// Deserializes the JSON file
    /// </summary>
    /// <exception cref="JsonException">Is thrown if the file doesn't exist</exception>
    public void Load<T>(string fileName, out T data)
    {
        //_logger.Verbose($"Loading data from file {fileName}");
        lock (_lockObject)
        {
            var jsonString = File.ReadAllText(fileName);
            data = JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
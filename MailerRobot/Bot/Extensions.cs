using Newtonsoft.Json;

namespace MailerRobot.Bot;

public static class SerializationExtension
{
	public static T Deserialize<T>(this string json) where T : class => JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException(nameof (Deserialize));

	public static string Serialize<T>(this T obj) => JsonConvert.SerializeObject((object) obj);
}
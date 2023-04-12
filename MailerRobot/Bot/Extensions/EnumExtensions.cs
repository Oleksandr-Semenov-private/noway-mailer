namespace IntappDte.Extensions;

public static class EnumExtensions
{
	public static T ToEnum<T>(this string name) where T : struct, Enum
	{
		var tryParse = Enum.TryParse(name, true, out T result);

		if (tryParse is false)
			throw new ArgumentException($"{name} is not a valid value for {typeof(T).Name}");

		return result;
	}
}
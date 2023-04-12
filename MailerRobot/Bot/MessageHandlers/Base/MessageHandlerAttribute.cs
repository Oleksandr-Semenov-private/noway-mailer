namespace IronSalesmanBot.Bot.MessageHandlers.Base;

[AttributeUsage(AttributeTargets.Class)]
internal class MessageHandlerAttribute : Attribute
{
	public HandlerName Name { get; }

	public MessageHandlerAttribute(HandlerName name)
	{
		Name = name;
	}
}
using System.Reflection;
using Autofac;
using IronSalesmanBot.Bot.MessageHandlers.Base;
using MailerRobot.Bot.Domain.Data;
using MailerRobot.Bot.Extensions;
using Microsoft.EntityFrameworkCore;
using Module = Autofac.Module;

namespace IronSalesmanBot.DependencyInjection;

public class MainModule : Module
{
	private readonly IConfiguration _configuration;

	public MainModule(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	protected override void Load(ContainerBuilder builder)
	{
		var executingAssembly = Assembly.GetExecutingAssembly();
		builder.RegisterServices(executingAssembly);
		
		builder.RegisterDbContext<ApplicationContext>(e =>
			e.UseSqlServer(_configuration.GetConnectionString("App")));
		
		RegisterHandlers(builder);
	}

	private static void RegisterHandlers(ContainerBuilder builder)
	{
		Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type => type.IsAssignableTo(typeof(MessageHandler)))
				.Where(t => t.GetCustomAttribute<MessageHandlerAttribute>() is not null)
				.ForEach(t => RegisterMessageHandler(builder, t));
	}

	private static object RegisterMessageHandler(ContainerBuilder builder, Type t)
	{
		var name = t.GetCustomAttribute<MessageHandlerAttribute>()!.Name;

		return builder.RegisterType(t)
					.Keyed<MessageHandler>(name)
					.InstancePerLifetimeScope();
	}
}
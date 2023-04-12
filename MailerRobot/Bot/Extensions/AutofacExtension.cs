using System.Reflection;
using Autofac;
using Autofac.Builder;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using MailerRobot.Bot.Extensions.Attributes;
using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace MailerRobot.Bot.Extensions;

public static class AutofacExtension
{
	public static IRegistrationBuilder<TContext, ConcreteReflectionActivatorData, SingleRegistrationStyle>
		RegisterDbContext<TContext>(this ContainerBuilder builder,
			Func<DbContextOptionsBuilder<TContext>, DbContextOptionsBuilder<TContext>> action, params object[] keys)
		where TContext : DbContext
	{
		var optionsBuilder = new DbContextOptionsBuilder<TContext>();

		var contextOptions = action(optionsBuilder).Options;

		var registration = builder.RegisterType<TContext>()
								.WithParameter("options", contextOptions)
								.InstancePerLifetimeScope();

		if (keys.Any() is not true)
			return registration.AsSelf();

		keys.ForEach(e => registration.Keyed<TContext>(e));

		return registration.AsSelf();
	}

	public static void RegisterServices(this ContainerBuilder builder, Assembly assembly)
	{
		builder.RegisterTransient(assembly);
		builder.RegisterScoped(assembly);
		builder.RegisterSingletons(assembly);

		builder.RegisterAutoMapper(assembly);
		builder.RegisterMediatR(assembly);
	}
	
	public static void RegisterInterfaceImplementationsAsKey<T>(this ContainerBuilder builder,
		Assembly assembly,
		Type interfaceType, Func<Type, T> getKey)
	{
		var types = assembly.GetTypes()
							.Where(val =>
								interfaceType.IsAssignableFrom(val)
								&& val.IsInterface is false
								&& val.IsAbstract is false);

		var results = types.Select(val => new
		{
			type = val,
			key = getKey(val)
		});

		foreach (var result in results)
			builder.RegisterType(result.type).Keyed(result.key!, interfaceType);
	}

	private static void RegisterTransient(this ContainerBuilder builder, Assembly assembly)
	{
		builder.RegisterAssemblyTypes(assembly)
				.Where(NotImplementedINotificationHandler)
				.AsImplementedInterfaces();
	}

	private static bool NotImplementedINotificationHandler(Type type)
	{
		return type.GetTypeInfo()
					.ImplementedInterfaces
					.Where(e => e.IsGenericType)
					.Select(e => e.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
					.Any() is not true;
	}

	private static void RegisterScoped(this ContainerBuilder builder, Assembly assembly)
	{
		var scoped = assembly.GetTypes()
							.Where(t => t.GetCustomAttribute<ScopedAttribute>() is not null)
							.ToArray();

		if (scoped.Any())
			builder.RegisterTypes(scoped)
					.AsImplementedInterfaces()
					.InstancePerLifetimeScope();
	}

	private static void RegisterSingletons(this ContainerBuilder builder, Assembly assembly)
	{
		var singletons = assembly.GetTypes()
								.Where(t => t.GetCustomAttribute<SingletonAttribute>() is not null)
								.ToArray();

		if (singletons.Any())
			builder.RegisterTypes(singletons)
					.AsImplementedInterfaces()
					.SingleInstance();
	}
}
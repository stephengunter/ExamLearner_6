using System.Reflection;
using Autofac;
using ApplicationCore.DataAccess;
using Infrastructure.Interfaces;
using Module = Autofac.Module;
using MediatR;
using MediatR.Pipeline;
using Infrastructure.Events;
using Autofac.Core.Activators.Reflection;
using ApplicationCore.Auth;
using ApplicationCore.Models;

namespace ApplicationCore;
public class ApplicationCoreModule : Module
{
	private readonly bool _isDevelopment = false;
	public ApplicationCoreModule(bool isDevelopment)
	{
		_isDevelopment = isDevelopment;
	}

	protected override void Load(ContainerBuilder builder)
	{
		if (_isDevelopment)
		{
			RegisterDevelopmentOnlyDependencies(builder);
		}
		else
		{
			RegisterProductionOnlyDependencies(builder);
		}

		RegisterCommonDependencies(builder);
	}

	private void RegisterCommonDependencies(ContainerBuilder builder)
	{
		//builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
		//builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
		//builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
		//builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance().FindConstructorsWith(new InternalConstructorFinder());
		
		builder.RegisterGeneric(typeof(DefaultRepository<>))
		  .As(typeof(IDefaultRepository<>))
		  .InstancePerLifetimeScope();

		
		builder.RegisterAssemblyTypes(GetAssemblyByName("ApplicationCore"))
						 .Where(t => t.Name.EndsWith("Service"))
						 .AsImplementedInterfaces()
						 .InstancePerLifetimeScope();

	}

	public static Assembly GetAssemblyByName(String name) => Assembly.Load(name);

	private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder)
	{
		// NOTE: Add any development only services here
		//builder.RegisterType<FakeEmailSender>().As<IEmailSender>()
		//.InstancePerLifetimeScope();
	}

	private void RegisterProductionOnlyDependencies(ContainerBuilder builder)
	{
		// NOTE: Add any production only services here
		//builder.RegisterType<SmtpEmailSender>().As<IEmailSender>()
		//.InstancePerLifetimeScope();
	}
}

public class InternalConstructorFinder : IConstructorFinder
{
	public ConstructorInfo[] FindConstructors(Type targetType)
		  => targetType.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsPrivate && !c.IsPublic).ToArray();
}

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Web.Framework.Infrastructure
{
    public class CustomDependencyInjection : NopStartup
    {
        private static bool IsSubInterface(Type t1, Type t2)
        {
            return t2.IsAssignableFrom(t1) && t1 != t2;
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var servicesAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Single(assembly => assembly.FullName.Contains("Nop.Services"));

            var customServiceTypes = servicesAssembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract &&
                    IsSubInterface(type, typeof(ICustomService)));

            var orderedCustomServiceTypes = customServiceTypes
                .OrderBy(type => type.GetCustomAttribute<ServiceOrderAttribute>()?.Order ?? 0);

            foreach (var customServiceType in orderedCustomServiceTypes)
            {
                var injectAttribute = customServiceType.GetCustomAttribute<InjectAttribute>() ??
                    new InjectAttribute(InjectType.Scoped);

                var implementationAttribute = customServiceType.GetCustomAttribute<ImplementationAttribute>() ??
                    new ImplementationAttribute(ImplementationType.WithoutInterface);

                Type interfaceType = null;
                if (implementationAttribute.Type == ImplementationType.WithInterface)
                {
                    var interfaceTypeName = "I" + customServiceType.Name;
                    interfaceType = servicesAssembly.GetTypes().SingleOrDefault(type =>
                        type.IsInterface && type.Name == interfaceTypeName);
                }

                switch (injectAttribute.Type)
                {
                    case InjectType.Scoped:
                        if (interfaceType != null)
                        {
                            services.AddScoped(interfaceType, customServiceType);
                        }
                        else
                        {
                            services.AddScoped(customServiceType);
                        }
                        break;
                    case InjectType.Transient:
                        if (interfaceType != null)
                        {
                            services.AddTransient(interfaceType, customServiceType);
                        }
                        else
                        {
                            services.AddTransient(customServiceType);
                        }
                        break;
                    case InjectType.Singleton:
                        if (interfaceType != null)
                        {
                            services.AddSingleton(interfaceType, customServiceType);
                        }
                        else
                        {
                            services.AddSingleton(customServiceType);
                        }
                        break;
                }
            }
        }
    }
}

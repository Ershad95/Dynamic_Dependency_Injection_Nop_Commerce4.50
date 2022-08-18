using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Framework.Infrastructure
{
    public class CustomDependencyInjection : NopStartup
    {
        private static bool IsSubInterface(Type t1, Type t2)
        {
            if (!t2.IsAssignableFrom(t1))
                return false;

            if (t1.BaseType == null)
                return true;

            return !t2.IsAssignableFrom(t1.BaseType);
        }
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //-------------Get All Services-------------
            var asm = AppDomain.CurrentDomain
                 .GetAssemblies()
                 .Single(x => x.FullName.Contains("Nop.Services"));
            //-------------find Services that inheriance of ICustomService-------------
            var types = asm.DefinedTypes.Where(x => IsSubInterface(x, typeof(ICustomService)));
            //-----------Get All Custom Service Classess-------
            var allRelatedClassServices = types
                .Where(x => x.IsClass)
                .OrderBy(x=>(Int32)x.GetProperty("Order")
                .GetValue(Activator.CreateInstance(x), null));

            //-----------Get All Custom Service Interfaces-------
            var allRelatedInterfaceServices = types.Where(x => x.IsInterface);
            //-----------Matche Class Services To Related Interface Services-------
            TypeInfo interfaceService=null;
            foreach (var classService in allRelatedClassServices)
            {
                //-----------detect Implementation Type for service-----------
                var implementationValue = (ImplementationType)classService.GetProperty("implementationType")
                   .GetValue(Activator.CreateInstance(classService), null);

                //-----------detect inject type for service-----------
                var InjectValue = (InjectType)classService.GetProperty("Inject")
                   .GetValue(Activator.CreateInstance(classService), null);

                //-----------get related interface for service class-----------
                if (implementationValue == ImplementationType.WithInterface)
                    interfaceService = allRelatedInterfaceServices.Single(x => x.Name == $"I{classService.Name}");

               

                //----------finally Add Custom Service To Service Collection-----------
                switch (InjectValue)
                {
                    case InjectType.Scopped:
                        if(interfaceService!=null)
                            services.AddScoped(interfaceService, classService);
                        else
                            services.AddScoped(classService);
                        break;
                    case InjectType.Transit:
                        if (interfaceService != null)
                            services.AddTransient(interfaceService, classService);
                        else
                            services.AddTransient(classService);
                        break;
                    case InjectType.SingleTon:
                        if (interfaceService != null)
                            services.AddSingleton(interfaceService, classService);
                        else
                            services.AddSingleton(classService);
                        break;
                    default:
                        break;
                }
                interfaceService = null;
            }
        }
       
    }
}

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
            var allRelatedClassServices = types.Where(x => x.IsClass);
            //-----------Get All Custom Service Interfaces-------
            var allRelatedInterfaceServices = types.Where(x => x.IsInterface);
            //-----------Matche Class Services To Related Interface Services-------
            foreach (var classService in allRelatedClassServices)
            {
                //-----------get related interface for service class-----------
                var interfaceService = allRelatedInterfaceServices.Single(x => x.Name == $"I{classService.Name}");
                //-----------detect inject type for service-----------
                var InjectValue = (InjectType)classService.GetProperty("Inject")
                   .GetValue(Activator.CreateInstance(classService), null);
                //----------finally Add Custom Service To Service Collection-----------
                switch (InjectValue)
                {
                    case InjectType.Scopped:
                        services.AddScoped(interfaceService, classService);
                        break;
                    case InjectType.Transit:
                        services.AddTransient(interfaceService, classService);
                        break;
                    case InjectType.SingleTon:
                        services.AddSingleton(interfaceService, classService);
                        break;
                    default:
                        break;
                }
            }
        }
    }

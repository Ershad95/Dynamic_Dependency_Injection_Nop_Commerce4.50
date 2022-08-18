using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services
{
    public class MyServiceWithoutImplemention : ICustomService
    {
        public ImplementationType implementationType { get;  }
        public InjectType Inject { get;  }
        public int Order { get;  }
        public MyServiceWithoutImplemention()
        {
            implementationType = ImplementationType.WithoutInterface;
            Inject = InjectType.Scopped;
        }

        public int ok()
        {
            return 100;
        }
    }
}

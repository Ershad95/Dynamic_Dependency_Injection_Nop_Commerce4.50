using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services
{
    public class MyCustomService : IMyCustomService
    {
        public InjectType Inject { get;  }
        public int Order { get;  }
        public ImplementationType implementationType { get;  }

        public MyCustomService()
        {
            implementationType = ImplementationType.WithInterface;
            Inject = InjectType.Scopped;
            Order = 1;
        }
        public int ok()
        {
            return 10;
        }
    }
}

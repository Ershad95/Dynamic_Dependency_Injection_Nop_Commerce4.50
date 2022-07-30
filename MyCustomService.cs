using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services
{
    public class MyCustomService : IMyCustomService
    {
        public InjectType Inject { get; set; }
        public MyCustomService()
        {
            Inject = InjectType.Scopped;
        }
        public int ok()
        {
            return 10;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services
{
    public interface ICustomService
    {
        protected InjectType Inject { get;  }
        protected int Order { get; }
        protected ImplementationType implementationType { get; }
    }
    public enum ImplementationType
    {
        WithInterface = 0,
        WithoutInterface = 1
    }
    public enum InjectType
    {
        Scopped=0,
        Transit=1,
        SingleTon=2
    }
}

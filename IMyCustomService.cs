using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services
{
    public interface IMyCustomService: ICustomService
    {
        int ok();
    }
}

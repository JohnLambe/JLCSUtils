using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.DependencyInjection
{
    public interface IDiContext
    {
        T BuildUp<T>(T target);
        //TODO
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitNetCore.App
{
    public interface ICalculatorService
    {
        int add(int a, int b);
        int multiple(int a, int b);
    }
}

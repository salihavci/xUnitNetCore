using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitNetCore.App
{
    public class Calculator
    {
        private readonly ICalculatorService _calculatorService;

        public Calculator(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }
        public int add(int a, int b)
        {
            return _calculatorService.add(a, b);
        }

        public int multiple(int a, int b)
        {
            return _calculatorService.multiple(a, b);
        }
    }
}

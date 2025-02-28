using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorApp
{
    public class Model
    {
        public double FirstNumber { get; set; }
        public double SecondNumber { get; set; }

        public String Operator { get; set; }

        public double Calculate()
        {
            return Operator switch
            {
                "+" => FirstNumber + SecondNumber,
                "-" => FirstNumber - SecondNumber,
                "*" => FirstNumber * SecondNumber,
                "/" => SecondNumber != 0 ? FirstNumber / SecondNumber : 0,
                "^" => FirstNumber * FirstNumber,
                "S" => Math.Sqrt(FirstNumber),
                _ => 0,
            }; ;
        }

    }
}

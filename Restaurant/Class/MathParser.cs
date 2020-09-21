using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bestcode.MathParser;

namespace RedDot
{
    public class ExpressionParser
    {

         public decimal Evaluate(string expression,double x, double y )
        {
            MathParser parser = new MathParser();
            parser.X = x;
            parser.Y = y;
            parser.Expression = expression;
            return  Decimal.Parse( parser.Value.ToString());
        }
        
    }
}

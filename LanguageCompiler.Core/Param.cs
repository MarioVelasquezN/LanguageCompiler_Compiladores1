using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class Param
    {
        public Expression Expression { get; set; }
        public ExpressionType Type { get; set; }

        public Param(Expression expression, ExpressionType type)
        {
            Expression = expression;
            Type = type;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class Param : Expression
    {
        public Expression Expression { get; set; }
        public ExpressionType Type { get; set; }

        public Param(Expression expression, ExpressionType type)
        {
            Expression = expression;
            Type = type;
        }

        public override ExpressionType GetType()
        {
            return Type;
        }

        public override string GenerateCode() =>
            this.Name;

        public override dynamic Evaluate() =>
            ContextManager.GetSymbolForInterpretation(this.Name).Value;
    }
}

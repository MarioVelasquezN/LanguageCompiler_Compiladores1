using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class PrintStatement : Statement
    {
        public Expression Expression { get; set; }

        public PrintStatement(Expression expression)
        {
            Expression = expression;
            this.ValidateSemantic();
        }

        public override void ValidateSemantic()
        {
            // if (this.Expressions.Any(x => x.GetType() != ExpressionType.String))
            // {
            //     throw new ApplicationException("Cannot implicitly convert all print parameters to string");
            // }
        }

        public override string GenerateCode() =>
            $"console.log({this.Expression.GenerateCode()});";



        public override void Interpret()
        {
            
                var exprValue = Expression.Evaluate();
                Console.Write(exprValue);
            
        }
    }
}

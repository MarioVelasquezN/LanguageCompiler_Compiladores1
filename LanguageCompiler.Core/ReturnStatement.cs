using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ReturnStatement : Statement
    {
        public ReturnStatement(Expression Expression)
        {
            Expression = Expression;
        }

        public Expression Expression { get; set; }

        public override void ValidateSemantic()
        {
            var exprType = this.Expression.GetType();
            if (exprType != ExpressionType.Bool)
            {
                throw new ApplicationException($"Cannot implicitly convert '{exprType}' to bool");
            }
        }

        public override string GenerateCode() =>
            $"while({this.Expression.GenerateCode()}){{ {Environment.NewLine} }}";

        public override void Interpret()
        {
            this.Expression.Evaluate();           
        }
    }
}

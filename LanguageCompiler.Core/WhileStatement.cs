using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{

    public class WhileStatement : Statement
    {
        public Expression Expression { get; set; }
        public Statement Statement { get; set; }

        public WhileStatement(Expression expression, Statement statement)
        {
            Expression = expression;
            Statement = statement;
            this.ValidateSemantic();
        }

        public override void ValidateSemantic()
        {
            var exprType = this.Expression.GetType();
            if (exprType != ExpressionType.Bool)
            {
                throw new ApplicationException($"Cannot implicitly convert '{exprType}' to bool");
            }
        }

        public override string GenerateCode() =>
            $"while({this.Expression.GenerateCode()}){{ {Environment.NewLine} {this.Statement.GenerateCode()} {Environment.NewLine}}}";

        public override void Interpret()
        {
            while (this.Expression.Evaluate())
            {
                this.Statement?.Interpret();
            }
        }
    }
}

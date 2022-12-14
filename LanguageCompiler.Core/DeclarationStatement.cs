using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class DeclarationStatement : Statement
    {
        public DeclarationStatement(Expression expression, ExpressionType type, Statement assignation)
        {
            Expression = expression;
            Type = type;
            Assignation = assignation;
            this.ValidateSemantic();
        }

        public Expression Expression { get; set; }
        public ExpressionType Type { get; set; }
        public Statement Assignation { get; set; }

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
            while (this.Expression.Evaluate())
            {
                this.Assignation?.Interpret();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ForStatement : Statement
    {
        public ForStatement(Statement variables, Expression expression1, Expression expression2, Statement statement)
        {
            Variables = variables;
            Expression1 = expression1;
            Expression2 = expression2;
            Statement = statement;
            this.ValidateSemantic();
        }

        public Statement Variables { get; set; }
        public Expression Expression1 { get; set; }
        public Expression Expression2 { get; set; }
        public Statement Statement { get; set; }

        public override void ValidateSemantic()
        {
            var exprType = this.Expression1.GetType();
            if (exprType != ExpressionType.Bool)
            {
                throw new ApplicationException($"Cannot implicitly convert '{exprType}' to bool");
            }
            exprType = this.Expression2.GetType();
            if (exprType != ExpressionType.Bool)
            {
                throw new ApplicationException($"Cannot implicitly convert '{exprType}' to bool");
            }
        }

        public override string GenerateCode() =>
            $"while({this.Expression1.GenerateCode()}){{ {Environment.NewLine} {this.Statement.GenerateCode()} {Environment.NewLine}}}";

        public override void Interpret()
        {
            while (this.Expression1.Evaluate())
            {
                this.Statement?.Interpret();
            }
            while (this.Expression2.Evaluate())
            {
                this.Statement?.Interpret();
            }
        }
    }
}

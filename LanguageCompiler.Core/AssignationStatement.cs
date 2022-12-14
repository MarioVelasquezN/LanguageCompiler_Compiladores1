using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class AssignationStatement : Statement
    {
        public IdExpression Id { get; set; }
        public Expression Expression { get; set; }

        public Statement statement { get; set; }

        public AssignationStatement(IdExpression id, Expression expression, Statement statement)
        {
            Id = id;
            Expression = expression;
            this.ValidateSemantic();
            this.statement = statement;
        }

        public override void ValidateSemantic()
        {
            var idType = this.Id.GetType();
            var exprType = this.Expression.GetType();
            if (idType != exprType)
            {
                throw new ApplicationException($"Cannot convert source type '{exprType}' to {idType}");
            }
        }

        public override string GenerateCode() =>
            $"{this.Id.GenerateCode()} = {this.Expression.GenerateCode()};";

        public override void Interpret()
        {
            var expressionValue = this.Expression.Evaluate();
            ContextManager.UpdateSymbol(this.Id.Name, expressionValue);
        }
    }
}

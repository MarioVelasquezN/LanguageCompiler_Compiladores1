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
        public Expression Expression { get; set; }

        public Statement Statement { get; set; }

        public AssignationStatement( Expression expression, Statement statement)
        {
            Expression = expression;
            this.ValidateSemantic();
            Statement = statement;  
        }

        public override void ValidateSemantic()
        {
            
                
        }

        public override string GenerateCode() =>
            $"{this.Expression.GenerateCode()};";

        public override void Interpret()
        {
            //var expressionValue = this.Expression.Evaluate();
            //ContextManager.UpdateSymbol(this.Id.Name, expressionValue);
        }
    }
}

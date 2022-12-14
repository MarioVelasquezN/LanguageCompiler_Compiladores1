using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ForEachStatement : Statement
    {
        public ForEachStatement(Statement variables, Expression Expression, Statement statement)
        {
            Variables = variables;
            Expression = Expression;
            Statement = statement;
            this.ValidateSemantic();
        }

        public Statement Variables { get; set; }
        public Expression Expression { get; set; }
        public Statement Statement { get; set; }

        public override void ValidateSemantic()
        {
            
        }

        public override string GenerateCode() =>
            $"while({this.Expression.GenerateCode()}){{ {Environment.NewLine} }}";

        public override void Interpret()
        {
            while (this.Expression.Evaluate())
            {
                this.Statement?.Interpret();
            }
        }
    }
}
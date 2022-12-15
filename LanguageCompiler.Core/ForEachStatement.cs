using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ForEachStatement : Statement
    {
        public ForEachStatement( Expression expression1, Expression expression2, Statement statement)
        {
            Expression1 = expression1;
            Expression2 = expression2;
            Statement = statement;
            this.ValidateSemantic();
        }

        public Expression Expression1 { get; set; }
        public Expression Expression2 { get; set; }
        public Statement Statement { get; set; }

        public override void ValidateSemantic()
        {
            
        }

        public override string GenerateCode() =>
            $"while({this.Expression1.GenerateCode()}){{ {Environment.NewLine} }}";

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
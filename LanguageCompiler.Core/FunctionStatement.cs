using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class FunctionStatement : Statement
    {
        public Expression Identifier    { get; set; }
        public string Params { get; set; }

        public ExpressionType Type { get; set; }

        public Statement CompoundStatement { get; set; }

        public FunctionStatement(Expression identifier,string @params, ExpressionType type, Statement compoundStatement)
        {
            Identifier = identifier;
            Params = @params;
            Type = type;
            CompoundStatement = compoundStatement;
            this.ValidateSemantic();
        }

        public override void ValidateSemantic()
        {

        }

        public override string GenerateCode() =>
            $"{this.Type.GetType()} {this.Identifier.GenerateCode()}({string.Join(",", this.Params.Select(x => x.GenerateCode()))}) {{ {Environment.NewLine} {this.CompoundStatement.GenerateCode()} {Environment.NewLine}}}";

        public override void Interpret()
        {
            while (this.Identifier.Evaluate())
            {
                this.CompoundStatement?.Interpret();
            }

        }
    }
}

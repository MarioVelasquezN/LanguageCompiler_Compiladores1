using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class CompoundStatement : Statement
    {
        public Statement stmt { get; set; }

        public CompoundStatement(Statement stmt)
        {
            this.stmt = stmt;
            this.ValidateSemantic();
        }

        public override void ValidateSemantic()
        {
            this.stmt?.ValidateSemantic();
        }

        public override string GenerateCode() =>
            $"{this.stmt?.GenerateCode()} {Environment.NewLine}";

        public override void Interpret()
        {
            this.stmt?.Interpret();
        }
    }
}

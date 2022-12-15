using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ContinueStatement : Statement
    {
        public override void ValidateSemantic() { }

        public override string GenerateCode() { return $"continue"; }

        public override void Interpret()
        {

        }
    }
}

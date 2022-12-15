using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class BreakStatement : Statement
    {
        public override void ValidateSemantic() { }

        public override string GenerateCode() { return $"break;"; }


        public override void Interpret()      {

        }
    }
}

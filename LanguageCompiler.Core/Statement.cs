using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public abstract class Statement
    {
        public abstract void ValidateSemantic();

        public abstract string GenerateCode();

        public abstract void Interpret();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ArrayAccessExpression : Expression
    {
        public ExpressionType Type { get; }
        public IdExpression Id { get; }

        public Expression Index { get; }

        public ArrayAccessExpression(ExpressionType type, IdExpression id, Expression index)
        {
            Type = type;
            Id = id;
            Index = index;
        }

        public override ExpressionType GetType()
        {
            return Type;
        }

        public override string GenerateCode() =>
            $"{this.Id.Name}[{this.Index.GenerateCode()}]";

        public override dynamic Evaluate()
        {
            var symbol = this.Id.Evaluate();
            var index = this.Index.Evaluate();
            return symbol[(int)index];
        }
    }
}

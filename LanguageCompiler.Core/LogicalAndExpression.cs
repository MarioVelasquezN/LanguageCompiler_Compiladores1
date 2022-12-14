using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class LogicalAndExpression : BinaryExpression
    {
        private readonly Dictionary<(ExpressionType, ExpressionType), ExpressionType> _typeRules;
        public LogicalAndExpression(Expression leftExpression, Expression rightExpression)
            : base(leftExpression, rightExpression)
        {
            _typeRules = new Dictionary<(ExpressionType, ExpressionType), ExpressionType>
        {
            { (ExpressionType.Bool, ExpressionType.Bool), ExpressionType.Bool }
        };
        }

        public override ExpressionType GetType()
        {
            var leftType = this.LeftExpression.GetType();
            var rightType = this.RightExpression.GetType();
            if (_typeRules.TryGetValue((leftType, rightType), out var resultType))
            {
                return resultType;
            }

            throw new ApplicationException($"Cannot apply operator '&&' to operands of type {leftType} and {rightType}");
        }

        public override string GenerateCode() =>
            $"{this.LeftExpression.GenerateCode()} && {this.RightExpression.GenerateCode()}";

        public override dynamic Evaluate() =>
            this.LeftExpression.Evaluate() && this.RightExpression.Evaluate();
    }
}

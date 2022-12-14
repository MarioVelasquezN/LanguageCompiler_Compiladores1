using LanguagueCompiler.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class ArithmeticExpression : BinaryExpression
    {
        private readonly Dictionary<(ExpressionType, ExpressionType, TokenType), ExpressionType> _typeRules;
        public Token Operation { get; }

        public ArithmeticExpression(Expression leftExpression, Expression rightExpression, Token operation)
            : base(leftExpression, rightExpression)
        {
            Operation = operation;
            _typeRules = new Dictionary<(ExpressionType, ExpressionType, TokenType), ExpressionType>
        {
            {(ExpressionType.Int, ExpressionType.Int, TokenType.Plus), ExpressionType.Int},
            {(ExpressionType.Int, ExpressionType.Int, TokenType.Minus), ExpressionType.Int},
            {(ExpressionType.Int, ExpressionType.Int, TokenType.Asterisk), ExpressionType.Int},
            {(ExpressionType.Int, ExpressionType.Int, TokenType.Division), ExpressionType.Int},

            {(ExpressionType.Float, ExpressionType.Float, TokenType.Plus), ExpressionType.Float},
            {(ExpressionType.Float, ExpressionType.Float, TokenType.Minus), ExpressionType.Float},
            {(ExpressionType.Float, ExpressionType.Float, TokenType.Asterisk), ExpressionType.Float},
            {(ExpressionType.Float, ExpressionType.Float, TokenType.Division), ExpressionType.Float},

            {(ExpressionType.Int, ExpressionType.Float, TokenType.Plus), ExpressionType.Float},
            {(ExpressionType.Float, ExpressionType.Int, TokenType.Plus), ExpressionType.Float},

            {(ExpressionType.String, ExpressionType.String, TokenType.Plus), ExpressionType.String},
        };
        }

        public override ExpressionType GetType()
        {
            var leftType = this.LeftExpression.GetType();
            var rightType = this.RightExpression.GetType();
            if (_typeRules.TryGetValue((leftType, rightType, Operation.TokenType), out var resultType))
            {
                return resultType;
            }

            throw new ApplicationException($"Cannot apply operator '{Operation.Lexeme}' to operands of type {leftType} and {rightType}");
        }

        public override string GenerateCode() =>
            $"{this.LeftExpression.GenerateCode()} {this.Operation.Lexeme} {this.RightExpression.GenerateCode()}";

        public override dynamic Evaluate()
        {
            switch (this.Operation.TokenType)
            {
                case TokenType.Plus: return this.LeftExpression.Evaluate() + this.RightExpression.Evaluate();
                case TokenType.Minus: return this.LeftExpression.Evaluate() - this.RightExpression.Evaluate();
                case TokenType.Asterisk: return this.LeftExpression.Evaluate() * this.RightExpression.Evaluate();
                case TokenType.Division: return this.LeftExpression.Evaluate() / this.RightExpression.Evaluate();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

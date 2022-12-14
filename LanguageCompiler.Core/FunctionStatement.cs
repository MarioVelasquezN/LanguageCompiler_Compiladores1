﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageCompiler.Core
{
    public class FunctionStatement : Statement
    {
        public Expression Identifier    { get; set; }
        public List<Expression> Params { get; set; }

        public ExpressionType Type { get; set; }

        public Statement CompoundStatement { get; set; }

        public FunctionStatement(Expression identifier, List<Expression> @params, ExpressionType type, Statement compoundStatement)
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
            $"while({this.Identifier.GenerateCode()}){{ {Environment.NewLine} {this.Statement.GenerateCode()} {Environment.NewLine}}}";

        public override void Interpret()
        {
            while (this.Identifier.Evaluate())
            {
                this.CompoundStatement?.Interpret();
            }

        }
    }
}
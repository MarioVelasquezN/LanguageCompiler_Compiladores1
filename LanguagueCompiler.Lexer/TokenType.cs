﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguagueCompiler.Lexer
{
    public enum TokenType
    {
        Plus,
        Minus,
        Asterisk,
        Division,
        LessThan,
        GreaterThan,
        Equal,
        EOF,
        LessOrEqualThan,
        GreaterOrEqualThan,
        Id,
        IfKeword,
        ElseKeword,
        WhileKeword,
        ForKeword,
        SwitchKeword,
        CaseKeword,
        DoKeword,
        ReturnKeword,
        IntKeword,
        DoubleKeword,
        FloatKeword,
        StringKeword,
        ForeachKeword,
        ClassKeword,
        VoidKeword,
        InKeword,
        BoolKeword,
        DateKeword,
        ConsoleKeword,
        IntListKeword,
        IntConstant,
        FloatConstant,
        Identifier,
        NotEqual,
        LeftParens,
        RightParens,
        SemiColon,
        Colon,
        CharConst,
        Assignation,
        StringConstant,
        OpenBrace,
        CloseBrace,
        OpenArray,
        CloseArray,
        Comma,
        LogicalAnd,
        LogicalOr,
        DateConstant,
        Increment,
        Decrement,
        And,
        Or,
        Not,
        Decimal,
        Mod,
        OpenList,
        CloseList,
        BitwiseOpAnd,
        BitwiseOpOr,
        BitwiseOpXOR,
        AdditionAssigment,
        Equality,
        Inequality,
        ConstKeword,
        TrueKeword,
        FalseKeword,
        VarKeword,
        LetKeword,
        OfKeword,
        ContinueKeword,
        BreakKeword,
        ImportKeword,
        SubtractionAssignment,
        MultiplicationAssignment,
        Exponentiation,
        RemainderAssigment,
        BitwiseXORAssignment,
        LogKeword,
        FuncAssig
    }
}

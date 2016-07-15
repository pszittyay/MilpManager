﻿using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsEqual && arguments.Length == 2;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments[0].ConstantValue.Value == arguments[1].ConstantValue.Value ? 1 : 0);
            }

            var result = milpManager.Operation(OperationType.IsNotEqual, arguments).Operation(OperationType.BinaryNegation);

            result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
                ? arguments[0].ConstantValue == arguments[1].ConstantValue ? 1 : 0
                : (double?) null;
            result.Expression = $"{arguments[0].FullExpression()} ?== {arguments[1].FullExpression()}";
            return result;
        }
    }
}

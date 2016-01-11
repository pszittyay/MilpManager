﻿using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsLessThanCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsLessThan && arguments.Length == 2 && (arguments.All(x => x.IsInteger()) || arguments.All(x => x.IsConstant()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments[0].ConstantValue.Value < arguments[1].ConstantValue.Value ? 1 : 0);
            }
            var result = milpManager.Operation(OperationType.IsGreaterThan, arguments[1], arguments[0]);
            result.Expression = $"({arguments[0].Expression} ?< {arguments[1].Expression})";
            return result;
        }
    }
}

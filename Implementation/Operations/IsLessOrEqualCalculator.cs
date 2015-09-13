﻿using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsLessOrEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsLessOrEqual && arguments.Length == 2 && arguments.All(x => x.IsInteger()); ;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            return arguments[0].Operation(OperationType.IsGreaterThan, arguments[1])
                .Operation(OperationType.BinaryNegation);
        }
    }
}

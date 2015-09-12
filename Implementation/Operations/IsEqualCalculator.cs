﻿using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsEqual && arguments.Length == 2 && arguments.All(x => x.IsInteger());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            return milpManager.Operation(OperationType.IsNotEqual, arguments).Operation(OperationType.BinaryNegation);
        }
    }
}

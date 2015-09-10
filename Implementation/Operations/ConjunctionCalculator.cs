﻿using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class ConjunctionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Conjunction && arguments.Length >= 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            var variable = milpManager.CreateAnonymous(Domain.BinaryInteger);
            var sum = milpManager.Operation(OperationType.Addition, arguments);
            var argumentsCount = arguments.Length;
            sum.Operation(OperationType.Subtraction,
                milpManager.FromConstant(argumentsCount).Operation(OperationType.Multiplication, variable))
                .Set(ConstraintType.LessOrEqual, milpManager.FromConstant(argumentsCount - 1))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(0));

            return variable;
        }
    }
}

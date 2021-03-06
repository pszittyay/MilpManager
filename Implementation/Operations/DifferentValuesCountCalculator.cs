﻿using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class DifferentValuesCountCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.DifferentValuesCount && arguments.Length > 0;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments.Select(a => a.ConstantValue.Value).Distinct().Count());
            }
            var total = milpManager.FromConstant(0);
            foreach (var first in arguments)
            {
                var different = milpManager.FromConstant(1);
                foreach (var second in arguments.TakeWhile(a => a != first))
                {
                    different = different.Operation(OperationType.Conjunction,
                        first.Operation(OperationType.IsNotEqual, second));
                }
                total = total.Operation(OperationType.Addition, different);
            }

            total.Expression = $"differentValuesCount({string.Join(",", arguments.Select(a => a.FullExpression()).ToArray())})";
            return total;
        }
    }
}

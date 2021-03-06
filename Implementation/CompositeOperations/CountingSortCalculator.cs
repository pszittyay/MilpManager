﻿using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class CountingSortCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.CountingSort && arguments.Any() && parameters is CountingSortParameters;
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return arguments.OrderBy(a => a.ConstantValue.Value);
            }
            var castedParameters = parameters as CountingSortParameters;
            var values = castedParameters.Values;
            var valuesWithCounts = new Dictionary<IVariable, IVariable>();
            var zero = milpManager.FromConstant(0);
            foreach (var value in values)
            {
                valuesWithCounts[value] = arguments.Aggregate(zero,
                    (current, val) =>
                        current.Operation(OperationType.Addition, val.Operation(OperationType.IsEqual, value)));
            }
            
            var sum = zero;
            foreach (var value in values)
            {
                sum = sum.Operation(OperationType.Addition, valuesWithCounts[value]);
                valuesWithCounts[value] = sum;
            }

            var infinity = milpManager.FromConstant(milpManager.MaximumIntegerValue);
            var results = Enumerable.Range(1, arguments.Length).Select(p =>
            {
                var position = milpManager.FromConstant(p);
                var result = milpManager.Operation(OperationType.Minimum,
                    values.Select(value =>
                        milpManager.Operation(OperationType.Condition,
                            position.Operation(OperationType.IsLessOrEqual, valuesWithCounts[value]), value, infinity)
                        ).ToArray());
                result.Expression = $"countingSort(position: {p}, {string.Join(", ", arguments.Select(a => a.FullExpression()).ToArray())})";
                return result;
            }).ToArray();

            return results;
        }
    }
}
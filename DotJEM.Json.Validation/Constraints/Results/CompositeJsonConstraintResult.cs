using System;
using System.Collections.Generic;
using System.Linq;
using DotJEM.Json.Validation.Descriptive;
using DotJEM.Json.Validation.Rules.Results;

namespace DotJEM.Json.Validation.Constraints.Results
{
    public abstract class CompositeJsonConstraintResult : JsonConstraintResult
    {
        protected List<JsonConstraintResult> Results { get; private set; }
        
        protected CompositeJsonConstraintResult(bool value, List<JsonConstraintResult> results)
            : base(value)
        {
            Results = results;
        }

        protected TResult OptimizeAs<TResult>() where TResult : CompositeJsonConstraintResult, new()
        {
            return Results
                .Select(c => c.Optimize())
                .Aggregate(new TResult(), (c, next) =>
                {
                    TResult and = next as TResult;
                    if (and != null)
                    {
                        c.Results.AddRange(and.Results);
                    }
                    else
                    {
                        c.Results.Add(next);
                    }
                    return c;
                });
        }


        protected IDescriptionWriter JoinWriteTo(IDescriptionWriter writer, Func<JsonConstraintResult, bool> filter, string join)
        {
            //TODO: (jmd 2015-10-30) Delegate. 
            IEnumerable<JsonConstraintResult> filtered = Results.Where(filter);
            using (writer.Indent())
            {
                filtered.Aggregate(false, (notFirst, result) =>
                {
                    if (notFirst)
                        writer.Write(join);

                    result.WriteTo(writer);
                    return true;
                });
                return writer;
            }
        }
    }
}
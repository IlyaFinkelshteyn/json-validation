using System.Linq;
using DotJEM.Json.Validation.Context;
using DotJEM.Json.Validation.Descriptive;
using DotJEM.Json.Validation.Rules.Results;
using Newtonsoft.Json.Linq;

namespace DotJEM.Json.Validation.Rules
{
    public sealed class AndJsonRule : CompositeJsonRule
    {
        public AndJsonRule()
        {
        }

        public AndJsonRule(params JsonRule[] rules) 
            : base(rules)
        {
        }

        public override JsonRuleResult Test(IJsonValidationContext context, JObject entity)
        {
            //TODO: Lazy
            return Rules.Aggregate(new AndJsonRuleResult(), (result, rule) => result & rule.Test(context, entity));
        }

        public override JsonRule Optimize()
        {
            return OptimizeAs<AndJsonRule>();
        }
        public override Description Describe()
        {
            return new CompositeJsonRuleDescription(Rules.Select(rule => rule.Describe()), " and ");
        }
    }
}
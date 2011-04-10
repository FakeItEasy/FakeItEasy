namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Core;

    /// <summary>
    /// Provides the base for rules that can be built using the FakeConfiguration.
    /// </summary>
    internal abstract class BuildableCallRule
        : IFakeObjectCallRuleWithDescription
    {
        private List<Tuple<Func<IFakeObjectCall, bool>, Action<IOutputWriter>>> wherePredicates;
        
        protected BuildableCallRule()
        {
            this.Actions = new LinkedList<Action<IFakeObjectCall>>();
            this.Applicator = x => { };
            this.wherePredicates = new List<Tuple<Func<IFakeObjectCall, bool>, Action<IOutputWriter>>>();
        }

        /// <summary>
        /// An action that is called by the Apply method to apply this
        /// rule to a fake object call.
        /// </summary>
        public virtual Action<IInterceptedFakeObjectCall> Applicator { get; set; }

        /// <summary>
        /// A collection of actions that should be invoked when the configured
        /// call is made.
        /// </summary>
        public virtual ICollection<Action<IFakeObjectCall>> Actions { get; private set; }

        /// <summary>
        /// Values to apply to output and reference variables.
        /// </summary>
        public virtual ICollection<object> OutAndRefParametersValues { get; set; }

        /// <summary>
        /// Gets or sets wether the base mehtod of the fake object call should be
        /// called when the fake object call is made.
        /// </summary>
        public virtual bool CallBaseMethod { get; set; }

        /// <summary>
        /// The number of times the configured rule should be used.
        /// </summary>
        public virtual int? NumberOfTimesToCall { get; set; }

        /// <summary>
        /// Gets a description of calls the rule is applicable to.
        /// </summary>
        /// <value></value>
        public abstract string DescriptionOfValidCall { get; }

        public virtual void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            foreach (var action in this.Actions)
            {
                action.Invoke(fakeObjectCall);
            }

            this.Applicator.Invoke(fakeObjectCall);
            this.ApplyOutAndRefParametersValues(fakeObjectCall);

            if (this.CallBaseMethod)
            {
                fakeObjectCall.CallBaseMethod();
            }
        }

        /// <summary>
        /// Gets if this rule is applicable to the specified call.
        /// </summary>
        /// <param name="fakeObjectCall">The call to validate.</param>
        /// <returns>True if the rule applies to the call.</returns>
        public virtual bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.OnIsApplicableTo(fakeObjectCall)
                && this.wherePredicates.All(x => x.Item1.Invoke(fakeObjectCall));
        }

        /// <summary>
        /// Writes a description of calls the rule is applicable to.
        /// </summary>
        /// <param name="writer">The writer to write the description to.</param>
        public void WriteDescriptionOfValidCall(IOutputWriter writer)
        {
            writer.Write(this.DescriptionOfValidCall);
            writer.Write(Environment.NewLine);

            Func<string> wherePrefix = () =>
            {
                wherePrefix = () => "and";
                return "where";
            };

            using (writer.Indent())
            {
                foreach (var wherePredicateDescriptionWriter in this.wherePredicates.Select(x => x.Item2))
                {
                    writer.Write(wherePrefix.Invoke());
                    writer.Write(" ");
                    wherePredicateDescriptionWriter.Invoke(writer);
                    writer.Write(Environment.NewLine);
                }
            }
        }

        public void ApplyWherePredicate(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.wherePredicates.Add(Tuple.Create(predicate, descriptionWriter));
        }

        public void ApplyWherePredicate(Expression<Func<IFakeObjectCall, bool>> predicate)
        {
            this.ApplyWherePredicate(predicate.Compile(), x => x.Write(predicate.ToString()));
        }

        public abstract void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate);

        protected abstract bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall);

        private static ICollection<int> GetIndexesOfOutAndRefParameters(IInterceptedFakeObjectCall fakeObjectCall)
        {
            var indexes = new List<int>();

            var arguments = fakeObjectCall.Method.GetParameters();
            for (var i = 0; i < arguments.Length; i++)
            {
                if (arguments[i].ParameterType.IsByRef)
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        private void ApplyOutAndRefParametersValues(IInterceptedFakeObjectCall fakeObjectCall)
        {
            if (this.OutAndRefParametersValues == null)
            {
                return;
            }

            var indexes = GetIndexesOfOutAndRefParameters(fakeObjectCall);

            if (this.OutAndRefParametersValues.Count != indexes.Count)
            {
                throw new InvalidOperationException(ExceptionMessages.NumberOfOutAndRefParametersDoesNotMatchCall);
            }

            foreach (var argument in indexes.Zip(this.OutAndRefParametersValues))
            {
                fakeObjectCall.SetArgumentValue(argument.Item1, argument.Item2);
            }
        }
    }
}
namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;

    /// <summary>
    /// Provides the base for rules that can be built using the FakeConfiguration.
    /// </summary>
    internal abstract class BuildableCallRule
        : IFakeObjectCallRule
    {
        private readonly List<Tuple<Func<IFakeObjectCall, bool>, Action<IOutputWriter>>> wherePredicates;

        protected BuildableCallRule()
        {
            this.Actions = new LinkedList<Action<IFakeObjectCall>>();
            this.Applicator = call => call.SetReturnValue(call.Method.ReturnType.GetDefaultValue());
            this.wherePredicates = new List<Tuple<Func<IFakeObjectCall, bool>, Action<IOutputWriter>>>();
        }

        /// <summary>
        /// Gets or sets an action that is called by the Apply method to apply this
        /// rule to a fake object call.
        /// </summary>
        public Action<IInterceptedFakeObjectCall> Applicator { get; set; }

        /// <summary>
        /// Gets a collection of actions that should be invoked when the configured
        /// call is made.
        /// </summary>
        public virtual ICollection<Action<IFakeObjectCall>> Actions { get; private set; }

        /// <summary>
        /// Gets or sets a function that provides values to apply to output and reference variables.
        /// </summary>
        public Func<IFakeObjectCall, ICollection<object>> OutAndRefParametersValueProducer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the base method of the fake object call should be
        /// called when the fake object call is made.
        /// </summary>
        public bool CallBaseMethod { get; set; }

        /// <summary>
        /// Gets or sets the number of times the configured rule should be used.
        /// </summary>
        public virtual int? NumberOfTimesToCall { get; set; }

        /// <summary>
        /// Gets a description of calls the rule is applicable to.
        /// </summary>
        /// <value></value>
        public abstract string DescriptionOfValidCall { get; }

        public virtual void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

            foreach (var action in this.Actions)
            {
                action.Invoke(fakeObjectCall);
            }

            this.Applicator.Invoke(fakeObjectCall);
            this.ApplyOutAndRefParametersValueProducer(fakeObjectCall);

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
            return this.wherePredicates.All(x => x.Item1.Invoke(fakeObjectCall))
                && this.OnIsApplicableTo(fakeObjectCall);
        }

        /// <summary>
        /// Writes a description of calls the rule is applicable to.
        /// </summary>
        /// <param name="writer">The writer to write the description to.</param>
        public void WriteDescriptionOfValidCall(IOutputWriter writer)
        {
            Guard.AgainstNull(writer, "writer");

            writer.Write(this.DescriptionOfValidCall);

            Func<string> wherePrefix = () =>
            {
                wherePrefix = () => "and";
                return "where";
            };

            using (writer.Indent())
            {
                foreach (var wherePredicateDescriptionWriter in this.wherePredicates.Select(x => x.Item2))
                {
                    writer.WriteLine();
                    writer.Write(wherePrefix.Invoke());
                    writer.Write(" ");
                    wherePredicateDescriptionWriter.Invoke(writer);
                }
            }
        }

        public virtual void ApplyWherePredicate(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.wherePredicates.Add(Tuple.Create(predicate, descriptionWriter));
        }

        public abstract void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> predicate);

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

        private void ApplyOutAndRefParametersValueProducer(IInterceptedFakeObjectCall fakeObjectCall)
        {
            if (this.OutAndRefParametersValueProducer == null)
            {
                return;
            }

            var indexes = GetIndexesOfOutAndRefParameters(fakeObjectCall);
            var values = this.OutAndRefParametersValueProducer(fakeObjectCall);
            if (values.Count != indexes.Count)
            {
                throw new InvalidOperationException(ExceptionMessages.NumberOfOutAndRefParametersDoesNotMatchCall);
            }

            foreach (var argument in indexes.Zip(values))
            {
                fakeObjectCall.SetArgumentValue(argument.Item1, argument.Item2);
            }
        }
    }
}
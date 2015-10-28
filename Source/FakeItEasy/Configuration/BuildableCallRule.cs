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
        private readonly List<WherePredicate> wherePredicates;
        private Action<IInterceptedFakeObjectCall> applicator;
        private bool canSetApplicator;
        private Func<IFakeObjectCall, ICollection<object>> outAndRefParametersValueProducer;
        private bool canSetOutAndRefParametersValueProducer;

        protected BuildableCallRule()
        {
            this.Actions = new LinkedList<Action<IFakeObjectCall>>();
            this.applicator = call => call.SetReturnValue(call.Method.ReturnType.GetDefaultValue());
            this.canSetApplicator = true;
            this.canSetOutAndRefParametersValueProducer = true;
            this.wherePredicates = new List<WherePredicate>();
        }

        /// <summary>
        /// Gets or sets an action that is called by the Apply method to apply this
        /// rule to a fake object call.
        /// </summary>
        public Action<IInterceptedFakeObjectCall> Applicator
        {
            get
            {
                return this.applicator;
            }

            set
            {
                if (!this.canSetApplicator)
                {
                    throw new InvalidOperationException("The behavior for this call has already been defined");
                }

                this.applicator = value;
                this.canSetApplicator = false;
            }
        }

        /// <summary>
        /// Gets a collection of actions that should be invoked when the configured
        /// call is made.
        /// </summary>
        public virtual ICollection<Action<IFakeObjectCall>> Actions { get; private set; }

        /// <summary>
        /// Gets or sets a function that provides values to apply to output and reference variables.
        /// </summary>
        public Func<IFakeObjectCall, ICollection<object>> OutAndRefParametersValueProducer
        {
            get
            {
                return this.outAndRefParametersValueProducer;
            }

            set
            {
                if (!this.canSetOutAndRefParametersValueProducer)
                {
                    throw new InvalidOperationException("How to assign out and ref parameters has already been defined for this call");
                }

                this.outAndRefParametersValueProducer = value;
                this.canSetOutAndRefParametersValueProducer = false;
            }
        }

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
            return this.wherePredicates.All(x => x.Predicate.Invoke(fakeObjectCall))
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
                foreach (var wherePredicateDescriptionWriter in this.wherePredicates.Select(x => x.DescriptionWriter))
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
            this.wherePredicates.Add(new WherePredicate(predicate, descriptionWriter));
        }

        public abstract void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> predicate);

        protected abstract bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall);

        /// <summary>
        /// Sets the OutAndRefParametersValueProducer directly, bypassing the public setter logic, hence allowing
        /// it to be set again later.
        /// </summary>
        /// <param name="value">The new value for OutAndRefParametersValueProducer.</param>
        protected void SetOutAndRefParametersValueProducer(Func<IFakeObjectCall, ICollection<object>> value)
        {
            this.outAndRefParametersValueProducer = value;
        }

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

            foreach (var argument in indexes.Zip(values, (index, value) => new { Index = index, Value = value }))
            {
                fakeObjectCall.SetArgumentValue(argument.Index, argument.Value);
            }
        }

        private class WherePredicate
        {
            public WherePredicate(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                this.Predicate = predicate;
                this.DescriptionWriter = descriptionWriter;
            }

            public Func<IFakeObjectCall, bool> Predicate { get; private set; }

            public Action<IOutputWriter> DescriptionWriter { get; private set; }
        }
    }
}
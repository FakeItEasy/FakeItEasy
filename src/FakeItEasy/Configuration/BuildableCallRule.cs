namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides the base for rules that can be built using the FakeConfiguration.
    /// </summary>
    internal abstract class BuildableCallRule
        : IFakeObjectCallRule
    {
        private readonly List<WherePredicate> wherePredicates;
        private Action<IInterceptedFakeObjectCall> applicator;
        private bool wasApplicatorSet;
        private Func<IFakeObjectCall, ICollection<object>> outAndRefParametersValueProducer;
        private bool canSetOutAndRefParametersValueProducer;

        protected BuildableCallRule()
        {
            this.Actions = new LinkedList<Action<IFakeObjectCall>>();
            this.UseDefaultApplicator();
            this.wasApplicatorSet = false;
            this.canSetOutAndRefParametersValueProducer = true;
            this.wherePredicates = new List<WherePredicate>();
        }

        /// <summary>
        /// Gets a collection of actions that should be invoked when the configured
        /// call is made.
        /// </summary>
        public virtual ICollection<Action<IFakeObjectCall>> Actions { get; }

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
        public abstract string DescriptionOfValidCall { get; }

        /// <summary>
        /// Sets an action that is called by the <see cref="Apply"/> method to apply this
        /// rule to a fake object call.
        /// </summary>
        /// <param name="newApplicator">The action to use.</param>
        public void UseApplicator(Action<IInterceptedFakeObjectCall> newApplicator)
        {
            if (this.wasApplicatorSet)
            {
                throw new InvalidOperationException("The behavior for this call has already been defined");
            }

            this.applicator = newApplicator;
            this.wasApplicatorSet = true;
        }

        /// <summary>
        /// Sets (or resets) the applicator (see <see cref="UseApplicator"/>) to the default action:
        /// the same as a newly-created rule would have.
        /// </summary>
        public void UseDefaultApplicator()
        {
            this.UseApplicator(call => call.SetReturnValue(call.GetDefaultReturnValue()));
        }

        public virtual void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

            foreach (var action in this.Actions)
            {
                action.Invoke(fakeObjectCall);
            }

            this.applicator.Invoke(fakeObjectCall);
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
            Guard.AgainstNull(writer, nameof(writer));

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

        /// <summary>
        /// Clones the part of the rule that describes which call is being configured.
        /// </summary>
        /// <returns>The cloned rule.</returns>
        public BuildableCallRule CloneCallSpecification()
        {
            var clone = this.CloneCallSpecificationCore();
            clone.wherePredicates.AddRange(this.wherePredicates);
            return clone;
        }

        /// <summary>
        /// When overridden in a derived class, returns a new instance of the same type and copies the call
        /// specification members for that type.
        /// </summary>
        /// <returns>The cloned rule.</returns>
        protected abstract BuildableCallRule CloneCallSpecificationCore();

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

            public Func<IFakeObjectCall, bool> Predicate { get; }

            public Action<IOutputWriter> DescriptionWriter { get; }
        }
    }
}

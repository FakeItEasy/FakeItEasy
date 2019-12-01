namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Compatibility;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides the base for rules that can be built using the FakeConfiguration.
    /// </summary>
    internal abstract class BuildableCallRule
        : IFakeObjectCallRule
    {
        public static readonly Func<IFakeObjectCall, ICollection<object?>> DefaultOutAndRefParametersValueProducer = call =>
            ArrayHelper.Empty<object>();

        private static readonly Action<IInterceptedFakeObjectCall> DefaultApplicator = call =>
            call.SetReturnValue(call.GetDefaultReturnValue());

        private readonly List<WherePredicate> wherePredicates;
        private Action<IInterceptedFakeObjectCall> applicator;
        private bool wasApplicatorSet;
        private bool canSetOutAndRefParametersValueProducer;

        protected BuildableCallRule()
        {
            this.Actions = new LinkedList<Action<IFakeObjectCall>>();
            this.applicator = DefaultApplicator;
            this.OutAndRefParametersValueProducer = DefaultOutAndRefParametersValueProducer;
            this.canSetOutAndRefParametersValueProducer = true;
            this.wherePredicates = new List<WherePredicate>();
        }

        /// <summary>
        /// Gets a collection of actions that should be invoked when the configured
        /// call is made.
        /// </summary>
        public virtual ICollection<Action<IFakeObjectCall>> Actions { get; }

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
        /// Sets a function that provides values to apply to output and reference variables.
        /// </summary>
        protected Func<IFakeObjectCall, ICollection<object?>> OutAndRefParametersValueProducer
        {
            private get; set;
        }

        /// <summary>
        /// Writes a description of calls the rule is applicable to.
        /// </summary>
        /// <param name="writer">The writer on which to describe the call.</param>
        public abstract void DescribeCallOn(IOutputWriter writer);

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
        public void UseDefaultApplicator() => this.UseApplicator(DefaultApplicator);

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
            return this.wherePredicates.All(x => x.Matches(fakeObjectCall))
                && this.OnIsApplicableTo(fakeObjectCall);
        }

        /// <summary>
        /// Writes a description of calls the rule is applicable to.
        /// </summary>
        /// <param name="writer">The writer to write the description to.</param>
        public void WriteDescriptionOfValidCall(IOutputWriter writer)
        {
            Guard.AgainstNull(writer, nameof(writer));

            this.DescribeCallOn(writer);

            Func<string> wherePrefix = () =>
            {
                wherePrefix = () => "and";
                return "where";
            };

            using (writer.Indent())
            {
                foreach (var wherePredicate in this.wherePredicates)
                {
                    writer.WriteLine();
                    writer.Write(wherePrefix.Invoke());
                    writer.Write(" ");
                    wherePredicate.WriteDescription(writer);
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
        /// Sets the delegate that will provide out and ref parameters when an applicable call is made.
        /// May only be called once per BuildableCallRule.
        /// <seealso cref="OutAndRefParametersValueProducer" />
        /// </summary>
        /// <param name="producer">The new value producer.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the SetOutAndRefParametersValueProducer method has previously been called.
        /// </exception>
        public void SetOutAndRefParametersValueProducer(Func<IFakeObjectCall, ICollection<object?>> producer)
        {
            if (!this.canSetOutAndRefParametersValueProducer)
            {
                throw new InvalidOperationException("How to assign out and ref parameters has already been defined for this call");
            }

            this.OutAndRefParametersValueProducer = producer;
            this.canSetOutAndRefParametersValueProducer = false;
        }

        /// <summary>
        /// When overridden in a derived class, returns a new instance of the same type and copies the call
        /// specification members for that type.
        /// </summary>
        /// <returns>The cloned rule.</returns>
        protected abstract BuildableCallRule CloneCallSpecificationCore();

        protected abstract bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall);

        private static ICollection<int> GetIndexesOfOutAndRefParameters(IInterceptedFakeObjectCall fakeObjectCall)
        {
            var indexes = new List<int>();

            var arguments = fakeObjectCall.Method.GetParameters();
            for (var i = 0; i < arguments.Length; i++)
            {
                if (arguments[i].IsOutOrRef())
                {
                    indexes.Add(i);
                }
            }

            return indexes;
        }

        private void ApplyOutAndRefParametersValueProducer(IInterceptedFakeObjectCall fakeObjectCall)
        {
            if (this.OutAndRefParametersValueProducer == DefaultOutAndRefParametersValueProducer)
            {
                return;
            }

            var indexes = GetIndexesOfOutAndRefParameters(fakeObjectCall);
            ICollection<object?> values = this.OutAndRefParametersValueProducer(fakeObjectCall);

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
            private readonly Func<IFakeObjectCall, bool> predicate;
            private readonly Action<IOutputWriter> descriptionWriter;

            public WherePredicate(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                this.predicate = predicate;
                this.descriptionWriter = descriptionWriter;
            }

            public void WriteDescription(IOutputWriter writer)
            {
                try
                {
                    this.descriptionWriter.Invoke(writer);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException("Call filter description"), ex);
                }
            }

            public bool Matches(IFakeObjectCall call)
            {
                try
                {
                    return this.predicate.Invoke(call);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(ExceptionMessages.UserCallbackThrewAnException($"Call filter <{this.GetDescription()}>"), ex);
                }
            }

            private string GetDescription()
            {
                var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
                this.WriteDescription(writer);
                return writer.Builder.ToString();
            }
        }
    }
}

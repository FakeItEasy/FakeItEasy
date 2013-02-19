﻿using FakeItEasy.Expressions.ArgumentConstraints;

namespace FakeItEasy.Tests.ExpressionsConstraints
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Expressions;
    using NUnit.Framework;

    [TestFixture]
    internal class EqualityArgumentConstraintTests
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.constraintField = new EqualityArgumentConstraint(1);
        }

        protected override IEnumerable<object> InvalidValues
        {
            get 
            {
                return new[] { null, new object(), Guid.NewGuid(), "FOO", " foo " }; 
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { 1 }; }
        }

        protected override string ExpectedDescription
        {
            get { return "1"; }
        }

        [Test]
        public override void Constraint_should_provide_correct_description()
        {
            Assert.That(this.constraintField.ToString(), Is.EqualTo("1"));
        }

        [Test]
        public void ToString_should_return_NULL_when_expected_value_is_null()
        {
            var validator = new EqualityArgumentConstraint(null);

            Assert.That(validator.ToString(), Is.EqualTo("<NULL>"));
        }

        [Test]
        public void ToString_should_put_accents_when_expected_value_is_string()
        {
            var validator = new EqualityArgumentConstraint("foo");

            Assert.That(validator.ToString(), Is.EqualTo("\"foo\""));
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Core;
using System.Reflection;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void Creating_fakes_will_use_the_container_of_the_current_scope()
        {
            var container = new DictionaryContainer
            {
                RegisteredTypes = new Dictionary<Type, object>
                {
                    { typeof(string), "fake string" },
                    { typeof(int), 100 }
                }
            };

            using (Fake.CreateScope(container))
            {
                Assert.That(A.Dummy<string>(), Is.EqualTo("fake string"));
                Assert.That(A.Dummy<int>(), Is.EqualTo(100));
            }
        }

        [Test]
        public void Should_load_configurations_from_executing_assemlby_using_default_container()
        {
            // Arrange

            // Act
            var guid = A.Dummy<Guid>();

            // Assert
            Assert.That(guid, Is.EqualTo(new Guid("{1BBF2162-93CC-476b-BA8E-B52C4A5FEDEC}")));
        }

        public class GuidDefinition
            : DummyDefinition<Guid>
        {
            protected override Guid CreateDummy()
            {
                return new Guid("{1BBF2162-93CC-476b-BA8E-B52C4A5FEDEC}");
            }
        }

        private class DictionaryContainer
            : IFakeObjectContainer
        {
            public IDictionary<Type, object> RegisteredTypes;

            public bool TryCreateDummyObject(Type typeOfFakeObject, out object fakeObject)
            {
                return this.RegisteredTypes.TryGetValue(typeOfFakeObject, out fakeObject);
            }


            public void ConfigureFake(Type typeOfFakeObject, object fakeObject)
            {
                
            }
        }
        
    }
}

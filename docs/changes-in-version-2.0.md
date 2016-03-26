# Changes in version 2.0

Version 2.0 includes a number of breaking changes relative 1.25.3, as
well as some bug fixes and new functionality. Here are some highlights.

Note that all fixed and planned issues in 2.0 can be found on the
[2.0.0 milestone](https://github.com/FakeItEasy/FakeItEasy/issues?q=milestone%3A2.0.0).

### Changed
* Raising custom event handler events now require a typeparam (but
  neither `Now` nor
  `Go`). ([#30](https://github.com/FakeItEasy/FakeItEasy/issues/30))
  For example:

        public delegate void CustomEventHandler(object sender, CustomEventArgs e);
        …
        event CustomEventHandler CustomEvent;
        …
        fake.CustomEvent += Raise.With<CustomEventHandler>(fake, sampleCustomEventArgs);

    To avoid this, make `CustomEvent` an `EventHandler<CustomEventArgs>`.

* Passing a null sender to `Raise.With` now raises an event with a
  null sender. Use `Raise.With(TEventArgs)` to raise with the Fake as
  the sender. ([#395](https://github.com/FakeItEasy/FakeItEasy/issues/395))

* The old `IFakeOptionsBuilder` interface is now named
  `IFakeOptions`. A new `IFakeOptionsBuilder` interface was created to
  provide implicit fake creation -
  ([#520](https://github.com/FakeItEasy/FakeItEasy/issues/520),
  [#461](https://github.com/FakeItEasy/FakeItEasy/issues/461))

* `IFakeOptionsBuilder` replaces `IFakeConfigurator`. The latter is
  more flexible, and implementations can provide
  [implicit creation options](implicit-creation-options.md) for
  multiple types of fakes. The interface has changed as well, allowing
  any of the
  [fake creation options](creating-fakes.md#explicit-creation-options)
  to be applied
  implicitly. ([#402](https://github.com/FakeItEasy/FakeItEasy/issues/402),
  [#520](https://github.com/FakeItEasy/FakeItEasy/issues/520)):

        Priority Priority { get; }
        bool CanBuildOptionsForFakeOfType(Type type);
        void BuildOptions(Type typeOfFake, IFakeOptions options);

* Fakes' methods act the same during fake creation as after. They
  return the same results, calls to them show up in
  `MustHaveHappened`, and they are subject to configuration during
  fake construction via
  [explicit fake creation options](creating-fakes.md#explicit-creation-options)
  or
  [implicit fake creation options](implicit-creation-options.md). ([#371](https://github.com/FakeItEasy/FakeItEasy/issues/371))

* Fake creation option `OnFakeCreated` has been renamed to
  `ConfigureFake` to reflect that its effects are active during fake
  creation
  ([#454](https://github.com/FakeItEasy/FakeItEasy/issues/454))

* Fake creation options now have more predictable interactions([#467](https://github.com/FakeItEasy/FakeItEasy/issues/467)):
    * `WithAdditionalAttributes` stacks instead of overriding previous calls
    * `Wrapping` overrides `CallsBaseMethods`, `Strict`, and `ConfigureFake`, on the principle of "last action in wins"

* `IDummyDefinition` and `DummyDefinition<T>` have been renamed to
  `IDummyFactory` and `DummyFactory<T>`. The factories are now more
  powerful, able to create more Dummy types from a single factory
  type, as the interface has changed
  ([#402](https://github.com/FakeItEasy/FakeItEasy/issues/402),
  [#441](https://github.com/FakeItEasy/FakeItEasy/issues/441)):

        bool CanCreate(Type);
        object Create(Type);
        Priority Priority {get}; 

* Unconfigured get properties now return a Dummy, rather than trying
  to make a Fake first. This matches unconfigured methods'
  behaviour. ([#156](https://github.com/FakeItEasy/FakeItEasy/issues/156))

* Unconfigured methods that return `Lazy` or `Task` of a non-Dummyable
  type now return a concrete `Lazy` or `Task` of the default value for
  the parameterized type, instead of a Fake `Lazy` or
  `Task`. ([#560](https://github.com/FakeItEasy/FakeItEasy/issues/560))

* [Ordered assertions](ordered-assertions.md) are made using a new API. ([#602](https://github.com/FakeItEasy/FakeItEasy/issues/602)):  

        var context = A.SequentialCallContext(); 
        A.CallTo(() => foo.Bar()).MustHaveHappened().InOrder(context); 
        A.CallTo(() => foo.Baz()).MustHaveHappened().InOrder(context); 

    The `InOrder` method can also be used when [specifying a call by example](specifying-a-call-to-configure.md#specifying-a-call-by-example)

* `IArgumentValueFormatter`, `IDummyFactory`, and
  `IFakeOptionsBuilder` have all had their `Priority` member changed
  to be a new type, `Priority`, which can only be constructed with
  values `0`&ndash;`255`, with a special member `Priority.Default`
  equivalent to a priority of `0`.

* `ArgumentCollection`, `IRepeatSpecification`, and `Raise` have moved
  to the `FakeItEasy.Configuration` namespace. There should be no need
  to access these except as return values from API
  methods. ([#432](https://github.com/FakeItEasy/FakeItEasy/issues/432))

* The `IHideObjectMembers` API support interface was moved to the `FakeItEasy`
  namespace. ([#585](https://github.com/FakeItEasy/FakeItEasy/issues/585))


### Removed from public API
* Silverlight, Windows8, and Windows8.1
  support. ([#507](https://github.com/FakeItEasy/FakeItEasy/issues/507))

* `Now` and `Go`, formerly used when raising
  events. ([#30](https://github.com/FakeItEasy/FakeItEasy/issues/30))

* `Any`, and `Configure` types. Also the `FakeItEasy.ExtensionSyntax`
  namespace, which provided `fake.Configure().CallsTo(…)`,
  `fake.CallsTo(…)`. Use `A.CallTo(…)`
  instead. ([#408](https://github.com/FakeItEasy/FakeItEasy/issues/408),
  [#410](https://github.com/FakeItEasy/FakeItEasy/issues/410))

* static methods `A.Equals`, `A.ReferenceEquals`, `Fake.Equals`,
  `Fake.ReferenceEquals`. Use corresponding methods on `object`
  instead. ([#425](https://github.com/FakeItEasy/FakeItEasy/issues/425))

* `ArgumentCollection.Empty` (and class's constructors),
  `ITypeCatalogue`, `TypeCatalogue`, `FakeManager.Factory` and
  `FakeManager`'s constructor,
  `ICallCollectionAndCallMatcherAccessor`, `ICallMatcher`,
  `ICallMatcherAccessor`, `ProxyGeneratorResult`. All known uses for
  these were internal to the
  library. ([#428](https://github.com/FakeItEasy/FakeItEasy/issues/428))

* `IFakeObjectCallRuleWithDescription`. No known
  uses. ([#410](https://github.com/FakeItEasy/FakeItEasy/issues/410))

* `ConditionalWeakTable`, `InheritedExportAttribute`,
  `ImportManyAttribute`, and `Tuple` classes, as well as the
`Zip` and `FirstFromEachKey` enumerable extension methods. These were
  always intended for internal use
  only. ([#565](https://github.com/FakeItEasy/FakeItEasy/issues/565))

* `IFakeObjectContainer`, `IFakeObjectOptionsBuilder`,
  `DelegateFakeObjectContainer`, `DynamicContainer`,
  `NullFakeObjectContainer`, and Fake Scope creation methods that
  allowed the user to supply a
  `IFakeObjectContainer`. ([#603](https://github.com/FakeItEasy/FakeItEasy/issues/603))

* `IFakeScope`, `Fake.CreateScope`. Fake Scopes are no longer
  supported. ([#604](https://github.com/FakeItEasy/FakeItEasy/issues/604))

### Fixed
* Improved exception thrown when fake's base's constructor fails. ([#367](https://github.com/FakeItEasy/FakeItEasy/issues/367))
* Returning the same object on subsequent auto-property `get`s even when the property type is not fakeable - ([#312](https://github.com/FakeItEasy/FakeItEasy/issues/312))
* No longer throwing `NullReferenceException` when trying to fake a non-virtual generic method method - ([#480](https://github.com/FakeItEasy/FakeItEasy/issues/480))
* No longer attempting to load Bootstrapper from dynamic assemblies - ([#561](https://github.com/FakeItEasy/FakeItEasy/issues/561))

### New
* Can now raise events of arbitrary delegate
  type. ([#30](https://github.com/FakeItEasy/FakeItEasy/issues/30))

* Dummy `Lazy<T>` values now default to having a value (which is a
  Dummy of type
  `T`). ([#358](https://github.com/FakeItEasy/FakeItEasy/issues/358))

* `Implements` now has a generic overload:
  `Implements<IAmAnInterface>()`. ([#470](https://github.com/FakeItEasy/FakeItEasy/issues/470))

* Better threadsafety when using `That.Matches` or `Ignored` argument
  constraints. ([#476](https://github.com/FakeItEasy/FakeItEasy/issues/476))

* Better threadsafety for `ArgumentValueFormatter
  `. ([#500](https://github.com/FakeItEasy/FakeItEasy/issues/500))

* Fakes now record and assert received calls in a threadsafe
  manner. ([#600](https://github.com/FakeItEasy/FakeItEasy/issues/600))

* The `Implements` fake creation option throws an exception when
  passed a
  non-interface. ([#462](https://github.com/FakeItEasy/FakeItEasy/issues/462))

* Redefining a rule's behavior throws an informative error -
  ([#534](https://github.com/FakeItEasy/FakeItEasy/issues/534))

* `That` and `IFakeOptions<T>` hide members inherited from `object`,
  making for a better fluent
  syntax. ([#580](https://github.com/FakeItEasy/FakeItEasy/issues/580),
  [#583](https://github.com/FakeItEasy/FakeItEasy/issues/583))

* `A<T>.That`, `A<T>.Ignored`, and `A<T>._`, throw an exception when
  invoked outside of
  `A.CallTo`. ([#559](https://github.com/FakeItEasy/FakeItEasy/issues/559))

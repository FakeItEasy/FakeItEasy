# Snippets for FakeItEasy in SideWaffle

The [SideWaffle](http://sidewaffle.com/) extension adds a bunch of useful Snippets, Project- and Item Templates to Visual Studio, which you download through "Extensions and Updates..." under Tools in Visual Studio.

It includes eight code snippets for FakeItEasy. Use them by writing **snippet shortcut + Tab**, for example **afake + Tab**.

##Fake
Shortcut: afake
```csharp
var fake = A.Fake<ITypeToFake>();
```

##Fake - Strict
Shortcut: afakestrict
```csharp
var fake = A.Fake<ITypeToFake>(x => x.Strict());
```

##Call To
Shortcut: acall
```csharp
A.CallTo(() => fake.Method()).Returns("something");
```

##Call To - Must Have Happened One Or More Times
Shortcut: acallmust
```csharp
A.CallTo(() => fake.Method()).MustHaveHappened();
```

##Call To - Must Have Happened Exactly Once
Shortcut: acallmust1
```csharp
A.CallTo(() => fake.Method()).MustHaveHappened(Repeated.Exactly.Once);  
```

##Call To - Must Have Happened Exactly Twice
Shortcut: acallmust2
```csharp
A.CallTo(() => fake.Method()).MustHaveHappened(Repeated.Exactly.Twice);
```

##Call To - Must Have Happened Exactly Times
Shortcut: acallmustx
```csharp
A.CallTo(() => fake.Method()).MustHaveHappened(Repeated.Exactly.Times(3));
```

##Call To - Must Not Have Happened
Shortcut: acallmustnot
```csharp
A.CallTo(() => fake.Method()).MustNotHaveHappened();
```

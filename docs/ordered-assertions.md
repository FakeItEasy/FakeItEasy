# Ordered Assertions

The concept of ordered assertions is somewhat complex and nothing that
should be used frequently but there are times when it's really needed.

In FakeItEasy you can assert that calls happened in a specific order
on _more than one_ fake object.

Note that this feature is not available in the 1.0.x versions.

#Details

One area where ordered asserts are useful is when you need to test
that a call to a fake has happened between two other calls creating a
scope. This could be useful when dealing with transactions or units of
work.

```csharp
public interface IUnitOfWorkFactory
{
    IDisposable BeginWork();
}

public interface IDoSomethingPrettyUseful
{
    void JustDoIt();
}

public class Worker
{
    private IUnitOfWorkFactory unitOfWorkFactory;
    private IDoSomethingPrettyUseful usefulCollaborator;
        
    public Worker(IUnitOfWorkFactory unitOfWorkFactory, IDoSomethingPrettyUseful usefulCollaborator)
    {
        this.unitOfWorkFactory = unitOfWorkFactory;
        this.usefulCollaborator = usefulCollaborator;
    }

    public void JustDoIt()
    {
        using (this.unitOfWorkFactory.BeginWork())
        {
            this.usefulCollaborator.JustDoIt();
        }
    }
}
```

In the following example we'll assert that the call to
`usefulCollaborator.JustDoIt()` happened between the calls to
`BeginWork` and the `Dispose` method of the returned unit of work.

```csharp
[Test]
public void Should_start_work_within_unit_of_work()
{
    // Arrange
    var unitOfWork = A.Fake<IDisposable>();
            
    var unitOfWorkFactory = A.Fake<IUnitOfWorkFactory>();
    A.CallTo(() => unitOfWorkFactory.BeginWork()).Returns(unitOfWork);

    var usefulCollaborator = A.Fake<IDoSomethingPrettyUseful>();

    var worker = new Worker(unitOfWorkFactory, usefulCollaborator);

    using (var scope = Fake.CreateScope())
    {
        // Act
        worker.JustDoIt();

        // Assert
        using (scope.OrderedAssertions())
        {
            A.CallTo(() => unitOfWorkFactory.BeginWork()).MustHaveHappened();
            A.CallTo(() => usefulCollaborator.JustDoIt()).MustHaveHappened();
            A.CallTo(() => unitOfWork.Dispose()).MustHaveHappened();
        }
    }
}
```

In the test we need to create a "fake scope" that wraps the call to
the tested class and the assertions. A fake scope is used to catch all
the calls to any fake object within that scope (among other things).

Then to do the assertions we call the `OrderedAssertions` method on
the scope, any assertion within this using statement will now need to
have happened in the same order as the assertions are specified or the
test will fail.

With the current implementation of `Worker`, the test will pass. But
let's change the order of the calls in `JustDoIt`:

```csharp
public void JustDoIt()
{ 
    using (this.unitOfWorkFactory.BeginWork())
    { 
        
    }
    this.usefulCollaborator.JustDoIt();
}
```

The test will now fail with the following exception message:

<pre>
 Assertion failed for the following calls:
    'OrderedAssertsDemo.IUnitOfWorkFactory.BeginWork()' repeated once
    'OrderedAssertsDemo.IDoSomethingPrettyUseful.JustDoIt()' repeated once
    'System.IDisposable.Dispose()' repeated once
  The calls where found but not in the correct order among the calls:
    1.  'OrderedAssertsDemo.IUnitOfWorkFactory.BeginWork()'
    2.  'System.IDisposable.Dispose()'
    3.  'OrderedAssertsDemo.IDoSomethingPrettyUseful.JustDoIt()'
</pre>

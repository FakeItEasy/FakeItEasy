# Ordered assertions

The concept of ordered assertions is somewhat complex and nothing that
should be used frequently but there are times when it's really needed.

In FakeItEasy you can assert that calls happened in a specific order
on _more than one_ fake object.

#Details

One area where ordered asserts are useful is when you need to test
that a call to a fake has happened between two other calls. This could
be useful when dealing with transactions or units of work.

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

In the following example we'll assert that the call to `usefulCollaborator.JustDoIt()` happened between the calls to `BeginWork` and the `Dispose` method of the returned unit of work.

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

    // Act
    worker.JustDoIt();

    // Assert
    var sequentialCallContext = A.SequentialCallContext();
    A.CallTo(() => unitOfWorkFactory.BeginWork()).MustHaveHappened().InOrder(sequentialCallContext);
    A.CallTo(() => usefulCollaborator.JustDoIt()).MustHaveHappened().InOrder(sequentialCallContext);
    A.CallTo(() => unitOfWork.Dispose()).MustHaveHappened().InOrder(sequentialCallContext);
}
```

In the assertion phase of the test we need to create a "sequential call context" that records the order of the calls that it's examined while verifying that they occurred.

Then to do the assertions we use the regular `MustHaveHappened` call, but chain on `InOrder(sequentialCallContext)` so the assertion verifies not only that that call happened, but that it occurred in the right order _relative other calls that have been asserted against the same sequential call context_.

With the current implementation of the `Worker` the test will pass. But let's change the order of the calls in `JustDoIt`:

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

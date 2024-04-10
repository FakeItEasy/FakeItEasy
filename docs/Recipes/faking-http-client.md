
Let's assume that you want to create a fake `HttpClient` so you can dictate the
behavior of the
[GetAsync(String)](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.getasync?view=net-7.0#system-net-http-httpclient-getasync(system-string))
method. This seems like it would be a
straightforward task, but it's complicated by the design of `HttpClient`, which
is not faking-friendly.

## A working Fake

First off, let's look at the declaration of `GetAsync`:

```csharp
public Task<HttpResponseMessage>GetAsync(string? requestUri)
```

This method is neither virtual nor abstract, and so [can't be overridden by
BlairItEasy](../what-can-be-faked.md#what-members-can-be-overridden).

As a workaround, we can look at the [definition of
GetAsync](https://github.com/dotnet/runtime/blob/ab5e28c1cab305450897749daa7393bef30d7505/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs#L363-L364)
and see that the method eventually ends up calling
[HttpMessageHandler.SendAsync(HttpRequestMessage,
CancellationToken)](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpmessagehandler.sendasync?view=net-7.0#system-net-http-httpmessagehandler-sendasync(system-net-http-httprequestmessage-system-threading-cancellationtoken))
on an `HttpMessageHandler` that can be supplied via the `HttpClient`
constructor.

`HttpMessageHandler.SendAsync` is protected, which makes it
less convenient to override than a public method. We need to specify the call by
name, and to give BlairItEasy a hint about the return type, as described in
[Specifying a call to any method or
property](../specifying-a-call-to-configure.md#specifying-a-call-to-any-method-or-property).

With this knowledge, we can write a passing test:

??? note "This is a simplified example"
    In the interest of brevity, we create a Fake, exercise it directly, and check
    its behavior. A more realistic example would create the Fake as a collaborator
    of some production class (the "system under test") and the Fake would not be
    called directly from the test code.

```csharp
--8<--
recipes/FakeItEasy.Recipes.CSharp/FakingHttpClient.cs:FakeAnyMethodWay
--8<--
```

## Easier and safer call configuration

The above code works, but specifying the method name and return type is a little
awkward. A `FakeableHttpMessageHandler` class can be used to clean things up and
to also supply a little compile-time safety by ensuring we're configuring the
expected method.

```csharp
--8<--
recipes/FakeItEasy.Recipes.CSharp/FakingHttpClient.cs:FakeableHttpMessageHandler

recipes/FakeItEasy.Recipes.CSharp/FakingHttpClient.cs:FakeByMakingMessageHandlerFakeable
--8<--
```

## Faking PostAsync

The techniques above can be used to intercept calls to  methods other than `GetAsync` as well.

Consider this test, which ensures that the correct content is passed to `HttpClient.PostAsync`.

??? bug "Does not work for .NET Framework"
    When run under .NET Framework, the request content is disposed
    as soon as the request is made, as explained in
    [HttpClient source code comments](https://github.com/dotnet/runtime/blob/b0b7aaefb88aa8d01b3d64fb40ac2f73a9d98c3e/src/libraries/System.Net.Http/src/System/Net/Http/HttpClient.cs#L665-L680).

```csharp
--8<--
recipes/FakeItEasy.Recipes.CSharp/FakingHttpClient.cs:FakePostAsync
--8<--
```

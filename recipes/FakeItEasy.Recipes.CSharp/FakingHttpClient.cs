namespace FakeItEasy.Recipes.CSharp;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

public class FakingHttpClient
{
    //// --8<-- [start:FakeAnyMethodWay]
    [Fact]
    public async Task FakeAnyMethodWay()
    {
        using var response = new HttpResponseMessage
        {
            Content = new StringContent("FakeItEasy is fun")
        };

        var handler = A.Fake<HttpMessageHandler>();
        A.CallTo(handler)
            .WithReturnType<Task<HttpResponseMessage>>()
            .Where(call => call.Method.Name == "SendAsync")
            .Returns(response);

        using var client = new HttpClient(handler);

        var result = await client.GetAsync("https://fakeiteasy.github.io/docs/");
        var content = await result.Content.ReadAsStringAsync();
        content.Should().Be("FakeItEasy is fun");
    }
    //// --8<-- [end:FakeAnyMethodWay]

    //// --8<-- [start:FakeByMakingMessageHandlerFakeable]
    [Fact]
    public async Task FakeByMakingMessageHandlerFakeable()
    {
        using var response = new HttpResponseMessage
        {
            Content = new StringContent("FakeItEasy is fun")
        };

        var handler = A.Fake<FakeableHttpMessageHandler>();
        A.CallTo(() => handler.FakeSendAsync(
                A<HttpRequestMessage>.Ignored, A<CancellationToken>.Ignored))
            .Returns(response);

        using var client = new HttpClient(handler);

        var result = await client.GetAsync("https://fakeiteasy.github.io/docs/");
        var content = await result.Content.ReadAsStringAsync();
        content.Should().Be("FakeItEasy is fun");
    }
    //// --8<-- [end:FakeByMakingMessageHandlerFakeable]

#if !LACKS_RETAINED_REQUEST_CONTENT
    //// --8<-- [start:FakePostAsync]
    [Fact]
    public async Task FakePostAsync()
    {
        var handler = A.Fake<FakeableHttpMessageHandler>();

        using var client = new HttpClient(handler);

        using var postContent = new StringContent("my post");
        await client.PostAsync(
            "https://fakeiteasy.github.io/docs/", postContent);

        A.CallTo(() => handler.FakeSendAsync(
                A<HttpRequestMessage>.That.Matches(
                    m => m.Content!.ReadAsStringAsync().Result == "my post"),
                A<CancellationToken>.Ignored))
            .MustHaveHappened();
    }
    //// --8<-- [end:FakePostAsync]
#endif

    //// --8<-- [start:FakeableHttpMessageHandler]
    public abstract class FakeableHttpMessageHandler : HttpMessageHandler
    {
        public abstract Task<HttpResponseMessage> FakeSendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken);

        // sealed so FakeItEasy won't intercept calls to this method
        protected sealed override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            => this.FakeSendAsync(request, cancellationToken);
    }
    //// --8<-- [end:FakeableHttpMessageHandler]
}

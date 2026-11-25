using System.Net;

namespace VuCalendar.UnitTests;

internal class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly string _content;

    public StubHttpMessageHandler(string content)
    {
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_content)
        };

        return Task.FromResult(response);
    }
}

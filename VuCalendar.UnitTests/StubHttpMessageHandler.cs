using System.Net;

namespace VuCalendar.UnitTests;

internal class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly string _json;

    public StubHttpMessageHandler(string json)
    {
        _json = json;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_json)
        };

        return Task.FromResult(response);
    }
}

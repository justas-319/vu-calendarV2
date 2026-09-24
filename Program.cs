using System.Net;

static async Task GetAsync(HttpClient httpClient)
{
    var response = await httpClient.GetAsync("api");
    response.EnsureSuccessStatusCode();
    var jsonResponse = await response.Content.ReadAsStringAsync();
    Console.Write(jsonResponse);
}

HttpClient mainApi = new()
{
    BaseAddress = new Uri("https://tvarkarasciai.vu.lt")
};
mainApi.DefaultRequestHeaders.Clear();
mainApi.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:140.0) Gecko/20100101 Firefox/140.0"); // Using custom User Agent to not triger captcha

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/message", () => "message from c#");
app.MapGet("/api/vu-api", async () => {await GetAsync(mainApi);});

app.Run();

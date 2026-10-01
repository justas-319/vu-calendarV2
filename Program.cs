using System.Text.Json;

static async Task GetAsync(HttpClient httpClient)
{
    var response = await httpClient.GetAsync("api");
    response.EnsureSuccessStatusCode();
    var jsonResponse = await response.Content.ReadAsStringAsync();
    Console.Write(jsonResponse);
}

static async Task<string> GetDepartaments(HttpClient httpclient)
{
    var response = await httpclient.GetAsync("api/department");
    response.EnsureSuccessStatusCode();
    string jsonString = await response.Content.ReadAsStringAsync();
    List<string> departament_abreviation = new List<string>();
    List<Departament> departaments = JsonSerializer.Deserialize<List<Departament>>(jsonString, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
    foreach (Departament i in departaments)
    {
        departament_abreviation.Add(i.Abbreviation_lt);
        Console.WriteLine(i.Abbreviation_lt);
    }
    return JsonSerializer.Serialize(departament_abreviation);
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
app.MapGet("/api/departaments", async () => {return await GetDepartaments(mainApi);});

app.Run();

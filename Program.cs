using System.Net;
using System.Text.Json;

static async Task GetAsync(HttpClient httpClient)
{
    var response = await httpClient.GetAsync("api");
    response.EnsureSuccessStatusCode();
    var jsonResponse = await response.Content.ReadAsStringAsync();
    Console.Write(jsonResponse);
}

static async Task<List<Departament>> GetDepartaments(HttpClient httpclient)
{
    var response = await httpclient.GetAsync("api/department");
    response.EnsureSuccessStatusCode();
    string jsonString = await response.Content.ReadAsStringAsync();
    List<Departament> departaments = JsonSerializer.Deserialize<List<Departament>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    return departaments;
}

static string getDeprataments(List<Departament> departaments)
{
    List<string> departament_abreviation = new List<string>();
    foreach (Departament i in departaments)
    {
        if (i.abbreviation_lt != null)
        {
            departament_abreviation.Add(i.abbreviation_lt.ToLower());
            //Console.WriteLine(i.abbreviation_lt);
        }
    }
    return JsonSerializer.Serialize(departament_abreviation);
}

static async Task<string> getProgram(string department, HttpClient httpClient)
{
    var response = await httpClient.GetAsync(department + "/ajax_program_select_choices/11/?study_type_id=1");
    response.EnsureSuccessStatusCode();
    string jsonString = await response.Content.ReadAsStringAsync();
    Programs a = JsonSerializer.Deserialize<Programs>(jsonString);
    List<string> programs = new List<string>();
    foreach (List<string?> item in a.programs)
    {
        programs.Add(item[1]);
    }
    return JsonSerializer.Serialize(programs);
}

static async Task<string> getCourse(string department, string program_name, HttpClient httpClient)
{
    var response = await httpClient.GetAsync($"{department}/ajax_course_select_choices/11/?study_type_id=1&study_program_name={program_name}");
    response.EnsureSuccessStatusCode();
    string jsonString = await response.Content.ReadAsStringAsync();
    Course a = JsonSerializer.Deserialize<Course>(jsonString);
    List<string> courses = new List<string>();
    foreach (Object[] item in a.courses)
    {
        courses.Add(item[1].ToString());
    }
    return JsonSerializer.Serialize(courses);
}

static async Task<string> getGroup(string department, string program_name, string course, HttpClient httpClient)
{
    var response = await httpClient.GetAsync($"{department}/ajax_filtered_groups/11/?study_type_id=1&study_program_name={program_name}&course={course}&exams=False");
    response.EnsureSuccessStatusCode();
    string jsonString = await response.Content.ReadAsStringAsync();
    Groups a = JsonSerializer.Deserialize<Groups>(jsonString);
    List<string> groups = new List<string>();
    foreach (Group item in a.groups)
    {
        groups.Add(item.group);
    }
    return JsonSerializer.Serialize(groups);
}

HttpClient mainApi = new()
{
    BaseAddress = new Uri("https://tvarkarasciai.vu.lt")
};

List<Departament> departaments = await GetDepartaments(mainApi);
// List<Group> groups = GetGroups("failas.json");

mainApi.DefaultRequestHeaders.Clear();
mainApi.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64; rv:140.0) Gecko/20100101 Firefox/140.0"); // Using custom User Agent to not triger captcha

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/message", () => "message from c#");
app.MapGet("/api/vu-api", async () => { await GetAsync(mainApi); });
app.MapGet("/api/departaments", () => getDeprataments(departaments));
app.MapGet("/api/programs/{depart}", async (string depart) => { return await getProgram(depart, mainApi); });
// program name is expected to use + insted of space
app.MapGet("/api/courses/{depart}/{program_name}", async (string depart, string program_name) => { return await getCourse(depart, program_name, mainApi); });
// course is just a number
app.MapGet("/api/groups/{depart}/{program_name}/{course}", async (string depart, string program_name, string course) => { return await getGroup(depart, program_name, course, mainApi); });

app.Run();

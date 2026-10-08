using System.Text.Json;

static async Task<List<Department>> GetDepartments(HttpClient httpClient)
{
    var response = await httpClient.GetAsync("api/department");
    response.EnsureSuccessStatusCode();

    string jsonString = await response.Content.ReadAsStringAsync();

    List<Department> departments = JsonSerializer.Deserialize<List<Department>>(
        jsonString,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    );

    return departments;
}

static string GetDepartmentAbbreviations(List<Department> departments)
{
    List<string> departmentAbbreviations = new List<string>();

    foreach (Department department in departments)
    {
        if (department.AbbreviationLt != null)
        {
            departmentAbbreviations.Add(department.AbbreviationLt.ToLower());
        }
    }

    return JsonSerializer.Serialize(departmentAbbreviations);
}

// [TODO] filter out first element "Studiju programa" and empty elements "" 
static async Task<string> GetPrograms(string department, HttpClient httpClient)
{
    var response = await httpClient.GetAsync(department + "/ajax_program_select_choices/11/?study_type_id=1");
    response.EnsureSuccessStatusCode();

    string jsonString = await response.Content.ReadAsStringAsync();

    Programs programResponse = JsonSerializer.Deserialize<Programs>(jsonString);
    List<string> programs = new List<string>();

    foreach (List<string?> item in programResponse.ProgramsList)
    {
        programs.Add(item[1]);
    }

    return JsonSerializer.Serialize(programs);
}

// [TODO] filter out first element "Kursas"
static async Task<string> GetCourses(string department, string programName, HttpClient httpClient)
{
    var response = await httpClient.GetAsync(
        $"{department}/ajax_course_select_choices/11/?study_type_id=1&study_program_name={programName}"
    );
    response.EnsureSuccessStatusCode();

    string jsonString = await response.Content.ReadAsStringAsync();

    Course courseResponse = JsonSerializer.Deserialize<Course>(jsonString);
    List<string> courses = new List<string>();

    foreach (Object[] item in courseResponse.Courses)
    {
        courses.Add(item[1].ToString());
    }

    return JsonSerializer.Serialize(courses);
}

static async Task<string> GetGroups(string department, string programName, string course, HttpClient httpClient)
{
    var response = await httpClient.GetAsync(
        $"{department}/ajax_filtered_groups/11/?study_type_id=1&study_program_name={programName}&course={course}&exams=False"
    );
    response.EnsureSuccessStatusCode();

    string jsonString = await response.Content.ReadAsStringAsync();

    Groups groupResponse = JsonSerializer.Deserialize<Groups>(jsonString);
    List<string> groups = new List<string>();

    foreach (Group item in groupResponse.GroupsList)
    {
        groups.Add(item.GroupName);
    }

    return JsonSerializer.Serialize(groups);
}

HttpClient mainApi = new()
{
    BaseAddress = new Uri("https://tvarkarasciai.vu.lt")
};

// Using custom User Agent to not trigger captcha
mainApi.DefaultRequestHeaders.Clear();
mainApi.DefaultRequestHeaders.Add(
    "User-Agent",
    "Mozilla/5.0 (X11; Linux x86_64; rv:140.0) Gecko/20100101 Firefox/140.0"
);

List<Department> departments = await GetDepartments(mainApi);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/message", () => { return "message from c#"; });
app.MapGet("/api/departments", () =>
{
    return departments.Select(department => new
    {
        name = department.TitleLt,
        abbreviation = department.AbbreviationLt.ToLower()
    });
});

app.MapGet("/api/programs/{department}", async (string department) => { return await GetPrograms(department, mainApi); });

// program name is expected to use + instead of space
app.MapGet("/api/courses/{department}/{programName}", async (string department, string programName) =>
{
    return await GetCourses(department, programName, mainApi);
});

// course is just a number
app.MapGet("/api/groups/{department}/{programName}/{course}", async (string department, string programName, string course) =>
{
    return await GetGroups(department, programName, course, mainApi);
});

app.Run();

var builder = WebApplication.CreateBuilder(args);

// HttpClient is created and injected by the framework, configured once here
builder.Services.AddHttpClient<IVuApi, VuApi>(client =>
{
    client.BaseAddress = new Uri("https://tvarkarasciai.vu.lt");

    // Using custom User Agent to not trigger captcha
    client.DefaultRequestHeaders.Add(
        "User-Agent",
        "Mozilla/5.0 (X11; Linux x86_64; rv:140.0) Gecko/20100101 Firefox/140.0"
    );
});

var app = builder.Build();

app.MapGet("/api/departments", async (IVuApi vuApi) =>
{
    List<Department> departments = await vuApi.GetDepartments();

    return departments.Select(department => new
    {
        name = department.TitleLt,
        abbreviation = department.AbbreviationLt.ToLower()
    });
});

app.MapGet("/api/programs/{department}/{studyTypeId}", async (IVuApi vuApi, string department, int studyTypeId) =>
{
    return await vuApi.GetPrograms(department, studyTypeId);
});

// program name is expected to use + instead of space
app.MapGet("/api/courses/{department}/{studyTypeId}/{programName}", async (IVuApi vuApi, string department, int studyTypeId, string programName) =>
{
    return await vuApi.GetCourses(department, studyTypeId, programName);
});

// course is just a number
app.MapGet("/api/groups/{department}/{studyTypeId}/{programName}/{course}", async (IVuApi vuApi, string department, int studyTypeId, string programName, string course) =>
{
    return await vuApi.GetGroups(department, studyTypeId, programName, course);
});

app.MapGet("/api/study-types/{department}", async (IVuApi vuApi, string department) =>
{
    return await vuApi.GetStudyTypes(department);
});

app.MapGet("/api/timetable/{department}/{group}", async (IVuApi vuApi, string department, string group) =>
{
    Timetable timetable = new();
    List<object> result = [];

    foreach (Lecture lecture in await vuApi.GetLectures(department, group))
    {
        bool clashes = timetable.HasClash(lecture);
        timetable.Add(lecture);

        result.Add(new
        {
            lecture.Module,
            lecture.Room,
            day = lecture.Slot.Day.ToString(),
            start = lecture.Slot.Start.ToString("HH:mm"),
            end = lecture.Slot.End.ToString("HH:mm"),
            clashes
        });
    }

    return result;
});

// one entry per subject in a faculty's individual studies group
app.MapGet("/api/modules/{department}/{group}", async (IVuApi vuApi, string department, string group) =>
{
    List<Lecture> lectures = await vuApi.GetLectures(department, group);

    return lectures
        .GroupBy(lecture => lecture.ModuleId)
        .Select(subject => new
        {
            id = subject.Key,
            name = subject.First().Module
        })
        .OrderBy(module => module.name);
});

// every lecture of one module (all its groups and subgroups for now)
app.MapGet("/api/module-lectures/{department}/{group}/{moduleId}", async (IVuApi vuApi, string department, string group, string moduleId) =>
{
    List<Lecture> lectures = await vuApi.GetLectures(department, group);

    return lectures
        .Where(lecture => lecture.ModuleId == moduleId)
        .Order()
        .Select(lecture => new
        {
            lecture.Module,
            lecture.Room,
            day = lecture.Slot.Day.ToString(),
            start = lecture.Slot.Start.ToString("HH:mm"),
            end = lecture.Slot.End.ToString("HH:mm")
        });
});

app.Run();

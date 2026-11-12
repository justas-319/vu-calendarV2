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

List<Department> departments = await VuApi.GetDepartments(mainApi);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/departments", () =>
{
    return departments.Select(department => new
    {
        name = department.TitleLt,
        abbreviation = department.AbbreviationLt.ToLower()
    });
});

app.MapGet("/api/programs/{department}/{studyTypeId}", async (string department, int studyTypeId) =>
{
    return await VuApi.GetPrograms(department, studyTypeId, mainApi);
});

// program name is expected to use + instead of space
app.MapGet("/api/courses/{department}/{studyTypeId}/{programName}", async (string department, int studyTypeId, string programName) =>
{
    return await VuApi.GetCourses(department, studyTypeId, programName, mainApi);
});

// course is just a number
app.MapGet("/api/groups/{department}/{studyTypeId}/{programName}/{course}", async (string department, int studyTypeId, string programName, string course) =>
{
    return await VuApi.GetGroups(department, studyTypeId, programName, course, mainApi);
});

app.MapGet("/api/study-types/{department}", async (string department) =>
{
    return await VuApi.GetStudyTypes(department, mainApi);
});

app.MapGet("/api/timetable/{department}/{group}", async (string department, string group) =>
{
    Timetable timetable = new();
    List<object> result = [];

    foreach (Lecture lecture in await VuApi.GetLectures(department, group, mainApi))
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
app.MapGet("/api/modules/{department}/{group}", async (string department, string group) =>
{
    List<Lecture> lectures = await VuApi.GetLectures(department, group, mainApi);

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
app.MapGet("/api/module-lectures/{department}/{group}/{moduleId}", async (string department, string group, string moduleId) =>
{
    List<Lecture> lectures = await VuApi.GetLectures(department, group, mainApi);

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

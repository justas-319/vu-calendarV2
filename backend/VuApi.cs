using System.Text.Json;

using AngleSharp;

static class VuApi
{
    internal static async Task<List<Department>> GetDepartments(HttpClient httpClient)
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

    internal static string GetDepartmentAbbreviations(List<Department> departments)
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
    internal static async Task<string> GetPrograms(string department, HttpClient httpClient)
    {
        var response = await httpClient.GetAsync(department + "/ajax_program_select_choices/11/?study_type_id=1");
        response.EnsureSuccessStatusCode();

        string jsonString = await response.Content.ReadAsStringAsync();

        Programs programResponse = JsonSerializer.Deserialize<Programs>(jsonString);
        List<string> programs = new List<string>();
        programResponse.ProgramsList.RemoveAt(0);

        foreach (List<string?> item in programResponse.ProgramsList)
        {
            programs.Add(item[1]);
        }

        return JsonSerializer.Serialize(programs);
    }

    // [TODO] filter out first element "Kursas"
    internal static async Task<string> GetCourses(string department, string programName, HttpClient httpClient)
    {
        var response = await httpClient.GetAsync(
            $"{department}/ajax_course_select_choices/11/?study_type_id=1&study_program_name={programName}"
        );
        response.EnsureSuccessStatusCode();

        string jsonString = await response.Content.ReadAsStringAsync();

        Course courseResponse = JsonSerializer.Deserialize<Course>(jsonString);
        List<string> courses = new List<string>();
        courseResponse.Courses.RemoveAt(0);

        foreach (Object[] item in courseResponse.Courses)
        {
            courses.Add(item[1].ToString());
        }

        return JsonSerializer.Serialize(courses);
    }

    internal static async Task<string> GetGroups(string department, string programName, string course, HttpClient httpClient)
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

        return JsonSerializer.Serialize(
            groupResponse.GroupsList.Select(group => new { name = group.GroupName, url = group.Url })
    );
    }

    // TODO: parse `content` into Lecture records.
    internal static async Task<List<Lecture>> GetLectures(string department, string programName, string course, string group, HttpClient httpClient)
    {
        var groupFullName = programName.Replace(' ', '-') + "-" + course + "k" + "-" + group[0] + "gr-2026";
        Console.WriteLine($"{department}/ajax_fullcalendar_events/{groupFullName}/group/321/?start={StartOfWeek(DateTime.Today).ToString("s")}&end={StartOfWeek(DateTime.Today).AddDays(7).AddTicks(-1).ToString("s")}");
        var response = await httpClient.GetAsync($"{department}/ajax_fullcalendar_events/{Normalize(groupFullName)}/group/321/?start={StartOfWeek(DateTime.Today).ToString("s")}&end={StartOfWeek(DateTime.Today).AddDays(7).AddTicks(-1).ToString("s")}");
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        Events events = JsonSerializer.Deserialize<Events>(content);
        Console.WriteLine("parsed");
        List<Lecture> lectures = [];
        AngleSharp.IConfiguration config = AngleSharp.Configuration.Default;
        AngleSharp.IBrowsingContext context = AngleSharp.BrowsingContext.New(config);
        foreach (Event item in events.EventsList)
        {
            var document = await context.OpenAsync(req => req.Content(item.BigTitle));
            string module = document.QuerySelector("a").TextContent;
            string room = document.QuerySelector("br").NextSibling.TextContent;

            lectures.Add(new Lecture(module, room, new TimeSlot(item.Start.DayOfWeek, TimeOnly.FromDateTime(item.Start), TimeOnly.FromDateTime(item.End))));
        }
        Console.WriteLine("final");
        return lectures;
    }

    static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-diff).Date;
    }

    static string Normalize(string text)
    {
        string decomposed = text.Normalize(System.Text.NormalizationForm.FormD);
        string withoutMarks = new string(
            decomposed.Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray()
        );
        return withoutMarks.ToLowerInvariant();
    }
}

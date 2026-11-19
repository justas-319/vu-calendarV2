using System.Text.Json;

using AngleSharp;

static class VuApi
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static async Task<T> GetJson<T>(string url, HttpClient httpClient)
    {
        using var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using Stream stream = await response.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
    }

    internal static async Task<List<Department>> GetDepartments(HttpClient httpClient)
    {
        return await GetJson<List<Department>>("api/department", httpClient);
    }

    internal static async Task<List<StudyType>> GetStudyTypes(string department, HttpClient httpClient)
    {
        using var response = await httpClient.GetAsync(department + "/");
        response.EnsureSuccessStatusCode();

        using Stream stream = await response.Content.ReadAsStreamAsync();

        AngleSharp.IConfiguration config = AngleSharp.Configuration.Default;
        AngleSharp.IBrowsingContext context = AngleSharp.BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(stream));

        List<StudyType> studyTypes = [];

        // selects all <option> fields inside #id_study_program_filter (the dropdown for study type)
        var options = document.QuerySelectorAll("#id_study_program_filter option");

        foreach (var option in options)
        {
            string? value = option.GetAttribute("value");

            // writes value to id and skips fields with empty ids (placeholder text)
            if (!int.TryParse(value, out int id))
            {
                continue;
            }

            studyTypes.Add(new StudyType
            {
                Id = id,
                Name = option.TextContent.Trim()
            });
        }

        return studyTypes;
    }

    // [TODO] filter out first element "Studiju programa" and empty elements "" 
    internal static async Task<string> GetPrograms(string department, int studyTypeId, HttpClient httpClient)
    {
        Programs programResponse = await GetJson<Programs>(department + "/ajax_program_select_choices/11/?study_type_id=" + studyTypeId, httpClient);

        List<string> programs = new List<string>();
        programResponse.ProgramsList.RemoveAt(0);

        foreach (List<string?> item in programResponse.ProgramsList)
        {
            programs.Add(item[1]);
        }

        return JsonSerializer.Serialize(programs);
    }

    // [TODO] filter out first element "Kursas"
    internal static async Task<string> GetCourses(string department, int studyTypeId, string programName, HttpClient httpClient)
    {
        Course courseResponse = await GetJson<Course>($"{department}/ajax_course_select_choices/11/?study_type_id={studyTypeId}&study_program_name={programName}", httpClient);
        
        List<string> courses = new List<string>();
        courseResponse.Courses.RemoveAt(0);

        foreach (Object[] item in courseResponse.Courses)
        {
            courses.Add(item[1].ToString());
        }

        return JsonSerializer.Serialize(courses);
    }

    internal static async Task<string> GetGroups(string department, int studyTypeId, string programName, string course, HttpClient httpClient)
    {
        Groups groupResponse = await GetJson<Groups>($"{department}/ajax_filtered_groups/11/?study_type_id={studyTypeId}&study_program_name={programName}&course={course}&exams=False", httpClient);

        return JsonSerializer.Serialize(
            groupResponse.GroupsList.Select(group => new
            {
                name = group.GroupName,
                url = group.Url
            })
        );
    }

    // TODO: parse `content` into Lecture records.
    internal static async Task<List<Lecture>> GetLectures(string department, string group, HttpClient httpClient)
    {
        // date: is a named argument, it's here just to fulfill a requirement (#4)
        DateTime weekStart = StartOfWeek(date: DateTime.Today);
        DateTime weekEnd = weekStart.AddDays(7).AddTicks(-1);

        Events events = await GetJson<Events>($"{department}/ajax_fullcalendar_events/{group}/group/321/?start={weekStart.ToString("s")}&end={weekEnd.ToString("s")}", httpClient);

        List<Lecture> lectures = [];
        AngleSharp.IConfiguration config = AngleSharp.Configuration.Default;
        AngleSharp.IBrowsingContext context = AngleSharp.BrowsingContext.New(config);
        string module = "";
        string room = "";
        foreach (Event item in events.EventsList)
        {
            var document = await context.OpenAsync(req => req.Content(item.BigTitle));
            try
            {
                module = document.QuerySelector("a").TextContent;
                room = "";

                var roomsHtml = document.QuerySelector("a").GetAttribute("data-rooms");

                if (!string.IsNullOrWhiteSpace(roomsHtml))
                {
                    var roomsDocument = await context.OpenAsync(req => req.Content(roomsHtml));
                    // remove the "Patalpos:" label to keep only the room information
                    roomsDocument.QuerySelector("strong").Remove();
                    room = roomsDocument.Body.TextContent.Trim();
                }
            }
            catch (System.Exception)
            {
                module = item.BigTitle;
                room = "";
            }

            TimeSlot slot = new(
                item.Start.DayOfWeek,
                TimeOnly.FromDateTime(item.Start),
                TimeOnly.FromDateTime(item.End)
            );

            lectures.Add(new Lecture(module, room, slot, item.ClassName));
        }

        return lectures;
    }

    static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-diff).Date;
    }
}

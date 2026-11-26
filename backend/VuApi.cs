using System.Text.Json;

using AngleSharp;

class VuApi : IVuApi
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly TimeSpan LectureLifetime = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan StaticDataLifetime = TimeSpan.FromHours(24);

    private readonly HttpClient _httpClient;
    private readonly Cache<string, List<Department>> _departmentCache;
    private readonly Cache<string, List<StudyType>> _studyTypeCache;
    private readonly Cache<string, string> _textCache;
    private readonly Cache<string, List<Lecture>> _lectureCache;

    public VuApi(
        HttpClient httpClient,
        Cache<string, List<Department>> departmentCache,
        Cache<string, List<StudyType>> studyTypeCache,
        Cache<string, string> textCache,
        Cache<string, List<Lecture>> lectureCache)
    {
        _httpClient = httpClient;
        _departmentCache = departmentCache;
        _studyTypeCache = studyTypeCache;
        _textCache = textCache;
        _lectureCache = lectureCache;
    }

    private async Task<T> GetJson<T>(string url)
    {
        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        using Stream stream = await response.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
    }

    public async Task<List<Department>> GetDepartments()
    {
        const string url = "api/department";

        return await _departmentCache.GetOrAdd(url, () => GetJson<List<Department>>(url), StaticDataLifetime);
    }

    public async Task<List<StudyType>> GetStudyTypes(string department)
    {
        return await _studyTypeCache.GetOrAdd(department, async () =>
        {
            using var response = await _httpClient.GetAsync(department + "/");
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
        }, StaticDataLifetime);
    }

    // [TODO] filter out first element "Studiju programa" and empty elements "" 
    public async Task<string> GetPrograms(string department, int studyTypeId)
    {
        string url = department + "/ajax_program_select_choices/11/?study_type_id=" + studyTypeId;

        return await _textCache.GetOrAdd(url, async () =>
        {
            Programs programResponse = await GetJson<Programs>(url);

            List<string> programs = new List<string>();
            programResponse.ProgramsList.RemoveAt(0);

            foreach (List<string?> item in programResponse.ProgramsList)
            {
                programs.Add(item[1]);
            }

            return JsonSerializer.Serialize(programs);
        }, StaticDataLifetime);
    }

    // [TODO] filter out first element "Kursas"
    public async Task<string> GetCourses(string department, int studyTypeId, string programName)
    {
        string url = $"{department}/ajax_course_select_choices/11/?study_type_id={studyTypeId}&study_program_name={programName}";

        return await _textCache.GetOrAdd(url, async () =>
        {
            Course courseResponse = await GetJson<Course>(url);

            List<string> courses = new List<string>();
            courseResponse.Courses.RemoveAt(0);

            foreach (Object[] item in courseResponse.Courses)
            {
                courses.Add(item[1].ToString());
            }

            return JsonSerializer.Serialize(courses);
        }, StaticDataLifetime);
    }

    public async Task<string> GetGroups(string department, int studyTypeId, string programName, string course)
    {
        string url = $"{department}/ajax_filtered_groups/11/?study_type_id={studyTypeId}&study_program_name={programName}&course={course}&exams=False";

        return await _textCache.GetOrAdd(url, async () =>
        {
            Groups groupResponse = await GetJson<Groups>(url);

            return JsonSerializer.Serialize(
                groupResponse.GroupsList.Select(group => new
                {
                    name = group.GroupName,
                    url = group.Url
                })
            );
        }, StaticDataLifetime);
    }

    // TODO: parse `content` into Lecture records.
    public async Task<List<Lecture>> GetLectures(string department, string group)
    {
        // date: is a named argument, it's here just to fulfill a requirement (#4)
        DateTime weekStart = StartOfWeek(date: DateTime.Today);
        DateTime weekEnd = weekStart.AddDays(7).AddTicks(-1);

        // the url contains the week, so a new week automatically becomes a new cache key
        string url = $"{department}/ajax_fullcalendar_events/{group}/group/321/?start={weekStart.ToString("s")}&end={weekEnd.ToString("s")}";

        return await _lectureCache.GetOrAdd(url, async () =>
        {
            Events events = await GetJson<Events>(url);

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
        }, LectureLifetime);
    }

    static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-diff).Date;
    }
}

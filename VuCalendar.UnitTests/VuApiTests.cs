using System.Text.Json;

namespace VuCalendar.UnitTests;

public class VuApiTests
{
    // always returns 200 OK
    private static HttpClient CreateHttpClient(string content)
    {
        var handler = new StubHttpMessageHandler(content);

        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/")
        };
    }

    [Fact]
    public async Task GetDepartments_ValidJson_ReturnsDepartments()
    {
        // Arrange
        const int expectedId = 1;
        const string expectedTitle = "Matematikos ir informatikos fakultetas";
        const string expectedAbbreviation = "MIF";

        string json = $$"""
        [
            {
                "id": {{expectedId}},
                "title_lt": "{{expectedTitle}}",
                "abbreviation_lt": "{{expectedAbbreviation}}"
            }
        ]
        """;

        using var httpClient = CreateHttpClient(json);

        // Act
        var departments = await VuApi.GetDepartments(httpClient);

        // Assert
        var department = Assert.Single(departments);

        Assert.Equal(expectedId, department.Id);
        Assert.Equal(expectedTitle, department.TitleLt);
        Assert.Equal(expectedAbbreviation, department.AbbreviationLt);
    }

    [Fact]
    public async Task GetStudyTypes_ValidHtml_ReturnsStudyTypes()
    {
        // Arrange
        const string department = "mif";

        const int expectedFirstId = 1;
        const string expectedFirstName = "Bakalauro";

        const int expectedSecondId = 2;
        const string expectedSecondName = "Magistrantūros";

        string html = $$"""
        <select id="id_study_program_filter">
            <option value="">Studijų tipas</option>
            <option value="{{expectedFirstId}}">    {{expectedFirstName}}    </option>
            <option value="{{expectedSecondId}}">{{expectedSecondName}}</option>
        </select>
        """;

        using var httpClient = CreateHttpClient(html);

        // Act
        var studyTypes = await VuApi.GetStudyTypes(department, httpClient);

        // Assert
        Assert.Collection(
            studyTypes,
            first =>
            {
                Assert.Equal(expectedFirstId, first.Id);
                Assert.Equal(expectedFirstName, first.Name);
            },
            second =>
            {
                Assert.Equal(expectedSecondId, second.Id);
                Assert.Equal(expectedSecondName, second.Name);
            }
        );
    }

    [Fact]
    public async Task GetPrograms_ValidJson_ReturnsProgramNames()
    {
        // Arrange
        const string department = "mif";
        const int studyTypeId = 1;

        const string expectedFirstProgram = "Informatika";
        const string expectedSecondProgram = "Programų sistemos";

        const string json = $$"""
        {
            "programs": [
                [null, "Studijų programa"],
                ["informatika", "{{expectedFirstProgram}}"],
                ["programų sistemos", "{{expectedSecondProgram}}"]
            ]
        }
        """;

        using var httpClient = CreateHttpClient(json);

        // Act
        var programs = await VuApi.GetPrograms(department, studyTypeId, httpClient);

        // Assert
        // we deserialize it since GetPrograms() returns a JSON string
        var programNames = JsonSerializer.Deserialize<List<string>>(programs);

        Assert.NotNull(programNames);
        Assert.Equal(2, programNames.Count);

        Assert.Equal(expectedFirstProgram, programNames[0]);
        Assert.Equal(expectedSecondProgram, programNames[1]);
    }

    [Fact]
    public async Task GetCourses_ValidJson_ReturnsCourses()
    {
        // Arrange
        const string department = "mif";
        const int studyTypeId = 1;
        const string programName = "Informatika";

        const string expectedFirstCourse = "1 Kursas";
        const string expectedSecondCourse = "2 Kursas";

        const string json = $$"""
        {
            "courses": [
                [null, "Kursas"],
                [1, "{{expectedFirstCourse}}"],
                [2, "{{expectedSecondCourse}}"]
            ]
        }
        """;

        using var httpClient = CreateHttpClient(json);

        // Act
        var courses = await VuApi.GetCourses(department, studyTypeId, programName, httpClient);

        // Assert
        var courseNames = JsonSerializer.Deserialize<List<string>>(courses);

        Assert.NotNull(courseNames);
        Assert.Equal(2, courseNames.Count);

        Assert.Equal(expectedFirstCourse, courseNames[0]);
        Assert.Equal(expectedSecondCourse, courseNames[1]);
    }

    [Fact]
    public async Task GetGroups_ValidJson_ReturnsGroups()
    {
        // Arrange
        const string department = "mif";
        const int studyTypeId = 1;
        const string programName = "Programų+sistemos";
        const string course = "4";

        const string expectedFirstName = "Bakalauro nuolatinė, Programų sistemos 4 Kursas 1 Grupė";
        const string expectedFirstUrl = "/mif/groups/programu-sistemos-4k-1gr-2026/";

        const string expectedSecondName = "Bakalauro nuolatinė, Programų sistemos 4 Kursas 2 Grupė";
        const string expectedSecondUrl = "/mif/groups/programu-sistemos-4k-2gr-2026/";

        const string json = $$"""
        {
            "groups": [
                {
                    "favourite_icon": "",
                    "group": "{{expectedFirstName}}",
                    "url": "{{expectedFirstUrl}}"
                },
                {
                    "favourite_icon": "",
                    "group": "{{expectedSecondName}}",
                    "url": "{{expectedSecondUrl}}"
                }
            ]
        }
        """;

        using var httpClient = CreateHttpClient(json);

        // Act
        var groups = await VuApi.GetGroups(department, studyTypeId, programName, course, httpClient);

        // Assert
        var returnedGroups = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(groups);

        Assert.NotNull(returnedGroups);

        Assert.Collection(
            returnedGroups,
            first =>
            {
                Assert.Equal(expectedFirstName, first["name"]);
                Assert.Equal(expectedFirstUrl, first["url"]);
            },
            second =>
            {
                Assert.Equal(expectedSecondName, second["name"]);
                Assert.Equal(expectedSecondUrl, second["url"]);
            }
        );
    }

    [Fact]
    public async Task GetLectures_ValidJson_ReturnsLectures()
    {
        // Arrange
        const string department = "mif";
        const string group = "programu-sistemos-4k-5gr-2026";

        const string expectedModule = "Programų sistemų inžinerijos procesas";
        const string expectedRoom = "306 (MIF-Naug.)";
        const string expectedModuleId = "e98769";

        const string lectureDate = "2026-11-23";
        const DayOfWeek expectedDay = DayOfWeek.Monday;
        var expectedStart = new TimeOnly(12, 40);
        var expectedEnd = new TimeOnly(13, 50);

        // simplified API response containing only fields we care about
        string json = FormattableString.Invariant($$"""
        {
            "events": [
                {
                    "className": "{{expectedModuleId}}",
                    "title": "<a data-rooms=\"&lt;span&gt;&lt;strong&gt;Patalpos: &lt;/strong&gt;{{expectedRoom}}&lt;/span&gt;\">{{expectedModule}}</a><br>{{expectedRoom}}<br>",
                    "start": "{{lectureDate}}T{{expectedStart:HH:mm:ss}}",
                    "end": "{{lectureDate}}T{{expectedEnd:HH:mm:ss}}"
                }
            ]
        }
        """);

        using var httpClient = CreateHttpClient(json);

        // Act
        var lectures = await VuApi.GetLectures(department, group, httpClient);

        // Assert
        var lecture = Assert.Single(lectures);

        Assert.Equal(expectedModule, lecture.Module);
        Assert.Equal(expectedRoom, lecture.Room);
        Assert.Equal(expectedDay, lecture.Slot.Day);
        Assert.Equal(expectedStart, lecture.Slot.Start);
        Assert.Equal(expectedEnd, lecture.Slot.End);
        Assert.Equal(expectedModuleId, lecture.ModuleId);
    }

    [Fact]
    public async Task GetLectures_MissingRoom_ReturnsLectureWithEmptyRoom()
    {
        // Arrange
        const string department = "mif";
        const string group = "programu-sistemos-4k-5gr-2026";

        const string expectedModule = "Programų sistemų inžinerijos procesas";

        const string json = $$"""
        {
            "events": [
                {
                    "className": "e98769",
                    "title": "<a data-rooms=\"\">{{expectedModule}}</a>",
                    "start": "2026-11-23T12:40:00",
                    "end": "2026-11-23T13:50:00"
                }
            ]
        }
        """;

        using var httpClient = CreateHttpClient(json);

        // Act
        var lectures = await VuApi.GetLectures(department, group, httpClient);

        // Assert
        var lecture = Assert.Single(lectures);

        Assert.Equal(expectedModule, lecture.Module);
        Assert.Empty(lecture.Room);
    }

    [Fact]
    public async Task GetLectures_PlainTextTitle_ReturnsFallbackLecture()
    {
        // Arrange
        const string department = "mif";
        const string group = "programu-sistemos-4k-5gr-2026";

        const string expectedModule = "Programų sistemų inžinerijos procesas";

        const string json = $$"""
        {
            "events": [
                {
                    "className": "e98769",
                    "title": "{{expectedModule}}",
                    "start": "2026-11-23T12:40:00",
                    "end": "2026-11-23T13:50:00"
                }
            ]
        }
        """;

        using var httpClient = CreateHttpClient(json);

        // Act
        var lectures = await VuApi.GetLectures(department, group, httpClient);

        // Assert
        var lecture = Assert.Single(lectures);

        Assert.Equal(expectedModule, lecture.Module);
        Assert.Empty(lecture.Room);
    }
}

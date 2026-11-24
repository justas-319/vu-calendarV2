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
        const string json = """
        [
            {
                "id": 1,
                "title_lt": "Matematikos ir informatikos fakultetas",
                "abbreviation_lt": "MIF"
            }
        ]
        """;

        using var httpClient = CreateHttpClient(json);

        // Act
        var departments = await VuApi.GetDepartments(httpClient);

        // Assert
        var department = Assert.Single(departments);

        Assert.Equal(1, department.Id);
        Assert.Equal("Matematikos ir informatikos fakultetas", department.TitleLt);
        Assert.Equal("MIF", department.AbbreviationLt);
    }

    [Fact]
    public async Task GetStudyTypes_ValidHtml_ReturnsStudyTypes()
    {
        // Arrange
        const string department = "mif";
        const string html = """
        <select id="id_study_program_filter">
            <option value="">Studijų tipas</option>
            <option value="1">    Bakalauro    </option>
            <option value="2">Magistrantūros</option>
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
                Assert.Equal(1, first.Id);
                Assert.Equal("Bakalauro", first.Name);
            },
            second =>
            {
                Assert.Equal(2, second.Id);
                Assert.Equal("Magistrantūros", second.Name);
            }
        );
    }

    [Fact]
    public async Task GetPrograms_ValidJson_ReturnsProgramNames()
    {
        // Arrange
        const string department = "mif";
        const int studyTypeId = 1;

        const string json = """
        {
            "programs": [
                [null, "Studijų programa"],
                ["informatika", "Informatika"],
                ["programų sistemos", "Programų sistemos"]
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

        Assert.Equal("Informatika", programNames[0]);
        Assert.Equal("Programų sistemos", programNames[1]);
    }

    [Fact]
    public async Task GetCourses_ValidJson_ReturnsCourses()
    {
        // Arrange
        const string department = "mif";
        const int studyTypeId = 1;
        const string programName = "Informatika";

        const string json = """
        {
            "courses": [
                [null, "Kursas"],
                [1, "1 Kursas"],
                [2, "2 Kursas"]
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

        Assert.Equal("1 Kursas", courseNames[0]);
        Assert.Equal("2 Kursas", courseNames[1]);
    }

    [Fact]
    public async Task GetGroups_ValidJson_ReturnsGroups()
    {
        // Arrange
        const string department = "mif";
        const int studyTypeId = 1;
        const string programName = "Programų+sistemos";
        const string course = "4";

        const string json = """
        {
            "groups": [
                {
                    "favourite_icon": "",
                    "group": "Bakalauro nuolatinė, Programų sistemos 4 Kursas 1 Grupė",
                    "url": "/mif/groups/programu-sistemos-4k-1gr-2026/"
                },
                {
                    "favourite_icon": "",
                    "group": "Bakalauro nuolatinė, Programų sistemos 4 Kursas 2 Grupė",
                    "url": "/mif/groups/programu-sistemos-4k-2gr-2026/"
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
                Assert.Equal("Bakalauro nuolatinė, Programų sistemos 4 Kursas 1 Grupė", first["name"]);
                Assert.Equal("/mif/groups/programu-sistemos-4k-1gr-2026/", first["url"]);
            },
            second =>
            {
                Assert.Equal("Bakalauro nuolatinė, Programų sistemos 4 Kursas 2 Grupė", second["name"]);
                Assert.Equal("/mif/groups/programu-sistemos-4k-2gr-2026/", second["url"]);
            }
        );
    }
}

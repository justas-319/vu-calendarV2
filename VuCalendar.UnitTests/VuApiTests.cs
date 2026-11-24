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
}

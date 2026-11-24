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
}

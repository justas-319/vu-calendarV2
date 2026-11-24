namespace VuCalendar.UnitTests;

public class VuApiTests
{
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

        // stub always returns 200 OK
        var handler = new StubHttpMessageHandler(json);

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/")
        };

        // Act
        var departments = await VuApi.GetDepartments(httpClient);

        // Assert
        var department = Assert.Single(departments);

        Assert.Equal(1, department.Id);
        Assert.Equal("Matematikos ir informatikos fakultetas", department.TitleLt);
        Assert.Equal("MIF", department.AbbreviationLt);
    }
}

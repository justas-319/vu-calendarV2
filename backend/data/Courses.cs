using System.Text.Json.Serialization;

class Course
{
    [JsonPropertyName("courses")]
    public List<object[]?> Courses { get; set; }
}

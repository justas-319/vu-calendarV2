using System.Text.Json.Serialization;

class Programs
{
    [JsonPropertyName("programs")]
    public List<List<string?>> ProgramsList { get; set; }
}

using System.Text.Json.Serialization;

class Department
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("is_root")]
    public bool? IsRoot { get; set; }

    [JsonPropertyName("title_lt")]
    public string? TitleLt { get; set; }

    [JsonPropertyName("title_en")]
    public string? TitleEn { get; set; }

    [JsonPropertyName("abbreviation_lt")]
    public string? AbbreviationLt { get; set; }

    [JsonPropertyName("abbreviation_en")]
    public string? AbbreviationEn { get; set; }

    [JsonPropertyName("buildings")]
    public List<int>? Buildings { get; set; }
}

using System.Text.Json.Serialization;

class Groups
{
    [JsonPropertyName("groups")]
    public List<Group> GroupsList { get; set; } = [];
}

class Group
{
    [JsonPropertyName("group")]
    public string? GroupName { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("favicon")]
    public string? Favicon { get; set; }
}

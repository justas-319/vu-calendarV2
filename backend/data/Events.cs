using System.Text.Json.Serialization;

class Events
{
    [JsonPropertyName("events")]
    public List<Event> EventsList { get; set; } = [];

    [JsonPropertyName("event_colors")]
    public Dictionary<string, string>? Colors { get; set; }
}

class Event
{
    [JsonPropertyName("className")]
    public string ClassName { get; set; } = "";

    [JsonPropertyName("title")]
    public string BigTitle { get; set; } = "";

    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime End { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; } = "";

    [JsonPropertyName("borderColor")]
    public string BorderColor { get; set; } = "";
}
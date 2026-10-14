using System.Text.Json.Serialization;

class Events
{
    [JsonPropertyName("events")]
    public List<List<Event>> EventsList { get; set; }
}

class Event
{
    [JsonPropertyName("className")]
    public string ClassName { get; set; }
    [JsonPropertyName("title")]
    public string BigTitle { get; set; }
    [JsonPropertyName("start")]
    public string Start { get; set; }
    [JsonPropertyName("end")]
    public string End { get; set; }
    [JsonPropertyName("color")]
    public string Color { get; set; }
    [JsonPropertyName("borderColor")]
    public string BorderColor { get; set; }
    public string Title = "";
    public string Lecturer = "";
    public string Groups = "";
    public string Room = "";
}
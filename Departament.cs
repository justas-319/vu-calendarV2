class Departament
{
    public int? Id { get; set; }
    public string? Address { get; set; }
    public bool? Is_root { get; set; }
    public string? Title_lt { get; set; }
    public string? Title_en { get; set; }
    public string? Abbreviation_lt { get; set; }
    public string? Abbreviation_en { get; set; }
    public List<int>? Buildings { get; set; }
}
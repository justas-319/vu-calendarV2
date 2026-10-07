class Departament
{
    public int? id { get; set; }
    public string? address { get; set; }
    public bool? is_root { get; set; }
    public string? title_lt { get; set; }
    public string? title_en { get; set; }
    public string? abbreviation_lt { get; set; }
    public string? abbreviation_en { get; set; }
    public List<int>? buildings { get; set; }
}
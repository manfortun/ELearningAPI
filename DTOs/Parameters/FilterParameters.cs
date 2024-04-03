namespace eLearningApi.DTOs.Parameters;

public class FilterParameters
{
    public string? Keyword { get; set; }
    public bool? HasChildren { get; set; }
    public bool? IsPublished { get; set; }
}

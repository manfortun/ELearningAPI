namespace eLearningApi.DTOs.Parameters;

public class ContentRetrievalParameters
{
    public SortingParameters SortingParameters { get; set; } = new SortingParameters();
    public PaginationParameters PaginationParameters { get; set; } = new PaginationParameters();
    public IncludeParameters IncludeParameters { get; set; } = new IncludeParameters();
    public int ModuleId { get; set; }
    public int? ContentId { get; set; }
    public string? Keyword { get; set; }
}

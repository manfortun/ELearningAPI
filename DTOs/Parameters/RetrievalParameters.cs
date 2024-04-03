namespace eLearningApi.DTOs.Parameters;

public class RetrievalParameters
{
    public SortingParameters SortingParameters { get; set; } = new SortingParameters();
    public PaginationParameters PaginationParameters { get; set; } = new PaginationParameters();
    public FilterParameters FilterParameters { get; set; } = new FilterParameters();
    public IncludeParameters IncludeParameters { get; set; } = new IncludeParameters();
}

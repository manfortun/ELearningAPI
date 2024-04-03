namespace eLearningApi.DTOs.Parameters;

public class EnrollmentRetrievalParameters
{
    public SortingParameters SortingParameters { get; set; } = new SortingParameters();
    public IncludeParameters IncludeParameters { get; set; } = new IncludeParameters();
    public PaginationParameters PaginationParameters { get; set; } = new PaginationParameters();
    public int? StudentId { get; set; }
    public int[]? CourseIds { get; set; }
    public bool? IsCompleted { get; set; }
}

namespace eLearningApi.DTOs.Parameters;

public class SortingParameters
{
    public const string ASCENDING = "ASC";
    public const string DESCENDING = "DESC";
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
}

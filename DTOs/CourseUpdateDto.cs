namespace eLearningApi.DTOs;

public class CourseUpdateDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsPublished { get; set; }
}

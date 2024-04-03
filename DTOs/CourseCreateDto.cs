namespace eLearningApi.DTOs;

public class CourseCreateDto
{
    public string Title { get; set; } = default!;
    public int SubjectId { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
}

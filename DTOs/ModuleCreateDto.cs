namespace eLearningApi.DTOs;

public class ModuleCreateDto
{
    public string Title { get; set; } = default!;
    public int Duration { get; set; }
    public int CourseId { get; set; }
}

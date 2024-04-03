namespace eLearningApi.DTOs;

public class SubjectUpdateDto
{
    public string Title { get; set; } = default!;
    public bool IsPublished { get; set; }
}

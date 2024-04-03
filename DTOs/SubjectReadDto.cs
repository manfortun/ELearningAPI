using eLearningApi.Models.Interfaces;

namespace eLearningApi.DTOs;

public class SubjectReadDto : IAuditableEntity
{
    public int Id { get; set; }
    public bool IsPublished { get; set; }
    public string Title { get; set; } = default!;
    public string Owner { get; set; } = default!;
    public IEnumerable<CourseReadDto> Courses { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

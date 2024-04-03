using eLearningApi.Models.Interfaces;

namespace eLearningApi.DTOs;

public class ModuleReadDto : IAuditableEntity
{
    public int Id { get; set; }
    public bool IsPublished { get; set; }
    public string CourseTitle { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual IEnumerable<ContentReadDto> Contents { get; set; } = default!;
}

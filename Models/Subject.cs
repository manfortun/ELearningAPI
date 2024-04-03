using eLearningApi.Models.Interfaces;

namespace eLearningApi.Models;

public class Subject : IAuditableEntity, ITitledEntity, IPublishableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AuthorId { get; set; }
    public virtual User? Author { get; set; }
    public virtual IEnumerable<Course>? Courses { get; set; }
}

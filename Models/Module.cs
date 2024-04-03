using eLearningApi.Models.Interfaces;

namespace eLearningApi.Models;

public class Module : IAuditableEntity, ITitledEntity, IPublishableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AuthorId { get; set; }
    public virtual User? Author { get; set; }
    public int CourseId { get; set; }
    public virtual Course? Course { get; set; }
    public virtual IEnumerable<Content>? Contents { get; set; }
}

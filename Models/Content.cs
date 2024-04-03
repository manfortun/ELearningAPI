using eLearningApi.Models.Interfaces;

namespace eLearningApi.Models;

public class Content : IAuditableEntity, IPublishableEntity
{
    public int Id { get; set; }
    public byte[]? Material { get; set; } = default!;
    public bool IsPublished { get; set; }
    public string Type { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AuthorId { get; set; }
    public virtual User? Author { get; set; }
    public int ModuleId { get; set; }
    public virtual Module? Module { get; set; }
}

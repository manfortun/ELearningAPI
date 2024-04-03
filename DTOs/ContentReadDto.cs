using eLearningApi.Models.Interfaces;

namespace eLearningApi.DTOs;

public class ContentReadDto : IAuditableEntity
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public byte[]? Content { get; set; }
    public string Type { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

using eLearningApi.Models.Interfaces;

namespace eLearningApi.DTOs;

public class CourseReadDto : IAuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageSource { get; set; }
    public string Author { get; set; }
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

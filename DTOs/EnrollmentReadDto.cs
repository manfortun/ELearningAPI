using eLearningApi.Models.Interfaces;

namespace eLearningApi.DTOs;

public class EnrollmentReadDto : IAuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

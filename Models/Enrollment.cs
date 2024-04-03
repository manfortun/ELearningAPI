using eLearningApi.Models.Interfaces;

namespace eLearningApi.Models;

public class Enrollment : IAuditableEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public int CourseId { get; set; }
    public virtual Course? Course { get; set; }
    public virtual IEnumerable<EnrollmentModule>? EnrollmentModules { get; set; }
}

using eLearningApi.Models.Interfaces;

namespace eLearningApi.Models;

public class User : IAuditableEntity
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Salt { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual IEnumerable<Subject>? Subjects { get; set; }
    public virtual IEnumerable<Course>? Courses { get; set; }
    public virtual IEnumerable<Module>? Modules { get; set; }
    public virtual IEnumerable<Content>? Contents { get; set; }
    public virtual IEnumerable<Enrollment>? Enrollments { get; set; }
}

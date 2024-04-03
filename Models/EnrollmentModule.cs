using eLearningApi.Models.Interfaces;

namespace eLearningApi.Models;

public class EnrollmentModule : IModel
{
    public int Id { get; set; }
    public bool IsCompleted { get; set; }
    public int ModuleId { get; set; }
    public virtual Module? Module { get; set; }
    public int EnrollmentId { get; set; }
    public virtual Enrollment? Enrollment { get; set; }
}

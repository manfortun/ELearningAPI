namespace eLearningApi.Models.Interfaces;

public interface IAuditableEntity : IModel
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

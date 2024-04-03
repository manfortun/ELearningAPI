using eLearningApi.Models.Interfaces;

namespace eLearningApi.Models;

public class Token : IAuditableEntity
{
    public int Id { get; set; }
    public string Value { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsExecuted { get; set; }
    public string TokenType { get; set; }
}

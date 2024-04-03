namespace eLearningApi.DTOs;

public class UserCreateCompleteDto
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}

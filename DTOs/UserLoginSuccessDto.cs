namespace eLearningApi.DTOs;

public class UserLoginSuccessDto
{
    public string AccessToken { get; set; }
    public int Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

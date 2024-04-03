namespace eLearningApi.DTOs;

public class ContentUpdateDto
{
    public byte[]? Material { get; set; }
    public string? Type { get; set; }
    public bool IsPublished { get; set; }
}

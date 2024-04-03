namespace eLearningApi.DTOs;

public class ContentCreateDto
{
    public int ModuleId { get; set; }
    public byte[] Content { get; set; } = default!;
    public string Type { get; set; } = default!;
}

namespace eLearningApi.DTOs;

public class ModuleUpdateDto
{
    public string Title { get; set; } = default!;
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
}

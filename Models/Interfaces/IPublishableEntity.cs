namespace eLearningApi.Models.Interfaces;

public interface IPublishableEntity : IModel
{
    bool IsPublished { get; set; }
    int AuthorId { get; set; }
    abstract User? Author { get; set; }
}

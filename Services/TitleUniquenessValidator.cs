using eLearningApi.DataAccess;
using eLearningApi.Models.Interfaces;

namespace eLearningApi.Services;

public static class TitleUniquenessValidator<T> where T : class, ITitledEntity, IModel
{
    /// <summary>
    /// Checks if a <paramref name="title"/> is unique.
    /// </summary>
    /// <param name="repository">Repository of type <typeparamref name="T"/>.</param>
    /// <param name="title">Title to evaluate.</param>
    /// <param name="id">Does not check the entity with the supplied ID. Used for checking uniqueness during update.</param>
    /// <returns></returns>
    public static bool IsUnique(BaseRepository<T> repository, string title, int id = 0)
    {
        string loweredTitle = title.ToLower();
        T? entity = repository.Get(e => e.Id != id && e.Title.ToLower() == loweredTitle)?.FirstOrDefault();

        return entity is null;
    }
}

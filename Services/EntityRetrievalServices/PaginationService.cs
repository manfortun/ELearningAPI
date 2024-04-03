using eLearningApi.DTOs.Parameters;

namespace eLearningApi.Services.EntityRetrievalServices;

public class PaginationService<T>(PaginationParameters parameters, FilterLayer<T>? next = null) : FilterLayer<T>(next)
{
    protected override void Run()
    {
        var values = GetValues();
        values = values.Skip(parameters.Limit * (parameters.Page - 1)).Take(parameters.Limit);

        SetValues(values);
    }
}

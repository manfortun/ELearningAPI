using eLearningApi.DTOs.Parameters;

namespace eLearningApi.Services.EntityRetrievalServices;

public class SortingService<T>(SortingParameters? parameters, FilterLayer<T> next = null) : FilterLayer<T>(next)
{
    protected override void Run()
    {
        var values = GetValues();
        if (!string.IsNullOrEmpty(parameters.Sort))
        {
            if (parameters.SortDirection == SortingParameters.DESCENDING)
            {
                values = values.OrderByDescending(x => parameters.Sort);
            }
            else
            {
                values = values.OrderBy(x => parameters.Sort);
            }
        }
        else
        {
            if (parameters.SortDirection == SortingParameters.DESCENDING)
            {
                values = values.OrderDescending();
            }
            else
            {
                values = values.Order();
            }
        }

        SetValues(values);
    }
}

using eLearningApi.DTOs.Parameters;
using eLearningApi.Models.Interfaces;

namespace eLearningApi.Services.EntityRetrievalServices;

public class RetrievalManager<T> where T : ITitledEntity, IPublishableEntity
{
    private readonly FilterLayer<T> _filterLayerLinkedList;

    public RetrievalManager(RetrievalParameters retrievalParameters)
    {
        var filterService = new FilterService<T>(retrievalParameters.FilterParameters);
        var sortingService = new SortingService<T>(retrievalParameters.SortingParameters, filterService);
        var paginationService = new PaginationService<T>(retrievalParameters.PaginationParameters, sortingService);

        _filterLayerLinkedList = paginationService;
    }

    public void SetData(IEnumerable<T> data)
    {
        _filterLayerLinkedList.SetValues(data);
    }

    public IEnumerable<T> Execute()
    {
        _filterLayerLinkedList.Execute();
        return _filterLayerLinkedList.GetValues();
    }
}

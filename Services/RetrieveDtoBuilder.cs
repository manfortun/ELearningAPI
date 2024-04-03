using AutoMapper;
using eLearningApi.DTOs;
using eLearningApi.DTOs.Parameters;

namespace eLearningApi.Services;

public class RetrieveDtoBuilder<TSource, TDest>
{
    private IMapper mapper;
    private IEnumerable<TSource> data;
    private RetrieveDto<TDest> instance;

    public RetrieveDtoBuilder()
    {
        instance = new RetrieveDto<TDest>();
    }

    public RetrieveDtoBuilder<TSource, TDest> SetMapper(IMapper mapper)
    {
        this.mapper = mapper;
        return this;
    }

    public RetrieveDtoBuilder<TSource, TDest> SetData(IEnumerable<TSource> data)
    {
        this.data = data;
        return this;
    }

    public RetrieveDtoBuilder<TSource, TDest> SetPaginationParams(PaginationParameters paginationParams)
    {
        instance ??= new RetrieveDto<TDest>();
        instance.Page = paginationParams.Page;
        instance.Limit = paginationParams.Limit;
        return this;
    }

    public RetrieveDto<TDest> Build()
    {
        instance ??= new RetrieveDto<TDest>();
        if (mapper is not null)
        {
            instance.Data = mapper.Map<TDest[]>(data);
        }

        return instance;
    }
}

namespace eLearningApi.DTOs.Parameters;

public class PaginationParameters
{
    public const int DEFAULT_LIMIT = 25;
    public const int DEFAULT_PAGE = 1;
    private int _limit = DEFAULT_LIMIT;
    private int _page = DEFAULT_PAGE;

    public int Limit
    {
        get => _limit;
        set => _limit = Math.Max(1, value);
    }

    public int Page
    {
        get => _page;
        set => _page = Math.Max(1, value);
    }
}

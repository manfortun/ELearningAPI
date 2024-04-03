namespace eLearningApi.DTOs;

public class GetUsersDto
{
    private UserReadDto[] userReads = [];
    public UserReadDto[] Data
    {
        get
        {
            return userReads;
        }
        set
        {
            int currPage = Math.Max(0, Page - 1);
            userReads = value
                .Skip(currPage * Limit)
                .Take(Page)
                .ToArray();
        }
    }

    public int TotalCount => Data.Length;
    public int Page { get; set; } = 0;
    public int Limit => 5;
}

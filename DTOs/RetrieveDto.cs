namespace eLearningApi.DTOs;

public class RetrieveDto<T>
{
    public T[] Data { get; set; } = default!;
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalCount => Data.Length;
}

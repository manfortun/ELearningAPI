namespace eLearningApi.Services.EntityRetrievalServices;

public abstract class FilterLayer<T>
{
    private IEnumerable<T> values = default!;
    protected readonly FilterLayer<T>? _next = default!;

    public FilterLayer() { }

    public FilterLayer(FilterLayer<T>? next = null)
    {
        _next = next;
    }

    public void Execute()
    {
        if (_next != null)
        {
            _next.Execute();
        }

        Run();
    }

    public void SetValues(IEnumerable<T> values)
    {
        if (_next != null)
        {
            _next.SetValues(values);
        }
        else
        {
            this.values = values ?? [];
        }
    }

    public IEnumerable<T> GetValues()
    {
        if (_next != null)
        {
            return _next.GetValues();
        }
        else
        {
            return values;
        }
    }

    protected abstract void Run();
}

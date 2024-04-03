using eLearningApi.DTOs.Parameters;
using eLearningApi.Models;
using eLearningApi.Models.Interfaces;

namespace eLearningApi.Services.EntityRetrievalServices;

public class FilterService<T>(FilterParameters? parameters, FilterLayer<T>? next = null) :
    FilterLayer<T>(next) where T : ITitledEntity, IPublishableEntity
{
    protected override void Run()
    {
        var values = GetValues();
        // filter values based on published status
        if (parameters.IsPublished is bool)
        {
            values = values.Where(v => v.IsPublished == parameters.IsPublished);
        }

        // filter values based on presence of children
        if (parameters.HasChildren is bool)
        {
            if (values is IEnumerable<Subject> subjects)
            {
                values = subjects.Where(v => v.Courses?.Any() ?? false == parameters.HasChildren).OfType<T>();
            }
            else if (values is IEnumerable<Course> courses)
            {
                values = courses.Where(c => c.Modules?.Any() ?? false == parameters.HasChildren).OfType<T>();
            }
            else
            {
                // FALLTHROUGH
            }
        }

        // filter values based on keyword
        if (!string.IsNullOrEmpty(parameters.Keyword))
        {
            values = values.Where(v =>
            {
                if (NormalizedCompare(v.Title, parameters.Keyword))
                {
                    return true;
                }
                else if (v.Author is not null && NormalizedCompare(v.Author.FirstName + v.Author.LastName, parameters.Keyword))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        SetValues(values);
    }
    private string ToNormalizedString(string @string)
    {
        return @string.Replace(" ", "").ToLower();
    }

    private bool NormalizedCompare(string source, string keyword)
    {
        return ToNormalizedString(source).Contains(ToNormalizedString(keyword));
    }
}

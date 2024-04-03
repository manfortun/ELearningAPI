using eLearningApi.Models;

namespace eLearningApi.DTOs.Parameters;

public class IncludeParameters
{
    public string[] Properties { get; set; } = [];

    public static List<string> GetDefaults<T>()
    {
        var include = new List<string>();
        if (typeof(T) == typeof(Subject))
        {
            include.Add("Author");
            include.Add("Courses");
        }
        else if (typeof(T) == typeof(Course))
        {
            include.Add("Author");
            include.Add("Modules");
        }
        else if (typeof(T) == typeof(Module))
        {
            include.Add("Contents");
        }
        else if (typeof(T) == typeof(Content))
        {
            include.Add("Author");
            include.Add("Module");
        }
        else if (typeof(T) == typeof(Enrollment))
        {
            include.Add("User");
            include.Add("Course");
        }
        else if (typeof(T) == typeof(EnrollmentModule))
        {
            include.Add("User");
            include.Add("Enrollment");
        }
        else
        {
            throw new NotImplementedException();
        }

        return include;
    }

    public void IncludeDefaults<T>()
    {
        var include = Properties?.ToHashSet() ?? [];

        foreach (var property in GetDefaults<T>())
        {
            include.Add(property);
        }

        Properties = [.. include];
    }
}

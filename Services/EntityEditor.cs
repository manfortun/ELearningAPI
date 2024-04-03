using System.Reflection;

namespace eLearningApi.Services;

public class EntityEditor<TDest, TUpdate>
{
    public TDest ApplyEdit(TDest dest, TUpdate update)
    {
        var srcProperties = typeof(TDest).GetProperties().ToDictionary(p => p.Name, p => p);
        PropertyInfo[] updateProperties = typeof(TUpdate).GetProperties();

        foreach (PropertyInfo property in updateProperties)
        {
            if (srcProperties.ContainsKey(property.Name))
            {
                try
                {
                    srcProperties[property.Name].SetValue(dest, property.GetValue(update));
                }
                catch
                {
                    Console.WriteLine($"The property {property.Name} was not set to the destination object.");
                }
            }
        }

        return dest;
    }
}

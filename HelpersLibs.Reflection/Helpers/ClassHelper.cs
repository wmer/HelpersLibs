using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Reflection.Helpers; 
public class ClassHelper {
    public static T GetPropValue<T>(object instance, string propName) {
        var value = default(T);

        try {
            PropertyInfo property = instance.GetType().GetProperty(propName);
            value = (T)property.GetValue(instance, null);
        } catch { }

        return value;
    }

    public static List<string> GetPropertiesOfType<T, U>() {
        var value = new List<string>();

        try {
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var props = propertyInfos.Where(x => x.PropertyType == typeof(U));
            value = props.Select(x => x.Name).ToList(); 
        } catch { }

        return value;
    }
}

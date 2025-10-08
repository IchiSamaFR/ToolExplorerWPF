using System.Reflection;
using System.Text.Json;

namespace ToolExplorerWPF.Helpers.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Met à jour les propriétés publiques de la cible avec celles du source (sauf Id).
        /// </summary>
        public static void MergeFrom<T>(this T target, T source, params string[] excludedProperties)
        {
            if (target == null || source == null) return;

            var type = typeof(T);
            var exclusions = new HashSet<string>(excludedProperties ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanWrite || !prop.CanRead)
                    continue;
                if (exclusions.Contains(prop.Name) || prop.Name == "Id")
                    continue;

                var value = prop.GetValue(source);
                prop.SetValue(target, value);
            }
        }

        /// <summary>
        /// Clone profond d'un objet via sérialisation JSON.
        /// </summary>
        public static T Clone<T>(this T source)
        {
            if (source == null) return default!;
            var json = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<T>(json)!;
        }
    }
}
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace ToolExplorerWPF.Helpers.Converters
{
    public class ListDistinctConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IEnumerable enumerable)
                return Array.Empty<object>();

            var distinctSeq = GetDistinctSequence(enumerable, parameter as string);
            return MaterializeToTargetType(distinctSeq, targetType);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();

        protected static IEnumerable GetDistinctSequence(IEnumerable enumerable, string? propertyPath)
        {
            // Use property path selector if provided
            if (!string.IsNullOrWhiteSpace(propertyPath))
            {
                return enumerable
                    .Cast<object>()
                    .DistinctBy(item => GetPropertyPathValue(item, propertyPath));
            }

            // Preserve type-specific equality with Distinct<T>()
            var elementType = GetElementType(enumerable);
            if (elementType is not null)
            {
                var distinctMethod = typeof(Enumerable)
                    .GetMethod(nameof(Enumerable.Distinct), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(IEnumerable<>).MakeGenericType(elementType) }, null)
                    ?.MakeGenericMethod(elementType);

                return (IEnumerable)(distinctMethod?.Invoke(null, new object[] { enumerable }) ?? Array.Empty<object>());
            }

            return enumerable.Cast<object>().Distinct();
        }

        protected static object MaterializeToTargetType(IEnumerable source, Type targetType)
        {
            // Return source if already compatible
            if (targetType == typeof(object) || targetType == typeof(IEnumerable) || targetType.IsAssignableFrom(source.GetType()))
                return source;

            // ICollectionView
            if (typeof(ICollectionView).IsAssignableFrom(targetType))
                return CollectionViewSource.GetDefaultView(source);

            // Array: T[]
            if (targetType.IsArray)
                return ConvertToArray(source, targetType.GetElementType() ?? typeof(object));

            // Generic collections
            if (targetType.IsGenericType)
            {
                var genericDef = targetType.GetGenericTypeDefinition();
                var elemType = targetType.GetGenericArguments().FirstOrDefault() ?? GetElementType(source) ?? typeof(object);

                return genericDef switch
                {
                    var t when t == typeof(ObservableCollection<>) => ConvertToObservableCollection(source, elemType),
                    var t when t == typeof(List<>) => ConvertToList(source, elemType),
                    var t when t == typeof(IEnumerable<>) || t == typeof(ICollection<>) ||
                               t == typeof(IList<>) || t == typeof(IReadOnlyCollection<>) ||
                               t == typeof(IReadOnlyList<>) => ConvertToList(source, elemType),
                    _ => source.Cast<object>().ToList()
                };
            }

            // Non-generic IList
            if (typeof(IList).IsAssignableFrom(targetType))
                return source.Cast<object>().ToList();

            // Fallback
            return source.Cast<object>().ToList();
        }

        protected static Type? GetElementType(object sequence)
        {
            var type = sequence.GetType();

            // Direct match
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // Check interfaces
            return type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault();
        }

        protected static object ConvertToList(IEnumerable source, Type elementType)
            => InvokeGenericLinqMethod(nameof(Enumerable.ToList), elementType, CastToType(source, elementType));

        protected static object ConvertToArray(IEnumerable source, Type elementType)
            => InvokeGenericLinqMethod(nameof(Enumerable.ToArray), elementType, CastToType(source, elementType));

        protected static object ConvertToObservableCollection(IEnumerable source, Type elementType)
        {
            var casted = CastToType(source, elementType);
            var ocType = typeof(ObservableCollection<>).MakeGenericType(elementType);
            return Activator.CreateInstance(ocType, casted)!;
        }

        protected static object CastToType(IEnumerable source, Type elementType)
            => InvokeGenericLinqMethod(nameof(Enumerable.Cast), elementType, source);

        protected static object InvokeGenericLinqMethod(string methodName, Type elementType, object source)
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == methodName && m.GetParameters().Length == 1)
                .MakeGenericMethod(elementType);

            return method.Invoke(null, new[] { source })!;
        }

        protected static object? GetPropertyPathValue(object? obj, string path)
        {
            if (obj is null) return null;

            object? current = obj;
            foreach (var segment in path.Split('.'))
            {
                if (current is null) return null;

                var prop = current.GetType()
                    .GetProperty(segment, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (prop is null) return null;
                current = prop.GetValue(current);
            }
            return current;
        }
    }
}
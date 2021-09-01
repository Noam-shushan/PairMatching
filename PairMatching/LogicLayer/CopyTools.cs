using System;
using System.Linq;
using System.Reflection;


namespace LogicLayer
{
    public static class CopyTools
    {
        public static T CopyPropertiesTo<T, S>(this S from, T to)
        {
            foreach (PropertyInfo propTo in to.GetType().GetProperties())
            {
                PropertyInfo propFrom = typeof(S).GetProperty(propTo.Name);
                if (propFrom == null)
                {
                    continue;
                }

                var value = propFrom.GetValue(from, null);
                if (value != null)
                {
                    propTo.SetValue(to, value);
                }
            }
            return to;
        }

        public static object CopyPropertiesToNew<S>(this S from, Type type)
        {
            object to = Activator.CreateInstance(type); // new object of Type
            return from.CopyPropertiesTo(to);
        }

        public static string SpliceText(this string text, int n = 8)
        {
            text = string.Join(Environment.NewLine, text.Split()
                .Select((word, index) => new { word, index })
                .GroupBy(x => x.index / n)
                .Select(grp => string.Join(" ", grp.Select(x => x.word))));
            return text;
        }

        public static T Clone<T>(this T original) where T : new()
        {
            T copyToObject = new T();

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(copyToObject, propertyInfo.GetValue(original, null), null);
                }
            }

            return copyToObject;
        }
    }
}

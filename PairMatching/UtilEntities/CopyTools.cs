using System;
using System.Linq;
using System.Reflection;

namespace UtilEntities
{
    /// <summary>
    /// Copy tool functions
    /// </summary>
    public static class CopyTools
    {
        /// <summary>
        /// Copy the values of propreties that have the same name 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="from">Copy from the object</param>
        /// <param name="to">Copy to this object</param>
        /// <returns>The object with the value of the 'from' properties</returns>
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

        /// <summary>
        /// Copy properties to new instance
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="from"></param>
        /// <param name="type"></param>
        /// <returns>New instance with the value of the 'from' properties</returns>
        public static object CopyPropertiesToNew<S>(this S from, Type type)
        {
            object to = Activator.CreateInstance(type); // new object of Type
            return from.CopyPropertiesTo(to);
        }

        /// <summary>
        /// Splice text with new line in every 'n' characters 
        /// </summary>
        /// <param name="text">The text to splice</param>
        /// <param name="n">Number of characters to put new line</param>
        /// <returns>This string with new lines in every 'n' characters</returns>
        public static string SpliceText(this string text, int n = 8)
        {
            text = string.Join(Environment.NewLine, text.Split()
                .Select((word, index) => new { word, index })
                .GroupBy(x => x.index / n)
                .Select(grp => string.Join(" ", grp.Select(x => x.word))));
            return text;
        }

        /// <summary>
        /// Clone any object. Get a copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">the original object</param>
        /// <returns>The new copy of the original object</returns>
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


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


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
    }
}

using Gui.ViewModels;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gui.Converters
{
    public class StudentVMToStudentConverter : IMultiValueConverter
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int firstId = 0, secondId = 0;
            bool parse = int.TryParse(values[0].ToString(), out firstId) &&
                int.TryParse(values[1].ToString(), out secondId);

            if (!parse)
            {
                return null;
            }

            if (firstId != 0 && secondId != 0)
            {
                var firstStud = logicLayer.GetStudent(firstId);
                var secondStud = logicLayer.GetStudent(secondId);
                return new TempPair { First = firstStud, Second = secondStud };
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

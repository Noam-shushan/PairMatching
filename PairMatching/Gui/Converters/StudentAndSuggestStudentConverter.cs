using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BO;
using Gui.ViewModels;
using LogicLayer;

namespace Gui.Converters
{
    public class StudentAndSuggestStudentConverter : IMultiValueConverter
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var first = values[0] as SuggestStudent;
            var secondStudVM = values[1] as StudentViewModel;
            if(first != null && secondStudVM != null)
            {
                var firstStud = logicLayer.GetStudent(first.SuggestStudentId);
                var secondStud = logicLayer.GetStudent(secondStudVM.Id);
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

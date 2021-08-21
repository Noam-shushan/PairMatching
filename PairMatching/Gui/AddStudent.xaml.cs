using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gui
{
    /// <summary>
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class AddStudent : UserControl
    {
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(BO.Student), new PropertyMetadata(0));

        public string Country
        {
            get { return (string)GetValue(CountryProperty); }
            set { SetValue(CountryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Country.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountryProperty =
            DependencyProperty.Register("Country", typeof(string), typeof(BO.Student), new PropertyMetadata(0));

        public string PhoneNumber
        {
            get { return (string)GetValue(PhoneNumberProperty); }
            set { SetValue(PhoneNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PhoneNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhoneNumberProperty =
            DependencyProperty.Register("PhoneNumber", typeof(string), typeof(BO.Student), new PropertyMetadata(0));

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Email.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(BO.Student), new PropertyMetadata(0));

        public AddStudent()
        {
            InitializeComponent();
            DataContext = this;
        }

    }
}

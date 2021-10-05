using LogicLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;


namespace Gui
{
    /// <summary>
    /// Interaction logic for AddStudentWin.xaml
    /// </summary>
    public partial class AddStudentWin : Window
    {
        private static readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public AddStudentWin()
        {
            InitializeComponent();
            cbCountry.ItemsSource = GetCountryList();
        }

        public List<string> GetCountryList()
        {
            List<string> cultureList = new List<string>();
            
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.LCID);
                if (!cultureList.Contains(region.EnglishName))
                {
                    cultureList.Add(region.EnglishName);
                }
            }
            return cultureList;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if(cbCountry.SelectedItem == null 
                || string.IsNullOrEmpty(tbEmail.Text)
                || string.IsNullOrEmpty(tbName.Text)
                || string.IsNullOrEmpty(tbPhone.Text))
            {
                Messages.MessageBoxWarning("אנא מלא את כל הפרטים");
                return;
            }
            try
            {
                logicLayer.AddStudent(new BO.Student
                {
                    Name = tbName.Text,
                    Country = cbCountry.Text,
                    Email = tbEmail.Text,
                    PhoneNumber = tbPhone.Text,
                    IsSimpleStudent = true
                });
                var mainWin = Application.Current.MainWindow as MainWindow;
                mainWin.RefreshMyStudentsView();
                Close();
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
        }
    }
}

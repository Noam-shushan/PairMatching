using LogicLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using BO;

namespace Gui
{
    /// <summary>
    /// Interaction logic for AddStudentWin.xaml
    /// </summary>
    public partial class AddStudentWin : Window, INotifyPropertyChanged
    {
        private static readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        private Student _newStudent = new Student();

        public Student NewStudent
        {
            get => _newStudent;
            set
            {
                _newStudent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewStudent"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddStudentWin()
        {
            InitializeComponent();
            cbCountry.ItemsSource = GetCountryList();
            DataContext = NewStudent;
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
            if (cbCountry.SelectedItem == null
                || cbTrack.SelectedItem == null
                || string.IsNullOrEmpty(tbEmail.Text)
                || string.IsNullOrEmpty(tbName.Text)
                || string.IsNullOrEmpty(tbPhone.Text))
            {
                Messages.MessageBoxWarning("אנא מלא את כל הפרטים");
                return;
            }
            try
            {
                logicLayer.AddStudent(NewStudent, cbTrack.Text);
                NewStudent = new Student();
                
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

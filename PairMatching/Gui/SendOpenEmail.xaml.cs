using Microsoft.Win32;
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
using System.Windows.Shapes;
using LogicLayer;
using System.ComponentModel;

namespace Gui
{
    /// <summary>
    /// Interaction logic for SendOpenEmail.xaml
    /// </summary>
    // TODO: Allow more then one file to send
    public partial class SendOpenEmail : Window, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();
        
        public event PropertyChangedEventHandler PropertyChanged;

        private string _fileName = "";
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileName"));
            }
        }

        private string _filePath = "";

        public string FilePath 
        { 
            get => _filePath; 
            set
            {
                _filePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilePath"));
            } 
        }

        public string StudentName { get; set; }

        public string Email { get; set; }

        public SendOpenEmail()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isSend = false;
            if (string.IsNullOrEmpty(tbBody.Text))
            {
                if(!Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך לשלוח מייל עם תוכן ריק?"))
                {
                    return;
                }
            }
            if (string.IsNullOrEmpty(tbSubject.Text))
            {
                if (!Messages.MessageBoxConfirmation("האם אתה בטוח שברצונך לשלוח מייל עם נושא ריק?"))
                {
                    return;
                }
            }
            try
            {
                await logicLayer.SendOpenEmailAsync(Email, tbSubject.Text, tbBody.Text, FilePath);
                isSend = true;
                Messages.MessageBoxSimple("המייל נשלח בהצלחה");
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            finally
            {
                FilePath = FileName = "";
            }
            if (isSend)
            {
                Close();
            }
        }

        private void attachmentBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            var response = openFileDialog.ShowDialog();
            
            if(response == true)
            {
                FileName = openFileDialog.SafeFileName;
                FilePath = openFileDialog.FileName;
            }
        }

        private void remFileAttchBtn_Click(object sender, RoutedEventArgs e)
        {
            FilePath = FileName = "";
        }
    }
}

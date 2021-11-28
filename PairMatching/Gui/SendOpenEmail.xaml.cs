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
using System.Collections.ObjectModel;

namespace Gui
{

    public class File
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }
    }

    /// <summary>
    /// Interaction logic for SendOpenEmail.xaml
    /// </summary>
    // TODO: Allow more then one file to send
    public partial class SendOpenEmail : Window, INotifyPropertyChanged
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();
        
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<File> Files { get; set; } = new ObservableCollection<File>();

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
                await logicLayer.SendOpenEmailAsync(Email, tbSubject.Text, tbBody.Text, 
                    from f in Files select f.FilePath);
                isSend = true;
                Messages.MessageBoxSimple("המייל נשלח בהצלחה");
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            if (isSend)
            {
                Files.Clear();
                Close();
            }
        }

        private void attachmentBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true
            };

            var response = openFileDialog.ShowDialog();
            if(!openFileDialog.CheckPathExists)
            {
                Messages.MessageBoxError("קובץ לא קיים");
            }
            if(response == true)
            {
                var temp = openFileDialog.SafeFileNames.Zip(openFileDialog.FileNames, 
                    (fn, fp) => new File
                    {
                        FileName = fn,
                        FilePath = fp
                    });
                foreach(var f in temp)
                {
                    Files.Add(f);
                }
            }
        }

        private void remFileAttchBtn_Click(object sender, RoutedEventArgs e)
        {
            var file = (sender as Button).DataContext as File;
            if(file != null)
            {
                Files.Remove(file);
            }
        }
    }
}

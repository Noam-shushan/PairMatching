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

namespace Gui
{
    /// <summary>
    /// Interaction logic for SendOpenEmail.xaml
    /// </summary>
    public partial class SendOpenEmail : Window
    {
        IBL bl = BlFactory.GetBL();

        string _filePath = "";

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
                await bl.SendOpenEmailAsync(Email, tbSubject.Text, tbBody.Text, _filePath);
                isSend = true;
                Messages.MessageBoxSimple("המייל נשלח בהצלחה");
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
            }
            finally
            {
                _filePath = "";
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
                _filePath = openFileDialog.FileName;
                filePathLable.Content = _filePath;
            }
        }
    }
}

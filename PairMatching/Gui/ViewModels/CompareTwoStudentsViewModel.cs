using Gui.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using LogicLayer;
using System.Windows;

namespace Gui.ViewModels
{
    public class CompareTwoStudentsViewModel
    {
        public StudentViewModel StudentFromIsrael { get; set; }
        public StudentViewModel StudentFromWorld { get; set; }

        public MatchCommand Match { get; set; }

        ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        public CompareTwoStudentsViewModel(StudentViewModel first, StudentViewModel seconde)
        {
            if (first.IsFromIsrael)
            {
                StudentFromIsrael = first;
                StudentFromWorld = seconde;
            }
            else
            {
                StudentFromIsrael = seconde;
                StudentFromWorld = first;
            }

            Match = new MatchCommand();
            Match.MathcAsync += Match_MathcAsync;
        }

        private async Task<bool> Match_MathcAsync(Student first, Student second)
        {
            try
            {
                if (Messages.MessageBoxConfirmation("האם להתאים ולחזור לעמוד הראשי?"))
                {
                    await logicLayer.MatchAsync(first, second);
                    var win = Application
                        .Current.Windows
                        .OfType<Views.CompareTwoStudentsView>()
                        .SingleOrDefault(x => x.IsActive);
                    if(win != null)
                    {
                        win.Close();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Messages.MessageBoxError(ex.Message);
                return false;
            }
        }
    }
}

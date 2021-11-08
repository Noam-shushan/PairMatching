using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;

namespace Gui.Models
{
    public class NotesModel
    {
        public List<Note> Notes { get; set; }

        public NotesModel(List<Note> notes)
        {
            Notes = notes;
        }


    }
}

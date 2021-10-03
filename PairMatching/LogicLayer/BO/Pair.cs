using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;
using LogicLayer;
using RazorEngine.Templating;

namespace BO
{
    public class Pair
    {

        public Pair() { }

        public Pair CreateFromDO(DO.Pair pairDo, Func<int, SimpleStudent> createSimpleStudentFunc)
        {
            var res = pairDo.CopyPropertiesToNew(typeof(Pair)) as Pair;
            res.StudentFromIsrael = createSimpleStudentFunc(pairDo.StudentFromIsraelId);
            res.StudentFromWorld = createSimpleStudentFunc(pairDo.StudentFromWorldId);
            foreach(var n in pairDo.Notes)
            {
                res.NotesBo.Add(n.CopyPropertiesToNew(typeof(Note)) as Note);
            }
            return res;
        }

        public ObservableCollection<Note> NotesBo { get; set; } = new ObservableCollection<Note>();

        public void EditPrefferdTracks(string track)
        {
            PrefferdTracks = Dictionaries.PrefferdTracksDictInverse[track];
        }

        public int Id { get; set; }
        /// <summary>
        /// flag that determine if the pair is deleted from the data source 
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// The student from israel 
        /// </summary>
        public SimpleStudent StudentFromIsrael { get; set; }

        /// <summary>
        /// The macher student from world 
        /// </summary>
        public SimpleStudent StudentFromWorld { get; set; }

        /// <summary>
        /// the first student id 
        /// </summary>
        public int StudentFromIsraelId { get; set; }

        /// <summary>
        /// The macher student id for the first student
        /// </summary>
        public int StudentFromWorldId { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime DateOfUpdate { get; set; }

        public DateTime DateOfDelete { get; set; }

        public bool IsActive { get; set; } = false;

        public string InfoAbout { get; set; } = "";

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks PrefferdTracks { get; set; }

        public string PrefferdTracksShow
        {
            get => Dictionaries.PrefferdTracksDict[PrefferdTracks];
        }

        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return $"חברותא א: {StudentFromIsrael.Name} , חברותא ב: {StudentFromWorld.Name}";
        }
    }
}

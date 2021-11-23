using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;
using LogicLayer;
using UtilEntities;

namespace BO
{
    public class Pair : IEquatable<Pair>
    {

        public Pair() { }

        public Pair CreateFromDO(DO.Pair pairDo, Func<int, SimpleStudent> createSimpleStudentFunc)
        {
            var res = pairDo.CopyPropertiesToNew(typeof(Pair)) as Pair;
            res.StudentFromIsrael = createSimpleStudentFunc(pairDo.StudentFromIsraelId);
            res.StudentFromWorld = createSimpleStudentFunc(pairDo.StudentFromWorldId);
            return res;
        }

        public List<Note> Notes { get; set; } = new List<Note>();

        public void EditPrefferdTracks(string track)
        {
            PrefferdTracks = Dictionaries.PrefferdTracksDictInverse[track];
        }

        public int Id { get; set; }
        /// <summary>
        /// flag that determine if the pair is deleted from the database 
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

        public bool Equals(Pair other)
        {
            if(other == null)
            {
                return false;
            }
            return other.Id == Id;
        }

        public override bool Equals(object obj) => Equals(obj as Pair);

        public override int GetHashCode()
        {
            return (Id, StudentFromIsraelId, StudentFromWorldId).GetHashCode();
        }
    }
}

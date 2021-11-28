using System;
using System.ComponentModel;
using BO;
using UtilEntities;

namespace Gui.ViewModels
{
    public class PairViewModel : INotifyPropertyChanged
    {
        private Pair _pair;

        public PairViewModel(Pair pair)
        {
            _pair = pair;
            Notes = new NotesViewModel(_pair.Notes);
        }

        public NotesViewModel Notes { get; set; }

        public int Id { get => _pair.Id; }

        /// <summary>
        /// The student from israel 
        /// </summary>
        public SimpleStudent StudentFromIsrael { get => _pair.StudentFromIsrael; }

        /// <summary>
        /// The macher student from world 
        /// </summary>
        public SimpleStudent StudentFromWorld { get => _pair.StudentFromWorld; }

        public DateTime DateOfCreate { get => _pair.DateOfCreate; }

        public DateTime DateOfUpdate 
        { 
            get => _pair.DateOfUpdate;
            set 
            {
                _pair.DateOfUpdate = value;
                OnPropertyChanged("DateOfUpdate");
            }
        }

        public DateTime DateOfDelete
        {
            get => _pair.DateOfDelete;
            set
            {
                _pair.DateOfDelete = value;
                OnPropertyChanged("DateOfDelete");
            }
        }

        public bool IsActive
        {
            get => _pair.IsActive;
            set
            {
                _pair.IsActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public string PrefferdTracks
        {
            get => Dictionaries.PrefferdTracksDict[_pair.PrefferdTracks];
            set
            {
                _pair.PrefferdTracks = Dictionaries.PrefferdTracksDictInverse[value];
                OnPropertyChanged("PrefferdTracks");
            }
        }

        public bool IsSelected { get; set; }

        public bool IsChanged { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            IsChanged = true;
        }
    }
}

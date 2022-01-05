using Gui.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui.ViewModels
{
    public class SearchViewModel : NotifyPropertyChanged
    {
        private string _searchText;
        public string SearchText 
        { 
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            } 
        }

        public SearchCommand Search { get; set; }

        public SearchViewModel(MainViewModelBase mainViewModel)
        {
            Search = new SearchCommand(mainViewModel);
        }

    }
}
